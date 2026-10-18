
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace starss.starssCode.Relics;


public sealed class StableStone : RelicModel
{
    private int _remainingRewards = 3;

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool HasUponPickupEffect => true;

    public override bool IsUsedUp => RemainingRewards <= 0;

    // 显示稳定附魔的悬浮提示
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        HoverTipFactory.FromEnchantment<Steady>();

    // 保存剩余触发次数
    [SavedProperty]
    private int RemainingRewards
    {
        get => _remainingRewards;

        set
        {
            AssertMutable();

            _remainingRewards = value;

            if (IsUsedUp)
            {
                Status = RelicStatus.Disabled;
            }
        }
    }

    // 获得时失去所有金币
    public override async Task AfterObtained()
    {
        await PlayerCmd.LoseGold(
            (decimal)Owner.Gold,
            Owner
        );
    }

    // 为接下来的三次卡牌奖励附加稳定
    public override bool TryModifyCardRewardOptionsLate(
        Player player,
        List<CardCreationResult> cardRewards,
        CardCreationOptions options)
    {
        if (player != Owner ||
            !options.Flags.HasFlag(CardCreationFlags.IsCardReward) ||
            IsUsedUp)
        {
            return false;
        }

        Steady steady = ModelDb.Enchantment<Steady>();

        foreach (CardCreationResult cardReward in cardRewards)
        {
            CardModel originalCard = cardReward.Card;

            if (!steady.CanEnchant(originalCard))
            {
                continue;
            }

            CardModel modifiedCard =
                Owner.RunState.CloneCard(originalCard);

            CardCmd.Enchant<Steady>(
                modifiedCard,
                1m
            );

            cardReward.ModifyCard(
                modifiedCard,
                this
            );
        }

        return true;
    }

    // 每生成一次卡牌奖励，消耗一次使用次数
    public override Task AfterModifyingCardRewardOptions()
    {
        if (IsUsedUp)
        {
            return Task.CompletedTask;
        }

        RemainingRewards--;

        return Task.CompletedTask;
    }
}
