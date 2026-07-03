using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Models;
using starss.starssCode.Mechanics;

namespace starss.starssCode.Cards;


public sealed class SanCheck : starssCard
{
    public SanCheck()
        : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(3),
        new HpLossVar(1M),
        new FateVar(50M),
        new DoomVar(50M)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(
            choiceContext,
            DynamicVars.Cards.BaseValue,
            Owner
        );

        await CreatureCmd.Damage(
            choiceContext,
            Owner.Creature,
            DynamicVars.HpLoss.BaseValue,
            ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move,
            this,
            cardPlay
        );

        var check = await DiceHelper.Check(
            Owner.Creature,
            fate: DynamicVars["Fate"].IntValue,
            doom: DynamicVars["Doom"].IntValue,
            choiceContext: choiceContext,
            sourceCard: this
        );

        if (check.FateSuccess)
        {
           

            CardModel card = await CardSelectCmd.FromChooseACardScreen(
                choiceContext,
                CardFactory.GetDistinctForCombat(
                    Owner,
                    Owner.Character.CardPool.GetUnlockedCards(
                        Owner.UnlockState,
                        Owner.RunState.CardMultiplayerConstraint
                    ).Where(c => c.Type == CardType.Skill),
                    3,
                    Owner.RunState.Rng.CombatCardGeneration
                ).ToList(),
                Owner,
                true
            );

            if (card != null)
            {
                card.SetToFreeThisTurn();

                await CardPileCmd.AddGeneratedCardToCombat(
                    card,
                    PileType.Hand,
                    Owner
                );
            }
            
        }

        if (check.DoomSuccess)
        {
           
            CardSelectorPrefs prefs = new CardSelectorPrefs(CardSelectorPrefs.EnchantSelectionPrompt, 1);

            CardModel? card = (await CardSelectCmd.FromHand(
                    choiceContext,
                    Owner,
                    prefs,
                    c => c.CostsEnergyOrStars(false) || c.CostsEnergyOrStars(true),
                    this))
                .FirstOrDefault();

            card?.SetToFreeThisCombat();
        }
    }

    protected override void OnUpgrade() => this.AddKeyword(CardKeyword.Innate);
}