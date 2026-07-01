using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using starss.starssCode.Mechanics;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace starss.starssCode.Cards;


public sealed class SleightOfHand : starssCard
{
    public SleightOfHand()
        : base(0, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
    {
    }

    protected override bool HasEnergyCostX => true;

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DoomVar(40M)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        int x = ResolveEnergyXValue();

        // 敌方失去 X（升级 X+1）力量
        int enemyLoss = IsUpgraded ? x + 1 : x;

        await PowerCmd.Apply<StrengthPower>(
            choiceContext,
            cardPlay.Target,
            -enemyLoss,
            Owner.Creature,
            this);

        // 自己获得 X 力量
        if (x > 0)
        {
            await PowerCmd.Apply<StrengthPower>(
                choiceContext,
                Owner.Creature,
                x,
                Owner.Creature,
                this);
        }

        var check = await DiceHelper.Check(
            Owner.Creature,
            fate: 101,
            doom: DynamicVars["Doom"].IntValue,
            choiceContext: choiceContext,
            sourceCard: this
        );

        if (check.DoomSuccess)
        {
            

            CardCmd.PreviewCardPileAdd(
                await CardPileCmd.AddGeneratedCardToCombat(
                    CombatState!.CreateCard<Void>(Owner),
                    PileType.Discard,
                    Owner
                )
            );
        }
    }
}