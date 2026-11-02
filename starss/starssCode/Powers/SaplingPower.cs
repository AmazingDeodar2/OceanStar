using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using starss.starssCode.Cards;
using System.Collections.Generic;

namespace starss.starssCode.Powers;

public sealed class SaplingPower : starssPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<CloverLeaf>()
    ];

    // 使本场战斗所有四叶草叶片额外重放
    public override int ModifyCardPlayCount(
        CardModel card,
        Creature creature,
        int originalPlayCount)
    {
        if (card is not CloverLeaf)
            return originalPlayCount;

        if (card.Owner.Creature != Owner)
            return originalPlayCount;

        return originalPlayCount + Amount;
    }
}