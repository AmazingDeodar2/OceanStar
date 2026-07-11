using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
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
    private const int BaseValue = 7;

    private int _currentValue = BaseValue;
    private int _increasedValue;

    [SavedProperty]
    public int CurrentValue
    {
        get => _currentValue;
        set
        {
            AssertMutable();

            _currentValue = value;

            DynamicVars.Damage.BaseValue = _currentValue;
            DynamicVars.Block.BaseValue = _currentValue;
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
        : base(
            1,
            CardType.Attack,
            CardRarity.Rare,
            TargetType.AnyEnemy)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(CurrentValue, ValueProp.Move),
        new BlockVar(CurrentValue, ValueProp.Move),
        new IntVar("BonusKey", 3M),
        new FateVar(30M),
        new DoomVar(71M)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        Creature user = Owner.Creature;
        int addAmount = DynamicVars["BonusKey"].IntValue;

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        await CreatureCmd.GainBlock(
            Owner.Creature,
            DynamicVars.Block,
            cardPlay
        );

        var diceResult = await DiceHelper.Check(
            creature: user,
            fate: DynamicVars["Fate"].IntValue,
            doom: DynamicVars["Doom"].IntValue,
            choiceContext: choiceContext,
            sourceCard: this,
            showUi: true
        );

        if (diceResult.FateSuccess)
        {
            BuffFromPlay(addAmount);

            if (DeckVersion is Humanology deckVersion)
                deckVersion.BuffFromPlay(addAmount);
        }

        if (diceResult.DoomSuccess)
        {
            await CardCmd.Exhaust(
                choiceContext,
                this
            );
        }
    }

    private void BuffFromPlay(int extraValue)
    {
        IncreasedValue += extraValue;
        UpdateValues();
    }

    private void UpdateValues()
    {
        CurrentValue = BaseValue + IncreasedValue;
    }

    protected override void OnUpgrade()
    {
        DynamicVars["BonusKey"].UpgradeValueBy(1M);
    }

    protected override void AfterDowngraded()
    {
        UpdateValues();
    }
}