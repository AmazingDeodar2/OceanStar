using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using starss.starssCode.Mechanics;
using starss.starssCode.Powers;

namespace starss.starssCode.States;

public sealed class OrangeDogSpaceState : StateModel
{
    public override string Id => "starss:orange_dog_space";

    public override string DisplayName => "橘狗空间";

    public OrangeDogSpaceState()
    {
        Duration = int.MaxValue;
    }

    public override async Task OnEnter(PlayerChoiceContext choiceContext)
    {
        await PowerCmd.Apply<OrangeDogSpacePower>(
            choiceContext,
            Owner.Creature,
            1M,
            Owner.Creature,
            null
        );
    }

    public override async Task OnExit(PlayerChoiceContext choiceContext)
    {
        await PowerCmd.Remove<OrangeDogSpacePower>(Owner.Creature);
    }
}