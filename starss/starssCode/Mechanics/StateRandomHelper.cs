using MegaCrit.Sts2.Core.Entities.Players;
using System;
using System.Collections.Generic;
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
}