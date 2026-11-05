
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Models;

namespace starss.starssCode.Relics;


public sealed class TwinSeed : RelicModel
{
    private bool _wasRevived;

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool IsUsedUp => _wasRevived;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(
            1M,
            ValueProp.Unblockable | ValueProp.Unpowered
        ),
        new HealVar(60M)
    ];

    // 保存复苏状态，保证跨战斗保留
    [SavedProperty]
    public bool WasRevived
    {
        get => _wasRevived;
        set
        {
            AssertMutable();
            _wasRevived = value;

            if (_wasRevived)
                Status = RelicStatus.Disabled;
        }
    }

    // =====================================================
    // 第一阶段：每场战斗开始失去1点生命
    // =====================================================

    public override async Task AfterPlayerTurnStart(
        PlayerChoiceContext choiceContext,
        Player player)
    {
        if (player != Owner)
            return;

        // 只在每场战斗的第一个回合触发
        if (Owner.PlayerCombatState.TurnNumber > 1)
            return;

        // 复苏已经触发，第一阶段永久关闭
        if (WasRevived)
            return;

        Flash();

        await CreatureCmd.Damage(
            choiceContext,
            Owner.Creature,
            DynamicVars.Damage,
            (Creature)null,
            (CardModel)null,
            (CardPlay)null
        );
    }

    // =====================================================
    // 第二阶段：濒死复苏
    // =====================================================

    public override bool ShouldDieLate(Creature creature)
    {
        // 只保护遗物持有者，且只能触发一次
        return creature != Owner.Creature || WasRevived;
    }

    public override async Task AfterPreventingDeath(
        Creature creature)
    {
        if (creature != Owner.Creature || WasRevived)
            return;

        Flash();

        // 标记复苏已经使用
        // 同时永久关闭第一阶段
        WasRevived = true;

        // 恢复最大生命值的60%
        decimal healAmount = Math.Max(
            1M,
            (decimal)creature.MaxHp *
            (DynamicVars["Heal"].BaseValue / 100M)
        );

        await CreatureCmd.Heal(
            creature,
            healAmount
        );
    }
}
