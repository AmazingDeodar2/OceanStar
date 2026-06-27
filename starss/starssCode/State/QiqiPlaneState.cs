using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using starss.starssCode.Mechanics;

namespace starss.starssCode.States;

public sealed class QiqiPlaneState : StateModel
{
    public override string Id => "starss:qiqi_plane";

    public override string DisplayName => "淇喵位面";

    public QiqiPlaneState()
    {
        Duration = int.MaxValue;
    }

    public override async Task OnEnter(PlayerChoiceContext choiceContext)
    {
        await CreatureCmd.GainBlock(
            Owner.Creature,
            7M,
            ValueProp.Unpowered,
            null
        );
    }

    public override async Task OnExit(PlayerChoiceContext choiceContext)
    {
        var enemies = Owner.Creature.CombatState.HittableEnemies.ToList();
        if (enemies.Count == 0)
            return;

        var target = Owner.RunState.Rng.CombatTargets.NextItem<Creature>(
            enemies
        );

        if (target == null)
            return;

        await CreatureCmd.Damage(
            choiceContext,
            target,
            7M,
            ValueProp.Unpowered,
            Owner.Creature
        );
    }
}