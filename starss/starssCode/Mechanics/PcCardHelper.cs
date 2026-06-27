using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Models;
using starss.starssCode.Cards;
using starss.starssCode.Cards.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace starss.starssCode.Mechanics;

public static class PcCardHelper
{
    public static IEnumerable<CardModel> GetPcCardPool(Player player)
    {
        return player.Character.CardPool
            .GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint)
            .Where(c => c is IPcCard);
    }

    public static List<CardModel> GetPcChoices(Player player, int count)
    {
        return CardFactory
            .GetDistinctForCombat(
                player,
                GetPcCardPool(player),
                count,
                player.RunState.Rng.CombatCardGeneration
            )
            .ToList();
    }
}