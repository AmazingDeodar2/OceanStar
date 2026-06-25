using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using starss.starssCode.Mechanics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Models;

namespace starss.starssCode.Cards;

public sealed class WildIdea : starssCard
{
    public WildIdea()
        : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(14M, ValueProp.Move),
        new FateVar(50M),
        new DoomVar(50M)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        var check = await DiceHelper.Check(
            Owner.Creature,
            fate: 50,
            doom: 50,
            choiceContext: choiceContext,
            sourceCard: this
        );

        if (check.FateSuccess)
        {
            await PowerCmd.Apply<FreePowerPower>(
                choiceContext,
                Owner.Creature,
                1M,
                Owner.Creature,
                this
            );
        }

        if (check.DoomSuccess)
        {
            CardPile pile = PileType.Hand.GetPile(Owner);
            CardModel card = Owner.RunState.Rng.CombatCardSelection.NextItem<CardModel>(pile.Cards);

            if (card != null)
                await CardCmd.Exhaust(choiceContext, card);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(6M);
    }
}