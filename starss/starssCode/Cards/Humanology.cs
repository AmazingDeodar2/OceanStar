using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using starss.starssCode.Mechanics;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace starss.starssCode.Cards;



public sealed class Humanology : starssCard
{
    private const int BaseDamage = 7;
    private const int BaseBlock = 7;
    private const string BonusKey = "BonusValue";

    // TheScythe 同款存档持久字段，每张卡独立数据
    private int _totalDamageBonus;
    private int _totalBlockBonus;

    [SavedProperty]
    public int TotalDamageBonus
    {
        get => _totalDamageBonus;
        set
        {
            AssertMutable();
            _totalDamageBonus = value;
        }
    }

    [SavedProperty]
    public int TotalBlockBonus
    {
        get => _totalBlockBonus;
        set
        {
            AssertMutable();
            _totalBlockBonus = value;
        }
    }

    public Humanology()
        : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
    }

    

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(BaseDamage, ValueProp.Move),
        new BlockVar(BaseBlock, ValueProp.Move),
        new IntVar("BonusKey", 3M),
        new FateVar(40M),
        new DoomVar(80M)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new List<IHoverTip>();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        Humanology self = this;
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));
        Creature user = Owner.Creature;
        int addAmount = DynamicVars["BonusKey"].IntValue;

        // 总伤害 = 基础伤害 + 本卡本局累计伤害加成
        decimal totalAttackDmg = DynamicVars.Damage.BaseValue + TotalDamageBonus;
        await DamageCmd.Attack(totalAttackDmg)
            .FromCard(self)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        // 总格挡 = 基础格挡 + 本卡本局累计格挡加成
        decimal totalGainBlock = DynamicVars.Block.BaseValue + TotalBlockBonus;
        await CreatureCmd.GainBlock(user, totalGainBlock, ValueProp.Move, null);

        // 命运厄运判定，写法对齐 DesertEagle
        var diceResult = await DiceHelper.Check(
            creature: user,
            fate: 40,
            doom: 80,
            choiceContext: choiceContext,
            sourceCard: self,
            showUi: true
        );

        if (diceResult.FateSuccess)
        {
            await DiceHelper.OnFateTriggered(choiceContext, self);
            BuffDamage(addAmount);
            BuffBlock(addAmount);
        }

        bool doomTriggered = diceResult.DoomSuccess;
        if (doomTriggered)
        {
            await DiceHelper.OnDoomTriggered(choiceContext, self);
            
            await CardCmd.Exhaust(choiceContext, self);
            
        }

    }

    private void BuffDamage(int value)
    {
        TotalDamageBonus += value;
    }

    private void BuffBlock(int value)
    {
        TotalBlockBonus += value;
    }
    protected override void OnUpgrade() => DynamicVars["BonusKey"].UpgradeValueBy(1M);
}