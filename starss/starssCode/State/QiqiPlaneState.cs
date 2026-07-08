using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using starss.starssCode.Mechanics;

namespace starss.starssCode.States;

public sealed class QiqiPlaneState : StateModel
{
    public override string Id => "starss:qiqi_plane";

    public override string DisplayName => "淇喵位面";
    private static readonly Dictionary<Player, int> EnterCountByPlayer = new();
    public QiqiPlaneState()
    {
        Duration = int.MaxValue;
    }
    private int GetEnterCount(Player player)
    {
        if (!EnterCountByPlayer.TryGetValue(player, out int count))
            count = 0;
        return count;
    }
    public override async Task OnEnter(PlayerChoiceContext choiceContext)
    {
        int enterTimes = GetEnterCount(Owner);
        decimal blockAmount = 8M + enterTimes * 3M;
        await CreatureCmd.GainBlock(
            Owner.Creature,
            blockAmount,
            ValueProp.Unpowered,
            null
        );
        EnterCountByPlayer[Owner] = enterTimes + 1;
    }

    public override async Task OnExit(PlayerChoiceContext choiceContext)
    {
        int enterTimes = GetEnterCount(Owner);
        decimal damageAmount = 8M + enterTimes * 3M;
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
            damageAmount,
            ValueProp.Unpowered,
            Owner.Creature
        );
    }
    public static void ClearEnterCounters()
    {
        EnterCountByPlayer.Clear();
    }
}