using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models.Powers;
using starss.starssCode.Mechanics;

namespace starss.starssCode.Powers;


public sealed class EvilSmilePower : starssPower
{
    private int _statesEntered;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task BeforeFlushLate(
        PlayerChoiceContext choiceContext,
        Player player)
    {
        // 只在能力拥有者自己的回合结束时触发
        if (player != Owner.Player)
            return;

        // 与官方 WellLaidPlansPower 一致，确认本次确实会进行回合末清手
        if (!Hook.ShouldFlush(player.Creature.CombatState, player))
            return;

        int enteredStateCount =
            StateRegistry.Get(player).EnteredStateCount;

        if (enteredStateCount <= 0)
            return;

        decimal totalAmount =
            enteredStateCount * Amount;

        Flash();

        await PowerCmd.Apply<DoomPower>(
            choiceContext,
            CombatState.HittableEnemies,
            totalAmount,
            Owner,
            null
        );
    }
}