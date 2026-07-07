using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using System.Threading.Tasks;

namespace starss.starssCode.Powers;


public sealed class WhiteFormPower : starssPower
{
    private int _cardsPlayed;
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardPlayed(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner)
            return;

        _cardsPlayed++;

        if (_cardsPlayed < 2)
            return;

        _cardsPlayed = 0;

        Flash();

        await CardPileCmd.Draw(
            choiceContext,
            Amount,
            Owner.Player
        );
    }
}