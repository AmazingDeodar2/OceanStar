using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using starss.starssCode.Relics;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace starss.starssCode.Mechanics;

public static class PineappleModelHelper
{
    private static readonly HashSet<Player> TriggeredThisCombat = new();

    public static async Task OnEnterState(
        PlayerChoiceContext choiceContext,
        Player player)
    {
        

        var relic = player.GetRelic<PineappleModel>();

        if (relic == null)
            return;
        if (TriggeredThisCombat.Contains(player))
            return;
        
        TriggeredThisCombat.Add(player);
        relic.Flash();

        

        foreach (var enemy in player.Creature.CombatState.HittableEnemies)
        {
            await PowerCmd.Apply<VulnerablePower>(
                choiceContext,
                enemy,
                2M,
                player.Creature,
                null
            );
        }
    }

    public static void Clear()
    {
        TriggeredThisCombat.Clear();
    }
}