using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Models;
using starss.starssCode.Cards;
using starss.starssCode.Cards.Interfaces;
using starss.starssCode.Mechanics;

namespace starss.starssCode.Powers;


public sealed class HatTrickPower : starssPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardPlayed(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        CardModel card = cardPlay.Card;

        if (card.Owner != Owner.Player)
            return;

        if (!IsPcCard(card))
            return;

        Flash();

        for (int i = 0; i < Amount; i++)
        {
            CardModel? generatedCard = GetRandomPcCard();

            if (generatedCard == null)
                continue;

            CardCmd.ApplyKeyword(generatedCard, CardKeyword.Ethereal);
            CardCmd.ApplyKeyword(generatedCard, CardKeyword.Exhaust);

            await CardPileCmd.AddGeneratedCardToCombat(
                generatedCard,
                PileType.Hand,
                Owner.Player
            );
        }
    }

    private static bool IsPcCard(CardModel card)
    {
        return card is IPcCard;
    }

    private CardModel? GetRandomPcCard()
    {
        return PcCardHelper.GetRandomPcCard(Owner.Player);
    }
}
