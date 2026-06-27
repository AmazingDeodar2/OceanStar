using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using starss.starssCode.States;
using System.Threading.Tasks;
using starss.starssCode.Cards.Interfaces;
using starss.starssCode.Mechanics;

namespace starss.starssCode.Cards;


public sealed class ThreeW : starssCard, IPcCard
{
    public ThreeW()
        : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(
            Owner.Creature,
            "Cast",
            Owner.Character.CastAnimDelay
        );

        await StateCmd.Enter(
            choiceContext,
            Owner,
            new OrangeDogSpaceState()
        );
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}