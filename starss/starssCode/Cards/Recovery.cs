using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using starss.starssCode.Powers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace starss.starssCode.Cards;


public sealed class Recovery : starssCard
{
    public Recovery()
        : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        // 打出的「新生」层数。每层 = 回合开始 2 格挡 + 1 活力。
        // 未升级 2 层 ⇒ 4/2；升级后 3 层 ⇒ 6/3；两张都打 = 5 层 ⇒ 10/5。
        new DynamicVar("Layers", 2M),
        // 每回合失去的幸运（交给 Debuff，Counter 直接求和：先普通 +3 再升级 +2 ⇒ 5）。
        new DynamicVar("LuckLoss", 3M),
        // 下面两个**只用于卡面文案**，显示本卡带来的总量 = 层数 × (2 格挡 / 1 活力)。
        // 改层数时记得同步改这两个（BlockGain = Layers × 2，VigorGain = Layers × 1）。
        new DynamicVar("BlockGain", 4M),
        new DynamicVar("VigorGain", 2M)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<RecoveryDrainPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars["LuckLoss"].BaseValue,
            Owner.Creature,
            this
        );

        RecoveryPower? buff = await PowerCmd.Apply<RecoveryPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars["Layers"].BaseValue,
            Owner.Creature,
            this
        );

        // 层数叠完之后，把 buff 上「显示用的总量 Var」同步成 每层值 × 层数。
        // 不能靠 Power 的 AfterApplied：叠层走 ModifyAmount，那个回调不会触发。
        buff?.SyncVarsToAmount();
    }

    protected override void OnUpgrade()
    {
        // 层数 2→3（格挡 4→6、活力 2→3），失去的幸运 3→2。
        DynamicVars["Layers"].UpgradeValueBy(1M);
        DynamicVars["LuckLoss"].UpgradeValueBy(-1M);
        DynamicVars["BlockGain"].UpgradeValueBy(2M);
        DynamicVars["VigorGain"].UpgradeValueBy(1M);
    }
}