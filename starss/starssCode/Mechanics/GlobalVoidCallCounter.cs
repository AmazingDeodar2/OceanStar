using MegaCrit.Sts2.Core.Entities.Players;
using System.Collections.Generic;

namespace starss.starssCode.Mechanics;

public static class GlobalVoidCallCounter
{
    // 每位玩家独立本局计数
    private static readonly Dictionary<Player, decimal> _totalGenerated = new();

    public static decimal GetTotalCount(Player player)
    {
        _totalGenerated.TryGetValue(player, out var count);
        return count;
    }

    public static void Increment(Player player)
    {
        if (!_totalGenerated.ContainsKey(player))
            _totalGenerated[player] = 0;
        _totalGenerated[player]++;
    }

    // 单玩家战斗结束清理
    public static void ClearForPlayer(Player player) => _totalGenerated.Remove(player);
    // 全部清空，防多局残留
    public static void ClearAll() => _totalGenerated.Clear();
}