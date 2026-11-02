using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Models;
using starss.starssCode.Cards;
using starss.starssCode.Cards.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace starss.starssCode.Mechanics;

/// <summary>
/// PC 牌池的**唯一出口**。卡牌 / 遗物 / 药水要随机拿 PC 牌，一律走这里，
/// 不要各自内联 <c>GetUnlockedCards(...).Where(c =&gt; c is IPcCard)</c>。
/// 判定依据是 <see cref="IPcCard"/> 标记接口，新增 PC 牌只要实现该接口即可自动入池。
/// </summary>
public static class PcCardHelper
{
    public static IEnumerable<CardModel> GetPcCardPool(Player player)
    {
        return player.Character.CardPool
            .GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint)
            .Where(c => c is IPcCard);
    }

    public static List<CardModel> GetPcChoices(Player player, int count)
    {
        return CardFactory
            .GetDistinctForCombat(
                player,
                GetPcCardPool(player),
                count,
                player.RunState.Rng.CombatCardGeneration
            )
            .ToList();
    }

    /// <summary>随机抽一张 PC 牌；池子为空时返回 <c>null</c>。</summary>
    public static CardModel? GetRandomPcCard(Player player)
    {
        return GetPcChoices(player, 1).FirstOrDefault();
    }
}