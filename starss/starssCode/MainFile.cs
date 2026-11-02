
using System;
using System.Linq;
using System.Reflection;

using BaseLib.Patches.Content;

using Godot;
using HarmonyLib;

using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;

using starss.starssCode.Ancients;
using starss.starssCode.Events;
using starss.starssCode.Relics;

namespace starss.starssCode;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "starss";
    public const string ResPath = $"res://{ModId}";

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } =
        new(
            ModId,
            MegaCrit.Sts2.Core.Logging.LogType.Generic
        );

    private static bool _relicsRegistered;

    // =========================================================
    // Mod 初始化入口
    // =========================================================

    public static void Initialize()
    {
        Logger.Info(
            "[anim][startup] starss Initialize 开始"
        );

        // =====================================================
        // 1. 注册海星先古之民专属遗物
        // 必须在 EventRelicPool 首次初始化前执行
        // =====================================================

        try
        {
            RegisterAncientRelics();
        }
        catch (Exception ex)
        {
            Logger.Error(
                $"[starss][startup] 先古遗物池注册失败：{ex}"
            );
        }

        // =====================================================
        // 2. 检查自定义事件类型
        //
        // 注意：
        // 不调用 ModelDb.Inject()
        // 不调用 new LeakySpoonCircle()
        // 不将事件加入随机事件池
        //
        // 游戏/Mod 框架会在 ModelDb.Init() 中扫描模型。
        // =====================================================

        try
        {
            CheckCustomEventType();
        }
        catch (Exception ex)
        {
            Logger.Error(
                $"[starss][event] 事件类型检查失败：{ex}"
            );
        }

        // =====================================================
        // 3. 注册 Godot C# 脚本
        // =====================================================

        try
        {
            Godot.Bridge.ScriptManagerBridge.LookupScriptsInAssembly(
                Assembly.GetExecutingAssembly()
            );

            Logger.Info(
                "[anim][startup] LookupScriptsInAssembly 完成"
            );
        }
        catch (Exception ex)
        {
            Logger.Error(
                $"[anim][startup] LookupScriptsInAssembly 失败：{ex}"
            );
        }

        // =====================================================
        // 4. 应用 Harmony 补丁
        // =====================================================

        try
        {
            Harmony harmony = new(ModId);

            harmony.PatchAll();

            int patchedCount =
                harmony.GetPatchedMethods().Count();

            Logger.Info(
                "[anim][startup] Harmony PatchAll 完成，" +
                $"全局已补丁方法数={patchedCount}"
            );
        }
        catch (Exception ex)
        {
            Logger.Error(
                $"[anim][startup] Harmony PatchAll 失败：{ex}"
            );
        }

        // =====================================================
        // 5. 检查先古之民注册状态
        // =====================================================

        try
        {
            CheckAncientRegistration();
        }
        catch (Exception ex)
        {
            Logger.Error(
                $"[starss][ancient] 注册检查失败：{ex}"
            );
        }

        Logger.Info(
            "[anim][startup] starss Initialize 完成"
        );
    }

    // =========================================================
    // 注册十件自定义先古遗物
    // =========================================================

    private static void RegisterAncientRelics()
    {
        if (_relicsRegistered)
        {
            Logger.Info(
                "[starss][relic] 遗物已注册，跳过重复注册"
            );

            return;
        }

        Logger.Info(
            "[starss][relic] 开始注册十件先古遗物"
        );

        // 第一组

        ModHelper.AddModelToPool<EventRelicPool, DivineTouch>();
        ModHelper.AddModelToPool<EventRelicPool, OriginAltar>();
        ModHelper.AddModelToPool<EventRelicPool, ShapeshiftingGate>();

        // 第二组

        ModHelper.AddModelToPool<EventRelicPool, MarkedCard>();
        ModHelper.AddModelToPool<EventRelicPool, CreepingVine>();
        ModHelper.AddModelToPool<EventRelicPool, StableStone>();

        // 第三组

        ModHelper.AddModelToPool<EventRelicPool, MetamorphosisPact>();
        ModHelper.AddModelToPool<EventRelicPool, GoldenCrucible>();
        ModHelper.AddModelToPool<EventRelicPool, FishingNet>();
        ModHelper.AddModelToPool<EventRelicPool, GreenGodHeart>();

        _relicsRegistered = true;

        Logger.Info(
            "[starss][relic] 十件先古遗物已提交到 EventRelicPool"
        );
    }

    // =========================================================
    // 检查自定义事件类型
    //
    // 此方法不会实例化事件，不会修改 ModelDb。
    // =========================================================

    private static void CheckCustomEventType()
    {
        Logger.Info(
            "[starss][event] ===== 自定义事件类型检查 ====="
        );

        Type eventType = typeof(LeakySpoonCircle);

        Logger.Info(
            $"[starss][event] 事件类型：{eventType.FullName}"
        );

        Logger.Info(
            $"[starss][event] 是否继承 EventModel：" +
            $"{typeof(EventModel).IsAssignableFrom(eventType)}"
        );

        Logger.Info(
            $"[starss][event] 是否具有公共无参构造函数：" +
            $"{eventType.GetConstructor(Type.EmptyTypes) != null}"
        );

        Logger.Info(
            $"[starss][event] 事件 ModelId：" +
            $"{ModelDb.GetId(eventType)}"
        );

        // 这里仅查询当前 ModelDb 的状态。
        // 如果初始化尚未完成，返回 False 属于正常情况。
        Logger.Info(
            $"[starss][event] 当前 ModelDb.Contains：" +
            $"{ModelDb.Contains(eventType)}"
        );

        Logger.Info(
            "[starss][event] 未执行手动 Inject，等待游戏正常初始化"
        );

        Logger.Info(
            "[starss][event] ===== 类型检查结束 ====="
        );
    }

    // =========================================================
    // 检查先古之民注册状态
    // =========================================================

    private static void CheckAncientRegistration()
    {
        Logger.Info(
            "[starss][ancient] ===== 注册检查 ====="
        );

        bool found = CustomContentDictionary.CustomAncients
            .Any(a => a is StarssAncient);

        Logger.Info(
            $"[starss][ancient] CustomAncients 中存在 StarssAncient: {found}"
        );

        foreach (var ancient in CustomContentDictionary.CustomAncients)
        {
            Logger.Info(
                $"[starss][ancient] 已注册: {ancient.GetType().FullName}"
            );
        }

        Logger.Info(
            "[starss][ancient] ===== 注册检查结束 ====="
        );
    }
}
