using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using starss.starssCode.Powers;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.ValueProps;

namespace starss.starssCode.Cards;

public sealed class MakkaPakka : starssCard
{
    public MakkaPakka()
        : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get
        {
            return new DynamicVar[]
            {
                new DamageVar("Damage", 6M, ValueProp.Unpowered)
            };
        }
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(
            Owner.Creature,
            "PowerUp",
            Owner.Character.PowerUpAnimDelay
        );

        await PowerCmd.Apply<MakkaPakkaPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars.Damage.BaseValue,
            Owner.Creature,
            this
        );
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Damage"].UpgradeValueBy(2M);
    }
}