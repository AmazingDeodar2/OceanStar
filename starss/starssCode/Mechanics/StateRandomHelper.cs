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
    public static StateModel GetRandomDifferentState(Player player)
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

        // 当前已经存在的状态ID
        var currentIds = StateRegistry.Get(player)
            .States
            .Select(s => s.Id)
            .ToHashSet();

        // 去掉已经存在的状态
        states = states
            .Where(factory => !currentIds.Contains(factory().Id))
            .ToList();

        if (states.Count == 0)
        {
            // 理论上只有容量>=7才会发生
            return GetRandomState(player);
        }

        var rng = player.RunState.Rng.CombatCardGeneration;
        int index = rng.NextInt(states.Count);

        return states[index]();
    }
}
    


