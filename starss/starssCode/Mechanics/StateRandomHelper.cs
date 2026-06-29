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
        };

        var index = Random.Shared.Next(states.Count);
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
        };

        return states
            .OrderBy(_ => Random.Shared.Next())
            .Take(count)
            .Select(factory => factory())
            .ToList();
    }
}

