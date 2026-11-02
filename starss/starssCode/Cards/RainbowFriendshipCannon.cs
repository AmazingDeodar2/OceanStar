using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using starss.starssCode.Mechanics;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace starss.starssCode.Cards;


public sealed class RainbowFriendshipCannon : starssCard
{
    private const string CalculatedHitsKey = "CalculatedHits";
    
    public RainbowFriendshipCannon()
        : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        // 每一跳的伤害（升级不再加伤）
        new DamageVar(7M, ValueProp.Move),

        // 0 基：没有「白送的一次」——0 个状态就是 0 跳，什么都不打。
        new CalculationBaseVar(0M),

        // 每进入过一个状态，攻击1次
        new CalculationExtraVar(1M),

        // 最终次数 = 进入过的状态数量
        new CalculatedVar(CalculatedHitsKey)
            .WithMultiplier(
                (card, _) => StateRegistry.Get(card.Owner).EnteredStateCount
            )
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        // 次数统一走 CalculatedVar（和卡面显示同一套计算），不要在这里另算一份。
        int hitCount =
            (int)((CalculatedVar)DynamicVars[CalculatedHitsKey])
                .Calculate(cardPlay.Target);

        // 没进过状态 ⇒ 0 跳。这是设计意图（不再白送一跳），但 0 跳就别把
        // WithHitCount(0) 喂给 AttackCommand 了，行为未定义，直接跳过。
        if (hitCount <= 0)
            return;

        // Hyperbeam 光束
        NHyperbeamVfx.Create(
            Owner.Creature,
            cardPlay.Target
        );

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitCount(hitCount)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        // 升级只降费：2 → 1。
        EnergyCost.UpgradeBy(-1);
    }
}