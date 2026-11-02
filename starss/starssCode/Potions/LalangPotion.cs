using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Models;
using starss.starssCode.Cards;
using starss.starssCode.Mechanics;

namespace starss.starssCode.Potions;

public sealed class LalangPotion : starssPotion
{
    public override PotionRarity Rarity => PotionRarity.Uncommon;

    public override PotionUsage Usage => PotionUsage.CombatOnly;

    public override TargetType TargetType => TargetType.Self;

    public override List<(string, string)>? Localization =>
    [
        ("title", "拉郎药水"),
        ("description", "从三张PC牌中选择1张加入手牌。本回合你可以免费打出它。"),
        ("flavor", "想要什么，自己选吧！")
    ];

    protected override async Task OnUse(
        PlayerChoiceContext choiceContext,
        Creature? target)
    {
        // 走 PC 牌池而不是硬编码卡牌清单：以后新增/移除 PC 牌会自动同步，且尊重解锁状态。
        List<CardModel> choices = PcCardHelper.GetPcChoices(Owner, 3);

        if (choices.Count == 0)
            return;

        CardModel? selectedCard =
            await CardSelectCmd.FromChooseACardScreen(
                choiceContext,
                choices,
                Owner,
                true
            );
        
        if (selectedCard == null)
            return;

        selectedCard.SetToFreeThisTurn();

        await CardPileCmd.AddGeneratedCardToCombat(
            selectedCard,
            PileType.Hand,
            Owner
        );
    }
}