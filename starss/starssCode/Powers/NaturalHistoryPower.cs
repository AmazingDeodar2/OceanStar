using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Models;

namespace starss.starssCode.Powers;


public sealed class NaturalHistoryPower : starssPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override decimal ModifyDamageMultiplicative(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        // 只增强攻击伤害
        if (!props.IsPoweredAttack())
            return 1M;

        // 必须是自己（或自己的宠物）造成的伤害
        if (dealer != Owner && !Owner.Pets.Contains(dealer))
            return 1M;

        if (target == null)
            return 1M;

        int debuffKinds = target.Powers
            .Where(p =>
                p.IsVisible &&
                p.Type == PowerType.Debuff)
            .Select(p => p.GetType())
            .Distinct()
            .Count();

        return debuffKinds >= 2 ? 1.5M : 1M;
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