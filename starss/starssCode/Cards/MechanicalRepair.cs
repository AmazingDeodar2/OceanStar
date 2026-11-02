
using System;
using System.Linq;
using System.Threading.Tasks;

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;

namespace starss.starssCode.Cards;

public sealed class MechanicalRepair : starssCard
{
    public MechanicalRepair()
        : base(
            4,
            CardType.Power,
            CardRarity.Rare,
            TargetType.Self)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        // 1. 抽一张牌
        await CardPileCmd.Draw(
            choiceContext,
            1,
            Owner
        );

        // 2. 获取手牌、抽牌堆、弃牌堆中所有未附魔的牌
        var cards = CardPile.GetCards(
                Owner,
                PileType.Hand,
                PileType.Draw,
                PileType.Discard
            )
            .Where(card => card.Enchantment == null)
            .ToList();

        // 3. 为每张符合条件的牌随机附魔
        foreach (var card in cards)
        {
            EnchantmentModel? enchantment =
                GetRandomEnchantment(card);

            if (enchantment == null)
                continue;

            CardCmd.Enchant(
                enchantment.ToMutable(),
                card,
                GetEnchantAmount(enchantment)
            );

            CardCmd.Preview(card);
        }
    }

    // 随机获取一个合法附魔
    // 排除：墨影（Inky）、弃用
    private EnchantmentModel? GetRandomEnchantment(
        CardModel card)
    {
        var enchantments = ModelDb.DebugEnchantments
            .Where(e =>
                e.CanEnchant(card) &&
                !IsExcludedEnchantment(e))
            .ToList();

        if (enchantments.Count == 0)
            return null;

        int index = Owner.RunState.Rng.CombatCardGeneration
            .NextInt(enchantments.Count);

        return enchantments[index];
    }

    // 判断是否属于需要排除的附魔
    private static bool IsExcludedEnchantment(
        EnchantmentModel enchantment)
    {
        // 排除墨影（Inky）
        if (string.Equals(
            enchantment.Id.Entry,
            "INKY",
            StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        // 临时排除中文名称为「弃用」的附魔
        // 注意：需要当前语言为中文
        string title = enchantment.Title.GetFormattedText();

        if (title.Contains("弃用",
            StringComparison.Ordinal))
        {
            return true;
        }

        return false;
    }

    // 附魔默认数值
    private decimal GetEnchantAmount(
        EnchantmentModel enchantment)
    {
        return 1M;
    }

    // 升级后获得保留
    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}
