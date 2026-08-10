using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Models;

namespace starss.starssCode.Powers;


public sealed class NaturalHistoryPower : starssPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;


    public override decimal ModifyDamageMultiplicative(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        // 只影响攻击伤害
        if (!props.IsPoweredAttack())
            return 1M;


        // 只影响自己造成的伤害
        if (dealer != Owner)
            return 1M;


        if (target == null)
            return 1M;


        int debuffKinds = target.Powers
            .Where(power =>
                power.Type == PowerType.Debuff)
            .Select(power =>
                power.GetType())
            .Distinct()
            .Count();


        // 至少两种不同负面状态
        if (debuffKinds < 2)
            return 1M;


        return 1M + Amount / 100M;
    }


    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner))
            return;


        await PowerCmd.Remove(this);
    }
}