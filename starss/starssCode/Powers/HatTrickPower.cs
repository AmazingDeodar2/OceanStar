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

namespace starss.starssCode.Powers;


public sealed class HatTrickPower : PowerModel
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

            if (generatedCard != null)
            {
                await CardPileCmd.AddGeneratedCardToCombat(
                    generatedCard,
                    PileType.Hand,
                    Owner.Player
                );
            }
        }
    }

    private static bool IsPcCard(CardModel card)
    {
        return card is TrojanHorse || card is SnowCedar || card is ThreeW || card is Nana || card is Qiqi || card is TT;
    }

    private CardModel? GetRandomPcCard()
    {
        return CardFactory
            .GetDistinctForCombat(
                Owner.Player,
                Owner.Player.Character.CardPool
                    .GetUnlockedCards(
                        Owner.Player.UnlockState,
                        Owner.Player.RunState.CardMultiplayerConstraint)
                    .Where(card => card is IPcCard),
                1,
                Owner.Player.RunState.Rng.CombatCardGeneration
            )
            .FirstOrDefault();
    }
}