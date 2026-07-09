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

    private int _currentDamage = BaseDamage;
    private int _currentBlock = BaseBlock;
    private int _increasedValue;

    [SavedProperty]
    public int CurrentDamage
    {
        get => _currentDamage;
        set
        {
            AssertMutable();
            _currentDamage = value;
            DynamicVars.Damage.BaseValue = _currentDamage;
        }
    }

    [SavedProperty]
    public int CurrentBlock
    {
        get => _currentBlock;
        set
        {
            AssertMutable();
            _currentBlock = value;
            DynamicVars.Block.BaseValue = _currentBlock;
        }
    }

    [SavedProperty]
    public int IncreasedValue
    {
        get => _increasedValue;
        set
        {
            AssertMutable();
            _increasedValue = value;
        }
    }

    public Humanology()
        : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
    }

    

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(CurrentDamage, ValueProp.Move),
        new BlockVar(CurrentBlock, ValueProp.Move),
        new IntVar("BonusKey", 3M),
        new FateVar(30M),
        new DoomVar(71M)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new List<IHoverTip>();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        Humanology self = this;
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));
        Creature user = Owner.Creature;
        int addAmount = DynamicVars["BonusKey"].IntValue;

        // 总伤害 = 基础伤害 + 本卡本局累计伤害加成
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        await CreatureCmd.GainBlock(
            Owner.Creature,
            DynamicVars.Block.BaseValue,
            ValueProp.Move,
            null
        );

        // 命运厄运判定，写法对齐 DesertEagle
        var diceResult = await DiceHelper.Check(
            creature: user,
            fate: DynamicVars["Fate"].IntValue,
            doom: DynamicVars["Doom"].IntValue,
            choiceContext: choiceContext,
            sourceCard: self,
            showUi: true
        );

        if (diceResult.FateSuccess)
        {
           
            BuffFromPlay(addAmount);

            if (DeckVersion is Humanology deckVersion)
                deckVersion.BuffFromPlay(addAmount);
        }

        bool doomTriggered = diceResult.DoomSuccess;
        if (doomTriggered)
        {
            
            
            await CardCmd.Exhaust(choiceContext, self);
            
        }

    }

    private void BuffFromPlay(int value)
    {
        IncreasedValue += value;
        UpdateValues();
    }

    private void UpdateValues()
    {
        CurrentDamage = BaseDamage + IncreasedValue;
        CurrentBlock = BaseBlock + IncreasedValue;
    }
    protected override void OnUpgrade() => DynamicVars["BonusKey"].UpgradeValueBy(1M);
}