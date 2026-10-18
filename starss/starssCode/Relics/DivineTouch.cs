using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Relics;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Models;

// 改成你“神之骰”卡牌实际所在的命名空间
using starss.starssCode.Cards;

namespace starss.starssCode.Relics;


public sealed class DivineTouch : RelicModel
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext choiceContext,
        ICombatState combatState)
    {
        // 只在持有者的第一回合触发
        if (player != Owner ||
            Owner.PlayerCombatState.TurnNumber != 1)
        {
            return;
        }

        // 创建一张“神之骰”
        var card = Owner.Creature.CombatState
            .CreateCard<GodDice>(Owner);

        // 将卡牌加入手牌
        await CardPileCmd.AddGeneratedCardsToCombat(
            new List<CardModel> { card },
            PileType.Hand,
            Owner
        );
    }
}