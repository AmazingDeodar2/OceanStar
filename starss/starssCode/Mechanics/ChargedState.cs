using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace starss.starssCode.Mechanics;

public class ChargedState : StateModel
{
    public override string Id => "starss:charged";

    public ChargedState(int duration)
    {
        Duration = duration;
    }

    public override async Task OnEnter(PlayerChoiceContext choiceContext)
    {
        
        await CreatureCmd.TriggerAnim(
            Owner.Creature,
            "Cast",
            Owner.Character.CastAnimDelay
        );
    }

   

    public override Task OnExit(PlayerChoiceContext choiceContext)
    {
        return Task.CompletedTask;
    }
    public override string DisplayName => "";
}