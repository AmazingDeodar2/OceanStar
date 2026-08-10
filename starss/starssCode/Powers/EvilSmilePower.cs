// using System.Threading.Tasks;
// using MegaCrit.Sts2.Core.Commands;
// using MegaCrit.Sts2.Core.Entities.Players;
// using MegaCrit.Sts2.Core.Entities.Powers;
// using MegaCrit.Sts2.Core.GameActions.Multiplayer;
// using MegaCrit.Sts2.Core.Hooks;
// using MegaCrit.Sts2.Core.Models.Powers;
// using starss.starssCode.Mechanics;
//
// namespace starss.starssCode.Powers;
//
// public sealed class EvilSmilePower : starssPower
// {
//     private int _statesEnteredThisCombat;
//
//     public override PowerType Type => PowerType.Buff;
//
//     public override PowerStackType StackType =>
//         PowerStackType.Counter;
//
//     public void AfterStateEntered(StateModel state)
//     {
//         _statesEnteredThisCombat++;
//     }
//
//     public override async Task BeforeFlushLate(
//         PlayerChoiceContext choiceContext,
//         Player player)
//     {
//         // 只在能力拥有者自己的回合结束时触发
//         if (player != Owner.Player)
//             return;
//
//         if (!Hook.ShouldFlush(
//                 player.Creature.CombatState,
//                 player))
//         {
//             return;
//         }
//
//         if (_statesEnteredThisCombat <= 0)
//             return;
//
//         decimal totalAmount =
//             _statesEnteredThisCombat * Amount;
//
//         Flash();
//
//         await PowerCmd.Apply<DoomPower>(
//             choiceContext,
//             CombatState.HittableEnemies,
//             totalAmount,
//             Owner,
//             null
//         );
//
//         // 不清零，继续累计整场战斗进入的状态数
//     }
// }