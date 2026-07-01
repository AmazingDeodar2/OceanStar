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
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

namespace starss.starssCode.Cards;

public sealed class DesertEagle : starssCard
{
    public DesertEagle()
        : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }
    // private bool exhaustThisPlay;
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6M, ValueProp.Move),
        new DynamicVar("Repeat", 3M),
        new FateVar(80M),
        new DoomVar(81M)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        var check = await DiceHelper.Check(
            Owner.Creature,
            fate: 80,
            doom: 81,
            choiceContext: choiceContext,
            sourceCard: this
        );

        if (check.FateSuccess)
        {
            
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .WithHitCount(DynamicVars["Repeat"].IntValue)
                .FromCard(this)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
            
        }

        if (check.DoomSuccess)
        {
            
            await CreatureCmd.Damage(
                choiceContext,
                Owner.Creature,
                DynamicVars.Damage.BaseValue,
                ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move,
                this
            );

            await CardCmd.Exhaust(choiceContext, this);
        }
    }
    // public override (PileType, CardPilePosition) ModifyCardPlayResultPileTypeAndPosition(
    //     CardModel card,
    //     bool isAutoPlay,
    //     ResourceInfo resources,
    //     PileType pileType,
    //     CardPilePosition position)
    // {
    //     if (!exhaustThisPlay)
    //         return (pileType, position);
    //
    //     exhaustThisPlay = false;
    //     return (PileType.Exhaust, CardPilePosition.Top);
    // }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2M);
    }
}