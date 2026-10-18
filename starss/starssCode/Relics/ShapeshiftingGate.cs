
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Runs;

namespace starss.starssCode.Relics;

public sealed class ShapeshiftingGate : RelicModel
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool HasUponPickupEffect => true;

    // 每回合正常抽牌数量 +1，包括第一回合
    public override decimal ModifyHandDraw(Player player, decimal count)
    {
        return player == Owner ? count + 1m : count;
    }

    public override async Task AfterObtained()
    {
        Player player = Owner;

        // 获取全部已注册的可玩角色，包括 Mod 角色
        List<CharacterModel> candidates = ModelDb.All
            .OfType<CharacterModel>()
            .Where(character =>
                character.IsPlayable &&
                character.Id != player.Character.Id)
            .ToList();

        if (candidates.Count == 0)
            return;

        // 使用游戏内随机数，避免额外创建 System.Random
        int index = player.RunState.Rng.Shuffle.NextInt(candidates.Count);
        CharacterModel targetCharacter = candidates[index];

        // 先取得双方的初始牌组快照
        List<CardModel> originalStartingDeck =
            player.Character.StartingDeck.ToList();

        List<CardModel> targetStartingDeck =
            targetCharacter.StartingDeck.ToList();

        // 防止选中的角色没有初始牌组
        if (targetStartingDeck.Count == 0)
            return;

        // 按卡牌实际类型统计原始初始牌组的数量
        Dictionary<Type, int> remaining = originalStartingDeck
            .GroupBy(card => card.GetType())
            .ToDictionary(group => group.Key, group => group.Count());

        // 先保存牌组快照，避免遍历过程中修改集合
        List<CardModel> currentDeck = player.Deck.Cards.ToList();
        List<CardModel> cardsToRemove = new();

        foreach (CardModel card in currentDeck)
        {
            Type cardType = card.GetType();

            if (!remaining.TryGetValue(cardType, out int count))
                continue;

            if (count <= 0)
                continue;

            cardsToRemove.Add(card);
            remaining[cardType] = count - 1;
        }

        // 删除仍保留的原始初始卡牌
        if (cardsToRemove.Count > 0)
        {
            await CardPileCmd.RemoveFromDeck(
                cardsToRemove,
                showPreview: false
            );
        }

        // 创建并注册目标角色的完整初始牌组
        List<CardModel> newCards = new();

        foreach (CardModel canonicalCard in targetStartingDeck)
        {
            CardModel newCard =
                player.RunState.CreateCard(canonicalCard, player);

            newCards.Add(newCard);
        }

        // 永久加入玩家牌组
        await CardPileCmd.Add(
            newCards,
            PileType.Deck,
            skipVisuals: true
        );

        Flash();
    }
}
