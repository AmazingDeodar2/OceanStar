using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using starss.starssCode.Mechanics;
using starss.starssCode.Powers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Models;

namespace starss.starssCode.Cards;

public sealed class LuckyFinish : starssCard
{
    private const string CalculatedDamageKey = "CalculatedDamage";
    public LuckyFinish()
        : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new FateVar(50M),
        new CalculationBaseVar(0M),
        new CalculationExtraVar(1M),
        new CalculatedVar(CalculatedDamageKey)
            .WithMultiplier((card, _) => GetLuckDamage(card))
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        // 当前幸运值
        
        var damage = GetLuckDamage(this);
        // 第一次伤害
        await DamageCmd.Attack(damage)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        // 命运检定
        var check = await DiceHelper.Check(
            Owner.Creature,
            fate: 50,
            doom: 101,
            choiceContext: choiceContext,
            sourceCard: this
        );

        // 命运成功，再打一遍
        if (check.FateSuccess)
        {
            await DamageCmd.Attack(damage)
                .FromCard(this)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }
    }
    private static decimal GetLuckDamage(CardModel card)
    {
        return card.Owner.Creature.GetPower<LuckyPower>()?.Amount ?? 0M;
    }
    protected override void OnUpgrade() => this.EnergyCost.UpgradeBy(-1);
}