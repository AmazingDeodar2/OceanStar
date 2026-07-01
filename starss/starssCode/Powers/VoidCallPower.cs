using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using starss.starssCode.Cards.Interfaces;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace starss.starssCode.Powers;


public sealed class VoidCallPower : starssPower
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

            await CardPileCmd.Draw(choiceContext, 1M * Amount, Owner.Player, false);

            Owner.Player.PlayerCombatState.GainEnergy(1M * Amount);
        }
    }
}