using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace starss.starssCode.Cards;

public sealed class StarCurse : starssCard
{
    public StarCurse()
        : base(0, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<VulnerablePower>(1M),
        new PowerVar<WeakPower>(1M),
        new PowerVar<PoisonPower>(1M),
        new PowerVar<DoomPower>(1M)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        await PowerCmd.Apply<VulnerablePower>(
            choiceContext,
            cardPlay.Target,
            DynamicVars.Vulnerable.BaseValue,
            Owner.Creature,
            this);

        await PowerCmd.Apply<WeakPower>(
            choiceContext,
            cardPlay.Target,
            DynamicVars.Weak.BaseValue,
            Owner.Creature,
            this);

        await PowerCmd.Apply<PoisonPower>(
            choiceContext,
            cardPlay.Target,
            DynamicVars.Poison.BaseValue,
            Owner.Creature,
            this);

        await PowerCmd.Apply<DoomPower>(
            choiceContext,
            cardPlay.Target,
            DynamicVars.Doom.BaseValue,
            Owner.Creature,
            this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Vulnerable.UpgradeValueBy(1M);
        DynamicVars.Weak.UpgradeValueBy(1M);
        DynamicVars.Poison.UpgradeValueBy(1M);
        DynamicVars.Doom.UpgradeValueBy(1M);
    }
}