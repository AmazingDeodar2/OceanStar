using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace starss.starssCode.Powers;


public sealed class JellyfishWorldPower : starssPower
{
    public override PowerType Type => PowerType.Buff;

    // 单实例，不叠层
    public override PowerStackType StackType => PowerStackType.Single;

    // true = 正常弃牌；false = 不弃牌
    public override bool ShouldFlush(Player player)
    {
        return Owner.Player != player;
    }
}