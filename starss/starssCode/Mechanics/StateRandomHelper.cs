using MegaCrit.Sts2.Core.Entities.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using starss.starssCode.States;

namespace starss.starssCode.Mechanics;

public static class StateRandomHelper
{
    public static StateModel GetRandomState(Player player)
    {
        var states = new List<Func<StateModel>>
        {
            () => new PonyLandState(),
            () => new FlatDomainState(),
            () => new OrangeDogSpaceState(),
            () => new JellyfishWorldState(),
            () => new QiqiPlaneState(),
            () => new RatCreviceState(),
            () => new GooseEggKitchenState(),
        };

        var rng = player.RunState.Rng.CombatCardGeneration;

        int index = rng.NextInt(states.Count);

        return states[index]();
    }
    public static List<StateModel> GetRandomDifferentStates(Player player, int count)
    {
        var states = new List<Func<StateModel>>
        {
            () => new PonyLandState(),
            () => new FlatDomainState(),
            () => new OrangeDogSpaceState(),
            () => new JellyfishWorldState(),
            () => new QiqiPlaneState(),
            () => new RatCreviceState(),
            () => new GooseEggKitchenState(),
        };

        player.RunState.Rng.CombatCardGeneration.Shuffle(states);

        return states
            .Take(count)
            .Select(factory => factory())
            .ToList();
    }
}

