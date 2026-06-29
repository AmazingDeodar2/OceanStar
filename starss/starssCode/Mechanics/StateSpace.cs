using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using starss.starssCode.Powers;

namespace starss.starssCode.Mechanics;

public class StateSpace
{
    private readonly List<StateModel> states = new();

    public Player Owner { get; }

    public int Capacity { get; private set; }

    public IReadOnlyList<StateModel> States => states;

    public StateSpace(Player owner, int capacity = 1)
    {
        Owner = owner;
        Capacity = capacity;
    }

    public async Task Enter(PlayerChoiceContext choiceContext, StateModel state)
    {
        StateModel? existing = states.FirstOrDefault(s => s.Id == state.Id);

        if (existing != null)
        {
            StateUi.Refresh(this);
            return;
        }
        
        state.Owner = Owner;

        while (states.Count >= Capacity)
        {
            StateModel oldState = states[0];
            states.RemoveAt(0);
            await oldState.OnExit(choiceContext);
        }

        states.Add(state);
        await state.OnEnter(choiceContext);
        var actorPower = Owner.Creature.GetPower<ProfessionalActorPower>();
        if (actorPower != null)
        {
            await actorPower.AfterStateEntered(choiceContext, state);
        }
        var recruitmentPower = Owner.Creature.GetPower<RecruitmentProcessPower>();
        if (recruitmentPower != null)
        {
            await recruitmentPower.AfterStateEntered(choiceContext, state);
        }
        StateUi.Refresh(this);
    }

    public async Task ExitFirst(PlayerChoiceContext choiceContext)
    {
        if (states.Count <= 0)
            return;

        StateModel state = states[0];
        states.RemoveAt(0);
        await state.OnExit(choiceContext);
        StateUi.Refresh(this);
    }

    public async Task Exit(PlayerChoiceContext choiceContext, StateModel state)
    {
        if (!states.Remove(state))
            return;

        await state.OnExit(choiceContext);
        StateUi.Refresh(this);
    }

    

    public bool Has<T>() where T : StateModel
    {
        return states.Any(s => s is T);
    }

    public T? Get<T>() where T : StateModel
    {
        return states.OfType<T>().FirstOrDefault();
    }

    public void AddCapacity(int amount)
    {
        Capacity += amount;
    }
    
    public async Task AfterCardGeneratedForCombat(
        CardModel card,
        Player? creator)
    {
        foreach (var state in states.ToList())
        {
            await state.AfterCardGeneratedForCombat(card, creator);
        }

        StateUi.Refresh(this);
    }
    
    public int ModifyDiceRoll(Creature creature, CardModel? sourceCard, int roll)
    {
        foreach (var state in states)
        {
            roll = state.ModifyDiceRoll(creature, sourceCard, roll);
        }

        return roll;
    }
    
    public bool ShouldClearBlock(Creature creature)
    {
        foreach (var state in states)
        {
            if (!state.ShouldClearBlock(creature))
                return false;
        }

        return true;
    }
    
    public bool ShouldFlush(Player player)
    {
        foreach (var state in states)
        {
            if (!state.ShouldFlush(player))
                return false;
        }

        return true;
    }
}