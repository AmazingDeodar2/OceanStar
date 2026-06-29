using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using starss.starssCode.Mechanics;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Models;

namespace starss.starssCode.States;

public sealed class RatCreviceState : StateModel
{
    private readonly LocString selectionPrompt;

    public override string Id => "starss:rat_crevice";

    public override string DisplayName => "老鼠狭缝";
    
    public RatCreviceState()
    {
        Duration = int.MaxValue;
        selectionPrompt = null;
    }

    public RatCreviceState(LocString selectionPrompt)
    {
        Duration = int.MaxValue;
        this.selectionPrompt = selectionPrompt;
    }

    public override async Task OnEnter(PlayerChoiceContext choiceContext)
    {
        CardSelectorPrefs prefs = new CardSelectorPrefs(selectionPrompt, 1);

        CardModel card = (await CardSelectCmd.FromCombatPile(
            choiceContext,
            PileType.Draw.GetPile(Owner),
            Owner,
            prefs
        )).FirstOrDefault();

        if (card == null)
            return;

        await CardPileCmd.Add(card, PileType.Hand);
    }

    public override async Task OnExit(PlayerChoiceContext choiceContext)
    {
        CardSelectorPrefs prefs = new CardSelectorPrefs(selectionPrompt, 1);

        CardModel card = (await CardSelectCmd.FromCombatPile(
            choiceContext,
            PileType.Discard.GetPile(Owner),
            Owner,
            prefs
        )).FirstOrDefault();

        if (card == null)
            return;

        await CardPileCmd.Add(card, PileType.Hand);
    }
}