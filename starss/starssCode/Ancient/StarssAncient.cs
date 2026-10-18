
using System;
using System.Collections.Generic;
using System.Linq;

using BaseLib.Abstracts;
using BaseLib.Utils;

using Godot;

using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;

using starss.starssCode.Relics;

namespace starss.starssCode.Ancients;

public class StarssAncient : CustomAncientModel
{
    public StarssAncient()
        : base(
            autoAdd: true,
            logDialogueLoad: true
        )
    {
        GD.Print("[starss Ancient] Constructor called");
    }

    // ==========================================
    // 自定义先古之民场景
    // ==========================================

    public override string? CustomScenePath =>
        "res://starss/scenes/events/starss_ancient.tscn";


    // ==========================================
    // 地图节点图标
    // ==========================================

    public override string? CustomMapIconPath =>
        "res://starss/images/ancients/starss_map.png";

    public override string? CustomMapIconOutlinePath =>
        "res://starss/images/ancients/starss_map_outline.png";


    // ==========================================
    // 历史记录图标
    // ==========================================

    public override string? CustomRunHistoryIconPath =>
        "res://starss/images/ancients/starss_history.png";

    public override string? CustomRunHistoryIconOutlinePath =>
        "res://starss/images/ancients/starss_history_outline.png";


    // ==========================================
    // 调试：检查单件遗物的池归属
    // ==========================================

    private static void CheckRelicPool<T>()
        where T : RelicModel
    {
        string relicName = typeof(T).Name;

        try
        {
            var relic = ModelDb.Relic<T>();

            GD.Print(
                $"[starss Ancient] Checking relic: " +
                $"{relicName}, ID={relic.Id}, " +
                $"Rarity={relic.Rarity}"
            );

            // 不直接访问 relic.Pool，
            // 而是检查所有已注册的遗物池。
            var matches = ModelDb.AllRelicPools
                .Where(pool =>
                    pool.AllRelicIds.Contains(relic.Id))
                .ToList();

            if (matches.Count == 0)
            {
                GD.PushError(
                    $"[starss Ancient] NO POOL: " +
                    $"{relicName}, ID={relic.Id}"
                );
            }
            else
            {
                foreach (var pool in matches)
                {
                    GD.Print(
                        $"[starss Ancient] POOL OK: " +
                        $"{relicName}, Pool={pool.Id}"
                    );
                }
            }
        }
        catch (Exception ex)
        {
            GD.PushError(
                $"[starss Ancient] CHECK FAILED: " +
                $"{relicName}\n{ex}"
            );
        }
    }


    // ==========================================
    // 调试：检查全部十件遗物
    // ==========================================

    private static void CheckAllRelicPools()
    {
        GD.Print(
            "[starss Ancient] ===== RELIC POOL CHECK START ====="
        );

        CheckRelicPool<DivineTouch>();
        CheckRelicPool<OriginAltar>();
        CheckRelicPool<ShapeshiftingGate>();

        CheckRelicPool<MarkedCard>();
        CheckRelicPool<CreepingVine>();
        CheckRelicPool<StableStone>();

        CheckRelicPool<MetamorphosisPact>();
        CheckRelicPool<GoldenCrucible>();
        CheckRelicPool<FishingNet>();
        CheckRelicPool<GreenGodHeart>();

        GD.Print(
            "[starss Ancient] ===== RELIC POOL CHECK END ====="
        );
    }


    // ==========================================
    // 海星先古之民专属奖励池
    // ==========================================

    protected override OptionPools MakeOptionPools
    {
        get
        {
            GD.Print(
                "[starss Ancient] Building option pools"
            );

            // 调试：生成奖励池前检查所有遗物
            CheckAllRelicPools();

            return new OptionPools(

                // ==================================
                // 第一个奖励选项
                // 三件遗物等权重随机
                // ==================================

                MakePool(
                    
                    AncientOption<MarkedCard>(),
                    AncientOption<StableStone>(),
                    AncientOption<GoldenCrucible>(),
                    AncientOption<MetamorphosisPact>()
                    
                ),

                // ==================================
                // 第二个奖励选项
                // 三件遗物等权重随机
                // ==================================

                MakePool(
                    AncientOption<DivineTouch>(),
                    AncientOption<OriginAltar>(),
                    AncientOption<FishingNet>()
                    ),

                // ==================================
                // 第三个奖励选项
                // 四件遗物等权重随机
                // ==================================

                MakePool(
                    AncientOption<CreepingVine>(),
                    AncientOption<ShapeshiftingGate>(),
                    AncientOption<GreenGodHeart>()
                )
            );
        }
    }


    // ==========================================
    // 奖励选项生成
    // ==========================================

    protected override IReadOnlyList<EventOption>
        GenerateInitialOptions()
    {
        GD.Print(
            "[starss Ancient] GenerateInitialOptions START"
        );

        try
        {
            var options = base.GenerateInitialOptions();

            GD.Print(
                $"[starss Ancient] Generated " +
                $"{options.Count} options"
            );

            return options;
        }
        catch (Exception ex)
        {
            GD.PushError(
                "[starss Ancient] Option generation failed: " +
                ex
            );

            throw;
        }
    }


    // ==========================================
    // 章节出现条件
    // 仅允许第一幕正常随机出现
    // ==========================================

    public override bool IsValidForAct(ActModel act)
    {
        bool isFirstAct = act.Index == 0;

        GD.Print(
            $"[starss Ancient] Checking act: {act.Id}, " +
            $"Index={act.Index}, Allowed={isFirstAct}"
        );

        return isFirstAct;
    }


    // ==========================================
    // 不强制刷新
    // 保持正常随机生成机制
    // ==========================================

    public override bool ShouldForceSpawn(
        ActModel act,
        AncientEventModel? rngChosenAncient
    )
    {
        return false;
    }
}
