using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using starss.starssCode.Relics;

namespace starss.starssCode.Mechanics;

public static class FateRelicHelper
{
    private static readonly Dictionary<Player, int> RabbitFootCounter = new();

    public static async Task OnFateTriggered(
        PlayerChoiceContext choiceContext,
        Player player)
    {
        var relic = player.GetRelic<RabbitFoot>();
        if (relic == null)
            return;

        RabbitFootCounter.TryGetValue(player, out var count);
        count++;

        if (count < 3)
        {
            RabbitFootCounter[player] = count;
            return;
        }

        RabbitFootCounter[player] = 0;

        relic.Flash();

        foreach (var enemy in player.Creature.CombatState.HittableEnemies.ToList())
        {
            await CreatureCmd.Damage(
                choiceContext,
                enemy,
                7M,
                ValueProp.Unpowered,
                player.Creature
            );
        }
    }

    public static void Clear()
    {
        RabbitFootCounter.Clear();
    }
}