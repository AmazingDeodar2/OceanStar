using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Models;

namespace starss.starssCode.Powers;


public sealed class AbyssPower : starssPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardDrawn(
        PlayerChoiceContext choiceContext,
        CardModel card,
        bool fromHandDraw)
    {
        if (card.Owner != Owner.Player)
            return;

        if (card is MegaCrit.Sts2.Core.Models.Cards.Void || card is Beckon)
        {
            Flash();

            await CreatureCmd.Heal(
                Owner,
                Amount
            );
        }
    }
    public decimal AbyssCount { get; private set; }

    public void AddAbyssCount(decimal amount)
    {
        AbyssCount += amount;
        Flash();
    }
    private static bool IsCallCard(CardModel card)
    {
        return card.GetType().Name.Contains("Call");
    }
}