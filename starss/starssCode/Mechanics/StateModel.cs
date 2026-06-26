using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace starss.starssCode.Mechanics;

public abstract class StateModel
{
    public Player Owner { get; internal set; } = null!;

    public abstract string Id { get; }

    public virtual int Duration { get; protected set; }

    public virtual Task OnEnter(PlayerChoiceContext choiceContext)
    {
        return Task.CompletedTask;
    }
    public virtual Task OnActive(PlayerChoiceContext choiceContext) => Task.CompletedTask;

    public virtual Task OnExit(PlayerChoiceContext choiceContext)
    {
        return Task.CompletedTask;
    }
    public virtual int ModifyDiceRoll(Creature creature, CardModel? sourceCard, int roll)
    {
        return roll;
    }
    
    public virtual Task AfterCardGeneratedForCombat(
        CardModel card,
        Player? creator)
    {
        return Task.CompletedTask;
    }
    
    public abstract string DisplayName { get; }
    public bool IsExpired => Duration <= 0;
}