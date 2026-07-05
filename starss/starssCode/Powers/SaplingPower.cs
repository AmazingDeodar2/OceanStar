using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using starss.starssCode.Cards;
using System.Collections.Generic;

namespace starss.starssCode.Powers;


public sealed class SaplingPower : starssPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Sapling>()
    ];
}