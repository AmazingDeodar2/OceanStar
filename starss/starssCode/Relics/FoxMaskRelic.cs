using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using starss.starssCode.Cards.Interfaces;
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

        List<CardModel> choices = CardFactory
            .GetDistinctForCombat(
                Owner,
                Owner.Character.CardPool
                    .GetUnlockedCards(
                        Owner.UnlockState,
                        Owner.RunState.CardMultiplayerConstraint
                    )
                    .Where(c => c is IPcCard),
                1,
                Owner.RunState.Rng.CombatCardGeneration
            )
            .ToList();

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