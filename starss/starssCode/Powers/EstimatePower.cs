using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using System.Threading.Tasks;

namespace starss.starssCode.Powers;


public sealed class EstimatePower : starssPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    /// <summary>
    /// 判断当前玩家是否为获得额外回合的目标。
    /// </summary>
    public override bool ShouldTakeExtraTurn(Player player)
    {
        return Owner.Player == player;
    }

    /// <summary>
    /// 游戏确定给予目标玩家额外回合后，消耗此 Power。
    /// </summary>
    public override Task AfterTakingExtraTurn(Player player)
    {
        if (Owner.Player != player)
            return Task.CompletedTask;

        // 移除 Power，防止后续回合再次触发。
        RemoveInternal();

        return Task.CompletedTask;
    }
}