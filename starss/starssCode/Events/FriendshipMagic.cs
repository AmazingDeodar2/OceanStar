// using MegaCrit.Sts2.Core.CardSelection;
// using MegaCrit.Sts2.Core.Commands;
// using MegaCrit.Sts2.Core.Entities.Cards;
// using MegaCrit.Sts2.Core.Events;
// using MegaCrit.Sts2.Core.HoverTips;
// using MegaCrit.Sts2.Core.Localization.DynamicVars;
// using System;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using MegaCrit.Sts2.Core.Models;
// using starss.starssCode.Cards;
//
// namespace starss.starssCode.Events;
//
// public sealed class FriendshipMagic : EventModel
// {
//     private const string CardChoiceCountKey = "CardChoiceCount";
//     private const string GoldAmountKey = "GoldAmount";
//
//     private const string LocPrefix = "STARSS-FRIENDSHIP_MAGIC";
//
//     protected override IEnumerable<DynamicVar> CanonicalVars =>
//     [
//         new DynamicVar(CardChoiceCountKey, 2M),
//         new DynamicVar(GoldAmountKey, 77M)
//     ];
//
//     protected override IReadOnlyList<EventOption> GenerateInitialOptions()
//     {
//         return
//         [
//             new EventOption(
//                 this,
//                 ChoosePcCards,
//                 $"{LocPrefix}.pages.INITIAL.options.CHOOSE_CARDS",
//                 Array.Empty<IHoverTip>()
//             ),
//
//             new EventOption(
//                 this,
//                 GainGold,
//                 $"{LocPrefix}.pages.INITIAL.options.GAIN_GOLD",
//                 Array.Empty<IHoverTip>()
//             )
//         ];
//     }
//
//     private async Task ChoosePcCards()
//     {
//         List<CardCreationResult> choices =
//         [
//             new(new TT()),
//             new(new TrojanHorse()),
//             new(new Qiqi()),
//             new(new ThreeW()),
//             new(new SnowCedar()),
//             new(new Nana()),
//             new(new Gratitude())
//         ];
//
//         CardSelectorPrefs prefs = new(
//             L10NLookup(
//                 $"{LocPrefix}.pages.CHOOSE_CARDS.selectionScreenPrompt"
//             ),
//             DynamicVars[CardChoiceCountKey].IntValue
//         )
//         {
//             Cancelable = false
//         };
//
//         await SelectCardsToAddToDeckFromGrid(
//             choices,
//             prefs
//         );
//
//         SetEventFinished(
//             L10NLookup(
//                 $"{LocPrefix}.pages.CHOOSE_CARDS.description"
//             )
//         );
//     }
//
//     private async Task GainGold()
//     {
//         await PlayerCmd.GainGold(
//             DynamicVars[GoldAmountKey].BaseValue,
//             Owner
//         );
//
//         SetEventFinished(
//             L10NLookup(
//                 $"{LocPrefix}.pages.GAIN_GOLD.description"
//             )
//         );
//     }
// }