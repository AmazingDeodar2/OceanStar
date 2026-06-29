using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using starss.starssCode.Mechanics;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace starss.starssCode.Cards;


public sealed class EverChanging : starssCard
{
    public EverChanging()
        : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override bool HasEnergyCostX => true;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int count = ResolveEnergyXValue();

        if (IsUpgraded)
            count++;

        await CreatureCmd.TriggerAnim(
            Owner.Creature,
            "Cast",
            Owner.Character.CastAnimDelay
        );

        List<StateModel> states = StateRandomHelper.GetRandomDifferentStates(Owner, count);

        foreach (StateModel state in states)
        {
            await StateCmd.Enter(
                choiceContext,
                Owner,
                state
            );
        }
    }

    protected override void OnUpgrade()
    {
    }
}