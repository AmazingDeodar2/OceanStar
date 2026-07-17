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


public sealed class RainbowFriendshipCannon : starssCard
{
    private const string CalculatedHitsKey = "CalculatedHits";
    
    public RainbowFriendshipCannon()
        : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(7M, ValueProp.Move),
        
        // 基础攻击1次
        new CalculationBaseVar(1M),

        // 每进入过一个状态，额外攻击1次
        new CalculationExtraVar(1M),

        // 最终次数 = 1 + 进入过的状态数量 × 1
        new CalculatedVar(CalculatedHitsKey)
            .WithMultiplier(
                (card, _) => StateRegistry.Get(card.Owner).EnteredStateCount
            )
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        int enteredStates = StateRegistry.Get(Owner).EnteredStateCount;

        int hitCount = 1 + enteredStates;

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this,cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitCount(hitCount)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3M);
    }
}