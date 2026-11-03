using starss.starssCode.Cards.Interfaces;
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

namespace starss.starssCode.Cards;


public sealed class Brawl : starssCard, IFateCard
{
    public Brawl()
        : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(12M, ValueProp.Move),
        new FateVar(60M)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 对所有可以被攻击的敌人造成伤害。
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue) 
            .FromCard(this, cardPlay) 
            .TargetingAllOpponents(CombatState) 
            .WithHitFx("vfx/vfx_attack_slash") 
            .Execute(choiceContext); 
      
        
        var check = await DiceHelper.Check(
            Owner.Creature, 
            fate: DynamicVars["Fate"].IntValue, 
            doom: 101, 
            choiceContext: choiceContext, 
            sourceCard: this ); 
        // 命运：这张卡在本场战斗中的费用减少 1（困难成功的额外次数由「坦途」提供）。
        await FateEffect.Run(check, Owner.Creature, () => EnergyCost.AddThisCombat(-1));
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3M);
    }
}