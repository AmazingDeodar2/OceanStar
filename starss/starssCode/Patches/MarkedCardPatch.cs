
using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Odds;
using MegaCrit.Sts2.Core.Runs;
using starss.starssCode.Relics;

namespace starss.starssCode.Patches;

[HarmonyPatch]
public static class MarkedCardPatch
{
    private const float RareMultiplier = 1.5f;

    // 仅在当前卡牌奖励生成期间启用
    [ThreadStatic]
    private static bool _markedCardActive;

    [ThreadStatic]
    private static CardRarityOdds? _currentOdds;

    [HarmonyPatch(typeof(CardFactory), "RollForRarity")]
    [HarmonyPrefix]
    private static void RollForRarityPrefix(Player player)
    {
        _markedCardActive = player.Relics.Any(
            relic => relic is MarkedCard
        );

        _currentOdds = _markedCardActive
            ? player.PlayerOdds.CardRarity
            : null;
    }

    [HarmonyPatch(typeof(CardFactory), "RollForRarity")]
    [HarmonyPostfix]
    private static void RollForRarityPostfix()
    {
        _markedCardActive = false;
        _currentOdds = null;
    }

    // 稀有度抽取时提高稀有卡概率
    [HarmonyPatch(typeof(CardRarityOdds),
        nameof(CardRarityOdds.RollWithoutChangingFutureOdds),
        typeof(CardRarityOddsType),
        typeof(float))]
    [HarmonyPrefix]
    private static void ModifyRareOdds(
        CardRarityOdds __instance,
        CardRarityOddsType type,
        ref float offset)
    {
        if (!_markedCardActive)
            return;

        if (!ReferenceEquals(__instance, _currentOdds))
            return;

        if (type != CardRarityOddsType.RegularEncounter &&
            type != CardRarityOddsType.EliteEncounter)
            return;

        float baseRareOdds = type == CardRarityOddsType.RegularEncounter
            ? CardRarityOdds.RegularRareOdds
            : CardRarityOdds.EliteRareOdds;

        // 目标：最终稀有卡阈值变为原来的 1.5 倍
        float originalRareOdds = Math.Max(
            0f,
            baseRareOdds + offset
        );

        float targetRareOdds = Math.Min(
            1f,
            originalRareOdds * RareMultiplier
        );

        offset = targetRareOdds - baseRareOdds;
    }
}
