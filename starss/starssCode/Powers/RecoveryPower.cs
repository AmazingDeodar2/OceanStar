using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace starss.starssCode.Powers;


/// <summary>
/// 「新生」的**收益**部分（Buff）：回合开始获得格挡与活力。
///
/// 模型：**每层**（每 1 点 Amount）回合计 2 格挡 + 1 活力。
/// 卡牌按等级给层数——未升级「新生」2 层 ⇒ 4/2，升级后 3 层 ⇒ 6/3，
/// 两张都打出 = 5 层 ⇒ 10/5。层数交给 Counter 叠层，和 <see cref="RecoveryDrainPower"/> 同一套路。
///
/// ⚠️ 坑（2026-11-02 实测）：**不要**在 <c>AfterApplied</c> 里维护这类数值。
/// <c>PowerCmd.Apply&lt;T&gt;</c> 在能力**已存在**时走的是 <c>ModifyAmount</c> 分支
/// （PowerCmd.cs:82-91），那条分支**根本不调 AfterApplied**——
/// 也就是 AfterApplied 只在首次施加时触发一次，叠层时你的累加逻辑压根不会跑。
/// 表现就是「图标显示 2 层，实际效果还是第一张牌的数值」。
/// 所以这里所有数值一律由 Amount 现场推算，显示用的 Var 由卡牌在 OnPlay 末尾手工同步。
/// </summary>
public sealed class RecoveryPower : starssPower
{
    /// <summary>每层每回合获得的格挡。</summary>
    private const decimal BlockPerStack = 2M;

    /// <summary>每层每回合获得的活力。</summary>
    private const decimal VigorPerStack = 1M;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromPower<VigorPower>(),
            HoverTipFactory.Static(StaticHoverTip.Block)
        ];

    /// <summary>
    /// 这两个 Var **只用于显示**（卡面/buff 悬停文案里的总量）。
    /// 这里给的是「每层」的值；真正挂到身上后由 <see cref="SyncVarsToAmount"/> 改成总量。
    /// 实际结算一律用 <c>常量 × Amount</c>，不读这两个 Var，避免两处不一致。
    /// </summary>
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("BlockGain", BlockPerStack),
        new DynamicVar("VigorGain", VigorPerStack)
    ];

    /// <summary>
    /// 把显示用的两个 Var 同步成「总量 = 每层值 × 当前层数」。
    /// 必须由**卡牌**在 <c>OnPlay</c> 里、<c>PowerCmd.Apply</c> 之后显式调用：
    /// 叠层走 ModifyAmount，AfterApplied 不触发，Power 自己没有机会同步（见类注释）。
    /// </summary>
    public void SyncVarsToAmount()
    {
        DynamicVars["BlockGain"].BaseValue = BlockPerStack * Amount;
        DynamicVars["VigorGain"].BaseValue = VigorPerStack * Amount;
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        // 只在这个能力拥有者自己的回合开始时触发。
        if (player != Owner.Player) return;

        Flash();

        await CreatureCmd.GainBlock(
            Owner,
            BlockPerStack * Amount,
            ValueProp.Unpowered,
            (CardPlay?)null
        );

        await PowerCmd.Apply<VigorPower>(
            choiceContext,
            Owner,
            VigorPerStack * Amount,
            Owner,
            (CardModel?)null
        );
    }
}
