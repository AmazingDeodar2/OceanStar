using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace starss.starssCode.Mechanics;

public static class StateCmd
{
    public static async Task Enter(
        PlayerChoiceContext choiceContext,
        Player player,
        StateModel state)
    {
        if (CombatManager.Instance.IsOverOrEnding)
            return;

        StateSpace space = StateRegistry.Get(player);
        await space.Enter(choiceContext, state);
        await PineappleModelHelper.OnEnterState(choiceContext, player);
    }

    public static async Task AfterCardGeneratedForCombat(
        CardModel card,
        Player? creator)
    {
        if (creator == null)
            return;

        await StateRegistry
            .Get(creator)
            .AfterCardGeneratedForCombat( card, creator);

        await SevenSevenSevenHelper.AfterCardGeneratedForCombat(
            card,
            creator
        );
        
        bool isVoidOrCall =
            card is MegaCrit.Sts2.Core.Models.Cards.Void
            || card is MegaCrit.Sts2.Core.Models.Cards.Beckon;

        if (isVoidOrCall)
        {
            GlobalVoidCallCounter.Increment(creator);
        }
    }

    public static async Task ExitFirst(
        PlayerChoiceContext choiceContext,
        Player player)
    {
        if (CombatManager.Instance.IsOverOrEnding)
            return;

        StateSpace space = StateRegistry.Get(player);
        await space.ExitFirst(choiceContext);
    }
    
    public static void AddCapacity(Player player, int amount)
    {
        StateRegistry.Get(player).AddCapacity(amount);
    }
    
    public static int ModifyDiceRoll(Creature creature, CardModel? sourceCard, int roll)
    {
        var player = creature.Player;
        if (player == null)
            return roll;

        return StateRegistry.Get(player).ModifyDiceRoll(creature, sourceCard, roll);
    }
}