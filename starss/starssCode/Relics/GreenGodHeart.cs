
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.ValueProps;

namespace starss.starssCode.Relics;

public sealed class GreenGodHeart : RelicModel
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool HasUponPickupEffect => true;

    // 当前已制作的其他九件海星遗物
    private static List<RelicModel> GetStarfishRelics()
    {
        return new List<RelicModel>
        {
            ModelDb.Relic<DivineTouch>(),
            ModelDb.Relic<OriginAltar>(),
            ModelDb.Relic<MarkedCard>(),
            ModelDb.Relic<ShapeshiftingGate>(),
            ModelDb.Relic<MetamorphosisPact>(),
            ModelDb.Relic<CreepingVine>(),
            ModelDb.Relic<StableStone>(),
            ModelDb.Relic<GoldenCrucible>(),
            ModelDb.Relic<FishingNet>()
        };
    }

    public override async Task AfterObtained()
    {
        var player = Owner;

        // 1. 计算最大生命值 60% 对应的生命损失
        // 不改变最大生命值
        decimal damage = Math.Ceiling(
            (decimal)player.Creature.MaxHp * 0.6m
        );

        // 2. 失去当前生命值
        await CreatureCmd.Damage(
            new ThrowingPlayerChoiceContext(),
            player.Creature,
            damage,
            ValueProp.Unblockable | ValueProp.Unpowered,
            null,
            null
        );

        // 3. 构建其他海星遗物候选池
        List<RelicModel> candidates = GetStarfishRelics()
            .Where(relic =>
                !player.Relics.Any(owned => owned.Id == relic.Id)
                && relic.IsAllowed(player.RunState))
            .ToList();

        // 4. 使用游戏奖励随机数洗牌
        player.PlayerRng.Rewards.Shuffle(candidates);

        // 5. 随机抽取最多三件不同的遗物
        List<RelicModel> selected = candidates
            .Take(3)
            .ToList();

        if (selected.Count == 0)
            return;

        // 6. 将遗物转换成原版奖励对象
        List<Reward> rewards = selected
            .Select(relic => (Reward)new RelicReward(relic.ToMutable(), player))
            .ToList();

        // 7. 弹出原版奖励界面
        await new RewardsSet(player)
            .WithCustomRewards(rewards)
            .WithSkippingDisallowed()
            .Offer();
    }
}
