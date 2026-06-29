using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using starss.starssCode.Powers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;

namespace starss.starssCode.Cards;


public sealed class AbyssJudgment : starssCard
{
    private const string CalculatedDamageKey = "CalculatedDamage";

    public AbyssJudgment()
        : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6M, ValueProp.Move),
        new CalculationBaseVar(0M),
        new ExtraDamageVar(4M),
        new CalculatedVar(CalculatedDamageKey)
            .WithMultiplier((card, _) => GetAbyssDamage(card))
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        await DamageCmd.Attack(GetAbyssDamage(this))
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    private static decimal GetAbyssDamage(CardModel card)
    {
        decimal count = card.Owner.Creature.GetPower<AbyssPower>()?.Amount ?? 0M;
        decimal extra = card.DynamicVars.CalculationExtra.BaseValue;

        return card.DynamicVars.Damage.BaseValue + count * extra;
    }

    protected override void OnUpgrade()
    {
        DynamicVars.CalculationExtra.UpgradeValueBy(1M);
    }
}