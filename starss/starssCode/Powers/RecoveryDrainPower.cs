using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace starss.starssCode.Powers;


/// <summary>
/// 「新生」的**代价**部分（Debuff）：回合开始失去幸运。
///
/// 实现上不需要任何 DynamicVar：卡牌每次打出时以「本卡要失去的幸运」为 amount 施加，
/// Counter 叠层天然就是求和——先打普通版（+3）再打升级版（+2）⇒ Amount = 5，
/// 每回合失去 5 点幸运。这比“拷值进 Var 再累加”少一套状态，也更直观（图标上的数字就是总量）。
/// </summary>
public sealed class RecoveryDrainPower : starssPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromPower<LuckyPower>()
        ];

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        // 只在这个能力拥有者自己的回合开始时触发。
        if (player != Owner.Player) return;

        Flash();

        await PowerCmd.Apply<LuckyPower>(
            choiceContext,
            Owner,
            -Amount,
            Owner,
            (CardModel?)null
        );
    }
}
