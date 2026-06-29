using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using starss.starssCode.Cards;
using starss.starssCode.Character;

namespace starss.starssCode.Patches;

public static class ArchaicToothStarssPatch
{
    // 这里改成你希望被升华的那张“初始牌”
    private static CardModel StarterCard => ModelDb.Card<RollDice>();

    // 这里是升华后的先古牌
    private static CardModel AncientCard => ModelDb.Card<WheelOfFate>();

    [HarmonyPatch(typeof(ArchaicTooth), "get_TranscendenceCards")]
    public static class TranscendenceCardsPatch
    {
        public static void Postfix(ref List<CardModel> __result)
        {
            if (__result.All(c => c.Id != AncientCard.Id))
                __result.Add(AncientCard);
        }
    }

    [HarmonyPatch(typeof(ArchaicTooth), "GetTranscendenceStarterCard")]
    public static class GetTranscendenceStarterCardPatch
    {
        public static bool Prefix(Player player, ref CardModel? __result)
        {
            if (player.Character.Id.Entry != Starss.CharacterId)
                return true;

            __result = player.Deck.Cards
                .FirstOrDefault(c => c.Id == StarterCard.Id);

            return false;
        }
    }

    [HarmonyPatch(typeof(ArchaicTooth), "GetTranscendenceTransformedCard")]
    public static class GetTranscendenceTransformedCardPatch
    {
        public static bool Prefix(CardModel starterCard, ref CardModel __result)
        {
            if (starterCard.Id != StarterCard.Id)
                return true;

            CardModel card = starterCard.Owner.RunState.CreateCard(AncientCard, starterCard.Owner);

            if (starterCard.IsUpgraded)
                CardCmd.Upgrade(card);

            if (starterCard.Enchantment != null)
            {
                var enchantment = (EnchantmentModel)starterCard.Enchantment.MutableClone();
                CardCmd.Enchant(enchantment, card, enchantment.Amount);
            }

            __result = card;
            return false;
        }
    }
}