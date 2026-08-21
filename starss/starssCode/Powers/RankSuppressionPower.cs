using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Models;

namespace starss.starssCode.Powers;


public sealed class RankSuppressionPower : starssPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;


    public override async Task BeforeApplied(
        Creature target,
        decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        Grow();

        await Task.CompletedTask;
    }


    public override decimal ModifyDamageMultiplicative(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if (dealer != Owner)
            return 1M;

        if (!props.IsPoweredAttack())
            return 1M;

        return 1.3M;
    }


    public override async Task BeforeSideTurnStart(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains(Owner))
            return;


        await PowerCmd.Decrement(this);


        if (Amount <= 0)
        {
            Shrink();

            await PowerCmd.Remove(this);
        }
    }


    private void Grow()
    {
        NCombatRoom.Instance?
            .GetCreatureNode(Owner)?
            .ScaleTo(1.5f, 0.0);
    }


    private void Shrink()
    {
        NCombatRoom.Instance?
            .GetCreatureNode(Owner)?
            .ScaleTo(1.0f, 0.0);
    }
}