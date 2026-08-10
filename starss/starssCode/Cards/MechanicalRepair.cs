using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;
using System.Linq;
using System.Threading.Tasks;


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
        // 抽一张牌
        await CardPileCmd.Draw(
            choiceContext,
            1,
            Owner
        );


        var cards = CardPile.GetCards(
                Owner,
                PileType.Hand,
                PileType.Draw,
                PileType.Discard
            )
            .Where(card =>
                card.Enchantment == null)
            .ToList();


        foreach (var card in cards)
        {
            var enchantment =
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


    private EnchantmentModel? GetRandomEnchantment(
        CardModel card)
    {
        var enchantments =
            ModelDb.DebugEnchantments
                .Where(e => e.CanEnchant(card))
                .ToList();


        if (enchantments.Count == 0)
            return null;


        int index =
            Owner.RunState.Rng.CombatCardGeneration
                .NextInt(enchantments.Count);


        return enchantments[index];
    }


    private decimal GetEnchantAmount(
        EnchantmentModel enchantment)
    {
        // 默认附魔数值
        // 后续如果有特殊附魔可以单独处理

        return 1M;
    }


    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}