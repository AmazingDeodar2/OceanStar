using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using starss.starssCode.Mechanics;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using VoidCard = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace starss.starssCode.Cards;

 
public sealed class PullYourCable : starssCard
{
    public PullYourCable()
        : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(7),
        new DoomVar(51M)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(
            Owner.Creature,
            "Cast",
            Owner.Character.CastAnimDelay
        );

        await CardPileCmd.Draw(
            choiceContext,
            DynamicVars.Cards.BaseValue,
            Owner
        );

        CardSelectorPrefs prefs = new CardSelectorPrefs(
            CardSelectorPrefs.ExhaustSelectionPrompt,
            1
        );

        CardModel card = (await CardSelectCmd.FromHand(
            choiceContext,
            Owner,
            prefs,
            null,
            this
        )).FirstOrDefault();

        if (card != null)
            await CardCmd.Exhaust(choiceContext, card);

        var result = await DiceHelper.Check(
            Owner.Creature,
            fate: 101,
            doom: DynamicVars["Doom"].IntValue,
            choiceContext: choiceContext,
            sourceCard: this
        );

        if (result.DoomSuccess)
        {
           
            CardModel voidCard = Owner.Creature.CombatState.CreateCard<VoidCard>(Owner);

            await CardPileCmd.AddGeneratedCardToCombat(
                voidCard,
                PileType.Discard,
                Owner
            );
            PileType.Discard.GetPile(Owner).InvokeCardAddFinished();
        }
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}