using System.Collections.Generic;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;

namespace starss.starssCode.Powers;

/// <summary>
/// 坦途：困难成功（投点 ≤ 命运值/2）时，命运效果额外触发 <see cref="PowerModel.Amount"/> 次。
///
/// 本能力自身不需要任何钩子——判定与执行都在 <c>FateEffect</c> 里，
/// 每次命运牌结算时用 <c>Creature.GetPowerAmount&lt;SmoothPathPower&gt;()</c> 读取层数
/// （<c>Creature.cs:596</c>）。
/// </summary>
public sealed class SmoothPathPower : starssPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(MegaCrit.Sts2.Core.Entities.Cards.CardKeyword.Innate)
    ];
}
