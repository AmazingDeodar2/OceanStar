using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace starss.starssCode.Cards;


public sealed class PenguinActivated : starssCard
{
    public PenguinActivated()
        : base(4, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        // BodySlam.cs:18-20 三件套：伤害 = CalculationBase + ExtraDamage × 状态牌数
        // （CalculatedVar.cs:70）。基础 29（升级 35），每张 +6，卡面 {CalculatedDamage} 实时
        // 显示最终伤害（UpdateCardPreview → Calculate，含力量等全局修正）。
        new CalculationBaseVar(29M),
        new ExtraDamageVar(6M),
        new CalculatedDamageVar(ValueProp.Move)
            .WithMultiplier((card, _) => ((PenguinActivated)card)._statusGenerated)
    ];

    /// <summary>
    /// “下一次打出前”的蓄力计数：每产生一张状态牌 +1，打出/开战时清零。
    /// 注意：该字段不参与游戏序列化——战斗中读档会丢失伤害加成（费用侧由
    /// EnergyCost 修饰器管理，不受影响）。卡面显示与实际伤害同源（都走 Calculate）。
    /// </summary>
    private int _statusGenerated;

    // 火箭飞拳模板（RocketPunch.cs:35-51）：只算自己产生的状态牌。
    // 官方虚空本身就是 CardType.Status（Void.cs:23），所以塞虚空/呼唤都会蓄力。
    public override Task AfterCardGeneratedForCombat(CardModel card, Player? creator)
    {
        if (creator != Owner)
            return Task.CompletedTask;
        if (card.Owner != Owner)
            return Task.CompletedTask;
        if (card.Type != CardType.Status)
            return Task.CompletedTask;

        _statusGenerated++;

        // 费用：引擎原生“打出前生效、打出后自动还原”（CardEnergyCost.cs:258 AddUntilPlayed），
        // 不需要任何手动重置。
        EnergyCost.AddUntilPlayed(-1);

        return Task.CompletedTask;
    }

    // 跨战斗防残留（Hook.cs:311 战斗开始时对 runState hook listeners 广播）。
    public override Task BeforeCombatStart()
    {
        _statusGenerated = 0;
        return base.BeforeCombatStart();
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        // 直接把 CalculatedDamage 传给 Attack：隐式转换取 Calculate(null)
        // （CalculatedVar.cs:107-110），力量等修正由 DamageCmd 内部再算。
        await DamageCmd.Attack(DynamicVars.CalculatedDamage)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_blunt", null, "blunt_attack.mp3")
            .Execute(choiceContext);

        // 打出后蓄力清零，回到基础伤害（费用修饰器由引擎自动清除）。
        _statusGenerated = 0;
    }

    protected override void OnUpgrade()
    {
        // 升级选 b：基础伤害 29 → 35（每张 +6 不变）。
        // 升级后引擎自动 RecalculateForUpgradeOrEnchant（CardModel.cs:2134），卡面无需手动刷。
        DynamicVars.CalculationBase.UpgradeValueBy(6M);
    }
}
