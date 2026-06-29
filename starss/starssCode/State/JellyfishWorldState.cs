using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using starss.starssCode.Mechanics;
using starss.starssCode.Powers;

namespace starss.starssCode.States;

public sealed class JellyfishWorldState : StateModel
{
    public override string Id => "starss:jellyfish_world";

    public override string DisplayName => "水母世界";

    public JellyfishWorldState()
    {
        Duration = int.MaxValue;
    }

    public override async Task OnEnter(PlayerChoiceContext choiceContext)
    {
        await PowerCmd.Apply<JellyfishWorldPower>(
            choiceContext,
            Owner.Creature,
            1M,
            Owner.Creature,
            null
        );
    }

    public override async Task OnExit(PlayerChoiceContext choiceContext)
    {
        await PowerCmd.Remove<JellyfishWorldPower>(Owner.Creature);
    }
}