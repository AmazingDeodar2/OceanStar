using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using starss.starssCode.Mechanics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace starss.starssCode.Cards;


public sealed class SpinningStrike : starssCard
{
    public SpinningStrike()
        : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    // 打击牌：可触发打击木偶（StrikeDummy.cs:29 判 Tags.Contains(CardTag.Strike)，
    // 条件是 IsPoweredAttack = Move 且非 Unpowered，ValuePropExtensions.cs:5-12）。
    protected override HashSet<CardTag> CanonicalTags =>
    [
        CardTag.Strike
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        // BodySlam.cs:16-21 三件套：伤害 = 8 + 4 × 连续命运成功次数，卡面 {CalculatedDamage} 实时显示。
        new CalculationBaseVar(8M),
        new ExtraDamageVar(4M),
        new CalculatedDamageVar(ValueProp.Move)
            .WithMultiplier((card, _) => ((SpinningStrike)card)._bonusStacks),
        new FateVar(50M)
    ];

    /// <summary>
    /// 连续命运成功的叠加层数：成功 +1（伤害 +4），失败清零（伤害回到 8）。
    /// 只统计本张牌自己的判定，跨回合保留到本次战斗结束。
    /// 注意：该字段不参与游戏序列化——战斗中读档会丢失叠加层数。
    /// </summary>
    private int _bonusStacks;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        // 直接传 CalculatedDamage：隐式转换取 Calculate(null)（CalculatedVar.cs:107-110）。
        // 自动打出时 cardPlay.Target 已被 CardCmd.AutoPlay 随机选好（CardCmd.cs:73-84）。
        await DamageCmd.Attack(DynamicVars.CalculatedDamage)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        // 命运 50 判定；doom 传 0 表示完全不用厄运（DiceHelper.cs:52 只在 doom > 0 时启用）。
        var check = await DiceHelper.Check(
            Owner.Creature,
            fate: DynamicVars["Fate"].IntValue,
            doom: 0,
            choiceContext: choiceContext,
            sourceCard: this
        );

        if (check.FateSuccess)
            _bonusStacks++;
        else
            _bonusStacks = 0;
    }

    /// <summary>
    /// 回合开始时（AutoPrePlay 阶段，抽牌之后）：若手牌或弃牌堆中有状态牌，
    /// 且本牌在抽牌堆或弃牌堆中，则自动打出（免费）。
    ///
    /// 用 Early 变体是官方 Bombardment.cs:44-55 的原话做法：防止与乱战等其它
    /// auto-pre-play 效果重复触发（例如乱战正把这张牌从抽牌堆顶打出）。
    /// 牌在非常规堆也能自动打出，因为 OnPlayWrapper 对 isAutoPlay 会无条件先移到
    /// Play 堆（CardModel.cs:1868-1875）。
    /// </summary>
    public override async Task AfterAutoPrePlayPhaseEnteredEarly(PlayerChoiceContext choiceContext, Player player)
    {
        CardPile? pile = Pile;

        if (pile == null || player != Owner)
            return;

        if (pile.Type != PileType.Draw && pile.Type != PileType.Discard)
            return;

        if (!HasStatusCard())
            return;

        await CardCmd.AutoPlay(choiceContext, this, null);
    }

    /// <summary>手牌或弃牌堆中是否有状态牌（本牌是 Attack，不会把自己算进去）。</summary>
    private bool HasStatusCard()
    {
        return
            PileType.Hand.GetPile(Owner).Cards.Any(card =>
                card.Type == CardType.Status)
            ||
            PileType.Discard.GetPile(Owner).Cards.Any(card =>
                card.Type == CardType.Status);
    }

    protected override void OnUpgrade()
    {
        // 升级：命运 50 → 70
        DynamicVars["Fate"].UpgradeValueBy(20M);
    }
}
