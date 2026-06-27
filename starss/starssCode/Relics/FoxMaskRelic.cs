using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Rooms;
using starss.starssCode.Cards.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Models;

namespace starss.starssCode.Relics;


public sealed class FoxMask : starssRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is not CombatRoom)
            return;

        Flash();

        List<CardModel> choices = CardFactory
            .GetDistinctForCombat(
                Owner,
                Owner.Character.CardPool
                    .GetUnlockedCards(
                        Owner.UnlockState,
                        Owner.RunState.CardMultiplayerConstraint
                    )
                    .Where(c => c is IPcCard),
                3,
                Owner.RunState.Rng.CombatCardGeneration
            )
            .ToList();

        if (choices.Count == 0)
            return;

        CardModel card = await CardSelectCmd.FromChooseACardScreen(
            new ThrowingPlayerChoiceContext(),
            choices,
            Owner,
            true
        );

        if (card == null)
            return;

        card.SetToFreeThisTurn();

        await CardPileCmd.AddGeneratedCardToCombat(
            card,
            PileType.Hand,
            Owner
        );
    }
}