using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace starss.starssCode.Powers;


public sealed class OrangeDogSpacePower : starssPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    // true = 正常清除格挡；false = 不清除格挡
    public override bool ShouldClearBlock(Creature creature)
    {
        return Owner != creature;
    }
}