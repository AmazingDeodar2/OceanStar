
using System.Collections.Generic;
using MegaCrit.Sts2.Core.Entities.Players;
using starss.starssCode.Mechanics;
using starss.starssCode.States;

public static class StateRegistry
{
    private static readonly Dictionary<Player, StateSpace> Spaces = new();

    public static StateSpace Get(Player player)
    {
        if (!Spaces.TryGetValue(player, out var stateSpace))
        {
            stateSpace = new StateSpace(player);
            Spaces[player] = stateSpace;
        }

        return stateSpace;
    }

    public static void Remove(Player player)
    {
        Spaces.Remove(player);
    }
    
    public static void ClearAll()
    {
        Spaces.Clear();
        PonyLandState.ClearGoldCounters();
        PineappleModelHelper.Clear();
        SevenSevenSevenHelper.Clear();
    }
}