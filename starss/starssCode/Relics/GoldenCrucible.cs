
using System.Collections.Generic;
using System.Threading.Tasks;

using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace starss.starssCode.Relics;


public sealed class GoldenCrucible : RelicModel
{
    private int _timesUsed;

    // 先古遗物
    public override RelicRarity Rarity => RelicRarity.Ancient;

    // 仅允许在单人模式中出现
    public override bool IsAllowed(IRunState runState)
    {
        return runState.Players.Count == 1;
    }

    // 总共生效两次
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[]
        {
            new CardsVar(2)
        };

    // 两次使用完毕后失效
    public override bool IsUsedUp =>
        TimesUsed >= DynamicVars.Cards.IntValue;

    // 未失效时显示剩余次数
    public override bool ShowCounter =>
        !IsUsedUp;

    public override int DisplayAmount =>
        DynamicVars.Cards.IntValue - TimesUsed;

    // 保存已经触发的次数
    [SavedProperty]
    public int TimesUsed
    {
        get => _timesUsed;

        set
        {
            AssertMutable();

            _timesUsed = value;

            InvokeDisplayAmountChanged();

            if (IsUsedUp)
            {
                Status = RelicStatus.Disabled;
            }
        }
    }

    // 宝箱房间的额外奖励中添加一个随机遗物
    public override bool TryModifyRewards(
        Player player,
        List<Reward> rewards,
        AbstractRoom? room)
    {
        // 只对遗物持有者生效
        if (player != Owner)
            return false;

        // 只在宝箱房间生效
        if (room is not TreasureRoom)
            return false;

        // 使用次数耗尽后不再生效
        if (IsUsedUp)
            return false;

        // 添加一个随机遗物奖励
        rewards.Add(new RelicReward(player));

        return true;
    }

    // 成功修改奖励后消耗一次使用次数
    public override Task AfterModifyingRewards()
    {
        if (!IsUsedUp)
        {
            TimesUsed++;
        }

        return Task.CompletedTask;
    }
}
