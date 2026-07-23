using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using starss.starssCode.Mechanics;
using starss.starssCode.Powers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Models.Powers;

namespace starss.starssCode.Cards;

public sealed class LetUsBegin : starssCard
{
    public LetUsBegin()
        : base(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Innate,
        CardKeyword.Exhaust
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(4M, ValueProp.Move),
        new DynamicVar("Repeat", 2M),
        new FateVar(50M),
        new DynamicVar("Bonus", 1M),
        new CardsVar(1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        for (int i = 0; i < DynamicVars["Repeat"].IntValue; i++)
            await AttackEnemy(choiceContext, cardPlay.Target, cardPlay);

        await PowerCmd.Apply<DrawCardsNextTurnPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars.Cards.BaseValue,
            Owner.Creature,
            this
        );

        var check = await DiceHelper.Check(
            Owner.Creature,
            fate: DynamicVars["Fate"].IntValue,
            doom: 101,
            choiceContext: choiceContext,
            sourceCard: this
        );

        if (check.FateSuccess)
        {
            for (int i = 0; i < DynamicVars["Bonus"].IntValue; i++)
                await AttackEnemy(choiceContext, cardPlay.Target, cardPlay);

            await PowerCmd.Apply<DrawCardsNextTurnPower>(
                choiceContext,
                Owner.Creature,
                DynamicVars.Cards.BaseValue,
                Owner.Creature,
                this
            );
        }
        if (check.HardSuccess)
        {
            for (int i = 0; i < DynamicVars["Bonus"].IntValue; i++)
                await AttackEnemy(choiceContext, cardPlay.Target, cardPlay);

            await PowerCmd.Apply<DrawCardsNextTurnPower>(
                choiceContext,
                Owner.Creature,
                DynamicVars.Cards.BaseValue,
                Owner.Creature,
                this
            );
        }
    }

    private async Task AttackEnemy(PlayerChoiceContext choiceContext, Creature target, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Bonus"].UpgradeValueBy(1M);
    }
}