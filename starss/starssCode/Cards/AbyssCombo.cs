using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Threading.Tasks;
using VoidCard = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace starss.starssCode.Cards;


public sealed class AbyssCombo : starssCard
{
    public AbyssCombo()
        : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.RandomEnemy)
    {
    }

    // 奇巧：回合结束前从手牌被弃置时免费打出（官方 Ricochet 同款 keyword）。
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Sly
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<VoidCard>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(4M, ValueProp.Move),
        new DynamicVar("Hits", 4M)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Ricochet.cs:26-31 模型：TargetType.RandomEnemy ⇒ cardPlay.Target 为 null，
        // 多段伤害由引擎随机分配到敌人身上。
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .WithHitCount(DynamicVars["Hits"].IntValue)
            .FromCard(this, cardPlay)
            .TargetingRandomOpponents(CombatState!)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        // CardPlay.IsAutoPlay（CardPlay.cs:46）：手动从手牌打出 = false；
        // 奇巧弃置触发的自动打出 = true（CardCmd.cs:192 先进弃牌堆 → :203 AutoPlay → :130）。
        // 只有主动打出才塞虚空，奇巧触发不塞。
        if (cardPlay.IsAutoPlay)
            return;

        CardModel voidCard = Owner.Creature.CombatState.CreateCard<VoidCard>(Owner);

        await CardPileCmd.AddGeneratedCardToCombat(
            voidCard,
            PileType.Discard,
            Owner
        );

        PileType.Discard.GetPile(Owner).InvokeCardAddFinished();
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Hits"].UpgradeValueBy(1M);
    }
}
