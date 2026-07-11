using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Models;
using starss.starssCode.Cards;

namespace starss.starssCode.Potions;

public sealed class LalangPotion : starssPotion
{
    public override PotionRarity Rarity => PotionRarity.Rare;

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
        var combatState = Owner.Creature.CombatState;

        List<CardModel> choices =
        [
            combatState.CreateCard<TT>(Owner),
            combatState.CreateCard<TrojanHorse>(Owner),
            combatState.CreateCard<Qiqi>(Owner),
            combatState.CreateCard<ThreeW>(Owner),
            combatState.CreateCard<SnowCedar>(Owner),
            combatState.CreateCard<Nana>(Owner),
            combatState.CreateCard<Gratitude>(Owner)
        ];
        
        CardModel? selectedCard =
            await CardSelectCmd.FromChooseACardScreen(
                choiceContext,
                choices.Take(3).ToList(),
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