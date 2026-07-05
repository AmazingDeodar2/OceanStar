using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace starss.starssCode.Cards;


public sealed class FateStrike : starssCard
{
    public FateStrike()
        : base(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
    }
    private bool _returnToHandAfterCardPlayed;
    protected override HashSet<CardTag> CanonicalTags =>
    [
        CardTag.Strike
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(4M, ValueProp.Move)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_starry_impact")
            .Execute(choiceContext);
    }

    public Task OnFateTriggered(PlayerChoiceContext choiceContext)
    {
        if (Pile.Type == PileType.Hand)
            return Task.CompletedTask;

        _returnToHandAfterCardPlayed = true;
        return Task.CompletedTask;
    }
    public override async Task AfterCardPlayedLate(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        if (!_returnToHandAfterCardPlayed)
            return;

        _returnToHandAfterCardPlayed = false;

        if (Pile.Type == PileType.Hand)
            return;

        await CardPileCmd.Add(
            this,
            PileType.Hand
        );
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2M);
    }
}