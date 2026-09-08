using System;
using System.Linq;
using System.Reflection;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;

namespace starss.starssCode;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "starss"; //Used for resource filepath
    public const string ResPath = $"res://{ModId}";

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } =
        new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        Logger.Info("[anim][startup] starss Initialize 开始");

        // 把本程序集里给 Godot 场景用的 C# 脚本类（StarssNCreature/StarssEnergyCounter/CustomMegaLabel…）
        // 注册给 Godot，否则 tscn 实例化时会报 "class could not be found" 并退回 BaseLib 转换。
        try
        {
            Godot.Bridge.ScriptManagerBridge.LookupScriptsInAssembly(Assembly.GetExecutingAssembly());
            Logger.Info("[anim][startup] LookupScriptsInAssembly 完成");
        }
        catch (Exception ex)
        {
            Logger.Error($"[anim][startup] LookupScriptsInAssembly 失败：{ex}");
        }

        try
        {
            Harmony harmony = new(ModId);
            harmony.PatchAll();
            int patchedCount = harmony.GetPatchedMethods().Count();
            Logger.Info($"[anim][startup] Harmony PatchAll 完成，全局已补丁方法数={patchedCount}");
        }
        catch (Exception ex)
        {
            Logger.Error($"[anim][startup] Harmony PatchAll 失败：{ex}");
        }
    }
}
