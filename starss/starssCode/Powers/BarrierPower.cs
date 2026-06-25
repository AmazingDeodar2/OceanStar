using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using starss.starssCode.Mechanics;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Models;

namespace starss.starssCode.Powers;

public sealed class BarrierPower : PowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override decimal ModifyHandDraw(Player player, decimal count)
    {
        return player != Owner.Player ? count : count + Amount;
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player)
            return;

        Flash();

        var check = await DiceHelper.Check(Owner, fate: 40, doom: 60);

        if (check.FateSuccess)
            await PlayerCmd.GainEnergy(1M, Owner.Player);

        if (check.DoomSuccess)
        {
            foreach (CardModel card in await CardSelectCmd.FromHand(
                         choiceContext,
                         player,
                         new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1),
                         null,
                         this))
            {
                await CardCmd.Exhaust(choiceContext, card);
            }
        }
    }
}