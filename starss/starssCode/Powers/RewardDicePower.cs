using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace starss.starssCode.Powers;

public sealed class RewardDicePower : starssPower
{
    public override PowerType Type => PowerType.Buff;

    // 每层奖励骰额外投一次，取最低值。
    // 1层 = 投2次取低值。
    public override PowerStackType StackType => PowerStackType.Counter;
}