using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using starss.starssCode.Mechanics;
using starss.starssCode.Powers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Models;

namespace starss.starssCode.Cards;

public sealed class LuckyFinish : starssCard
{
    private const string CalculatedDamageKey = "CalculatedDamage";

    public LuckyFinish()
        : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
    }

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new FateVar(50M),
        new CalculationBaseVar(0M),
        new CalculationExtraVar(1M),
        new CalculatedVar(CalculatedDamageKey)
            .WithMultiplier((card, _) => GetLuckDamage(card))
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        var damage = GetLuckDamage(this);

        await DamageCmd.Attack(damage)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        var check = await DiceHelper.Check(
            Owner.Creature,
            fate: DynamicVars["Fate"].IntValue,
            doom: 101,
            choiceContext: choiceContext,
            sourceCard: this
        );

        if (check.FateSuccess)
        {
           

            await CreatureCmd.GainBlock(
                Owner.Creature,
                damage,
                ValueProp.Unpowered,
                cardPlay
            );
        }
    }

    private static decimal GetLuckDamage(CardModel card)
    {
        var luck = card.Owner.Creature.GetPower<LuckyPower>()?.Amount ?? 0M;
        return Math.Floor(luck / 2M);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}