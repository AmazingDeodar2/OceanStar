using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using Godot;
using starss.starssCode.Mechanics;
namespace starss.starssCode.Cards;


public sealed class ChaosStrike : starssCard
{
    public ChaosStrike()
        : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
    }
    protected override HashSet<CardTag> CanonicalTags =>
    [
        CardTag.Strike
    ];
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(2M, ValueProp.Move),
        new DynamicVar("Hits", 5M),
        new DynamicVar("Power", 5M),
        new DynamicVar("Hitss", 2M),
        new FateVar(50M),
        new DoomVar(51M)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        
        var check = await DiceHelper.Check(
            Owner.Creature,
            fate: DynamicVars["Fate"].IntValue,
            doom: DynamicVars["Doom"].IntValue,
            choiceContext: choiceContext,
            sourceCard: this
        );

        if (check.FateSuccess)
        {
            
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .WithHitCount(DynamicVars["Hits"].IntValue)
                .FromCard(this, cardPlay)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
            
        }

        if (check.DoomSuccess)
        {
            await DamageCmd.Attack(DynamicVars["Power"].IntValue)
                .WithHitCount(DynamicVars["Hitss"].IntValue)
                .FromCard(this,cardPlay)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }
        
        
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Hits"].UpgradeValueBy(1M);
        DynamicVars["Power"].UpgradeValueBy(1M);
    }
}