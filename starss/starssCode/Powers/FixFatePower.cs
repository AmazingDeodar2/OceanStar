using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using starss.starssCode.Cards.Interfaces;

namespace starss.starssCode.Powers;

/// <summary>
/// 修正命运：
/// 1) 非命运（<see cref="IFateCard"/>）的「攻击牌」「技能牌」费用变为 0，
///    并在打出后进入消耗堆
///    —— 照抄官方 CorruptionPower.cs:17-46 的两个钩子（不真的给卡加 Exhaust 关键字）；
///    —— 能力牌（<see cref="CardType.Power"/>）不在范围内，不降费也不消耗。
/// 2) 每打出一张这样的牌，施加 2 点永久幸运（<see cref="LuckyPower"/>）。
///    幸运点数定义在 <see cref="CanonicalVars"/> 的「Luck」上，打出修正命运的卡牌
///    会把自己的同名变量拷进来（写法同 <see cref="TimeReversalPower"/>）。
/// </summary>
public sealed class FixFatePower : starssPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Luck", 2M)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust),
        HoverTipFactory.FromPower<LuckyPower>()
    ];

    /// <summary>只有「非命运」的「攻击牌 / 技能牌」在范围内（能力牌不参与）。</summary>
    private static bool IsAffected(CardModel card)
    {
        if (card is IFateCard)
            return false;

        return card.Type == CardType.Attack || card.Type == CardType.Skill;
    }

    /// <summary>从打出修正命运的卡牌同步幸运点数，保证卡面数字与实际效果一致。</summary>
    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        if (cardSource != null && cardSource.DynamicVars.ContainsKey("Luck"))
        {
            DynamicVars["Luck"].BaseValue = cardSource.DynamicVars["Luck"].BaseValue;
        }

        return Task.CompletedTask;
    }

    public override bool TryModifyEnergyCostInCombatLate(
        CardModel card,
        decimal originalCost,
        out decimal modifiedCost)
    {
        modifiedCost = originalCost;

        if (card.Owner.Creature != Owner || !IsAffected(card))
            return false;

        modifiedCost = 0M;
        return true;
    }

    public override CardLocation ModifyCardPlayResultLocation(
        CardModel card,
        bool isAutoPlay,
        ResourceInfo resources,
        CardLocation location)
    {
        if (card.Owner.Creature != Owner || !IsAffected(card))
            return location;

        location.pileType = PileType.Exhaust;
        return location;
    }

    public override Task AfterModifyingCardPlayResultLocation(
        CardModel card,
        CardLocation cardLocation)
    {
        Flash();
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        CardModel played = cardPlay.Card;

        if (played.Owner.Creature != Owner || !IsAffected(played))
            return;

        await PowerCmd.Apply<LuckyPower>(
            choiceContext,
            Owner,
            DynamicVars["Luck"].BaseValue,
            Owner,
            (CardModel?)null
        );
    }
}
