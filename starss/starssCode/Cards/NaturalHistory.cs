using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using starss.starssCode.Powers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace starss.starssCode.Cards;


public sealed class NaturalHistory : starssCard
{
    public NaturalHistory()
        : base(
            1,
            CardType.Power,
            CardRarity.Uncommon,
            TargetType.Self)
    {
    }


    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<NaturalHistoryPower>(50M)
    ];


    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(
            Owner.Creature,
            "PowerUp",
            Owner.Character.PowerUpAnimDelay
        );


        await PowerCmd.Apply<NaturalHistoryPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars["NaturalHistoryPower"].BaseValue,
            Owner.Creature,
            this
        );
    }


    protected override void OnUpgrade()
    {
        DynamicVars["NaturalHistoryPower"]
            .UpgradeValueBy(25M);
    }
}