using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace starss.starssCode.Powers;

public sealed class MedicinePower : starssPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        // 只在能力持有者结束自己的回合时移除。
        if (!participants.Contains(Owner))
            return;

        await PowerCmd.Remove(this);
    }
}