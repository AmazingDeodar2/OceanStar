using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using starss.starssCode.Cards.Interfaces;
using starss.starssCode.Relics;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Models;
using VoidCard = MegaCrit.Sts2.Core.Models.Cards.Void;
using BeckonCard = MegaCrit.Sts2.Core.Models.Cards.Beckon;
namespace starss.starssCode.Mechanics;

public static class SevenSevenSevenHelper
{
    private static readonly Dictionary<Player, int> Counts = new();

    public static async Task AfterCardGeneratedForCombat(
        CardModel card,
        Player? creator)
    {
        if (creator == null)
            return;

        if (card is not VoidCard && card is not BeckonCard)
            return;

        var relic = creator.GetRelic<SevenSevenSeven>();
        if (relic == null)
            return;

        Counts.TryGetValue(creator, out var count);
        count++;

        while (count >= 7)
        {
            count -= 7;

            relic.Flash();

            var enemies = creator.Creature.CombatState.HittableEnemies.ToList();
            if (enemies.Count > 0)
            {
                var target = creator.RunState.Rng.CombatTargets.NextItem<Creature>(enemies);

                if (target != null)
                {
                    await CreatureCmd.Damage(
                        new ThrowingPlayerChoiceContext(),
                        target,
                        70M,
                        ValueProp.Unpowered,
                        creator.Creature
                    );
                }
            }
        }

        Counts[creator] = count;
    }

    public static void Clear()
    {
        Counts.Clear();
    }
}