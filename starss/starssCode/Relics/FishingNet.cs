using System.Collections.Generic;
using System.Threading.Tasks;

using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace starss.starssCode.Relics;

 
public sealed class FishingNet : RelicModel
{
    private const string CombatsKey = "Combats";

    private int _combatsSeen;

    // 先古遗物
    public override RelicRarity Rarity => RelicRarity.Ancient;

    // 始终显示战斗计数器
    public override bool ShowCounter => true;

    // 显示当前累计的普通战斗次数：0~5
    public override int DisplayAmount =>
        CombatsSeen % DynamicVars[CombatsKey].IntValue;

    // 每6次普通战斗触发一次
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[]
        {
            new DynamicVar(CombatsKey, 6m)
        };

    // 保存普通战斗计数
    [SavedProperty]
    public int CombatsSeen
    {
        get => _combatsSeen;

        set
        {
            AssertMutable();

            _combatsSeen = value;

            InvokeDisplayAmountChanged();
        }
    }

    // 普通战斗胜利后触发
    public override Task AfterCombatVictory(CombatRoom room)
    {
        // 仅统计普通战斗
        // 不统计精英战斗、Boss战斗
        if (room.RoomType != RoomType.Monster)
        {
            return Task.CompletedTask;
        }

        // 普通战斗计数 +1
        CombatsSeen++;

        // 每累计6次触发一次
        if (CombatsSeen % DynamicVars[CombatsKey].IntValue != 0)
        {
            return Task.CompletedTask;
        }

        // 遗物闪光
        Flash();

        // 向当前战斗奖励中添加一次删牌奖励
        room.AddExtraReward(
            Owner,
            new CardRemovalReward(Owner)
        );

        return Task.CompletedTask;
    }
}