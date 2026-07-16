using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using starss.starssCode.Mechanics;
using VoidCard = MegaCrit.Sts2.Core.Models.Cards.Void;
namespace starss.starssCode.Cards;
using MegaCrit.Sts2.Core.Combat;  
using MegaCrit.Sts2.Core.Entities.Players;  


public sealed class Intimidate : starssCard
{
    private bool _generatedStatusThisTurn;

    private bool GeneratedStatusThisTurn
    {
        get => _generatedStatusThisTurn;
        set
        {
            AssertMutable();
            _generatedStatusThisTurn = value;
        }
    }

    public Intimidate()
        : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(14M, ValueProp.Move),
        new PowerVar<WeakPower>(1M)
    ];
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<WeakPower>()
    ];
    
    
    protected override bool ShouldGlowGoldInternal =>
        GeneratedStatusThisTurn;
    
    public override Task AfterCardGeneratedForCombat(
        CardModel card,
        Player? creator)
    {
        if (creator != Owner ||
            card.Owner != Owner ||
            card.Type != CardType.Status)
        {
            return Task.CompletedTask;
        }
        GeneratedStatusThisTurn = true;

        return Task.CompletedTask;
    }

    
    public override Task AfterEnergyReset(Player player)
    {
        if (player == Owner)
        {
            GeneratedStatusThisTurn = false;
        }

        return Task.CompletedTask;
    }
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        bool generatedStatus = GeneratedStatusThisTurn; 
        
        int hitCount = generatedStatus ? 2 : 1; 
        // 本回合生成过状态牌时攻击2次，否则攻击1次。
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue) 
            .WithHitCount(hitCount) 
            .FromCard(this, cardPlay) 
            .Targeting(cardPlay.Target)
            .WithHitFx( "vfx/vfx_attack_blunt", tmpSfx: "heavy_attack.mp3" ) 
            .Execute(choiceContext); 
        // 只有本回合生成过状态牌时才给予虚弱。
        if (generatedStatus)
        {
            await PowerCmd.Apply<WeakPower>( 
                choiceContext,
                cardPlay.Target,
                DynamicVars.Weak.BaseValue, 
                Owner.Creature, 
                this
                );
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3M);
        DynamicVars.Weak.UpgradeValueBy(1M);
    }
}