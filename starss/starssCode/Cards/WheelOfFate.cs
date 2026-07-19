using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using starss.starssCode.Powers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace starss.starssCode.Cards;

public sealed class WheelOfFate : starssCard
{
    public WheelOfFate()
        : base(0, CardType.Attack, CardRarity.Ancient, TargetType.AllEnemies)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(12M, ValueProp.Move),
        new DynamicVar("Luck", 40M)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this,cardPlay)
            .TargetingAllOpponents(CombatState!)
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
        DynamicVars.Damage.UpgradeValueBy(4M);
        DynamicVars["Luck"].UpgradeValueBy(10M);
    }
}