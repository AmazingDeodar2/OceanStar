using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using starss.starssCode.Powers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace starss.starssCode.Cards;

public sealed class RollDice : starssCard
{
    public RollDice()
        : base(0, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(3M, ValueProp.Move),
        new DynamicVar("Luck", 12M)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this,cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        await PowerCmd.Apply<NextCheckLuckPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars["Luck"].BaseValue,
            Owner.Creature,
            this
        );
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1M);
        DynamicVars["Luck"].UpgradeValueBy(8M);
    }
}