using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using starss.starssCode.Cards.Interfaces;
using starss.starssCode.Mechanics;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Models;

namespace starss.starssCode.Relics;

public sealed class FoxMask : starssRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains(Owner.Creature) || Owner.PlayerCombatState.TurnNumber > 1)
            return;

        List<CardModel> choices = PcCardHelper.GetPcChoices(Owner, 1);

        if (choices.Count == 0)
            return;

        Flash();

        CardModel card = choices[0];

        card.SetToFreeThisTurn();

        await CardPileCmd.AddGeneratedCardsToCombat(
            choices,
            PileType.Hand,
            Owner
        );
    }
}