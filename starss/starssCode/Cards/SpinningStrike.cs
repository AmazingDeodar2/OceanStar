using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace starss.starssCode.Cards;


public sealed class SpinningStrike : starssCard
{
    public SpinningStrike()
        : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override HashSet<CardTag> CanonicalTags =>
    [
        CardTag.Strike
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(14M, ValueProp.Move)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        // 自动打出时 cardPlay.Target 已被 CardCmd.AutoPlay 随机选好（CardCmd.cs:73-84）。
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    /// <summary>
    /// 回合结束时（AutoPostPlay 阶段，手牌尚未 flush）：若手牌或弃牌堆中有状态牌，
    /// 且本牌在抽牌堆或弃牌堆中，则自动打出（免费）。
    ///
    /// 模板照官方 HowlFromBeyond.cs:32-39（彼岸咆哮，回合结束自动打出）。
    /// 牌在非常规堆也能自动打出，因为 OnPlayWrapper 对 isAutoPlay 会无条件先移到
    /// Play 堆（CardModel.cs:1868-1875）。
    /// 手牌此时仍在手上：Hand flush 发生在 End 阶段（PlayerTurnPhase.cs:50-60）。
    /// </summary>
    public override async Task AfterAutoPostPlayPhaseEntered(PlayerChoiceContext choiceContext, Player player)
    {
        CardPile? pile = Pile;

        if (pile == null || player != Owner)
            return;

        if (pile.Type != PileType.Draw && pile.Type != PileType.Discard)
            return;

        if (!HasStatusCard())
            return;

        await CardCmd.AutoPlay(choiceContext, this, null);
    }

    /// <summary>手牌或弃牌堆中是否有状态牌（本牌是 Attack，不会把自己算进去）。</summary>
    private bool HasStatusCard()
    {
        return
            PileType.Hand.GetPile(Owner).Cards.Any(card =>
                card.Type == CardType.Status)
            ||
            PileType.Discard.GetPile(Owner).Cards.Any(card =>
                card.Type == CardType.Status);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3M);
    }
}
