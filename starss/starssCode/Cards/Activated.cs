using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using starss.starssCode.Powers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace starss.starssCode.Cards;

public sealed class Activated : starssCard
{
    public Activated()
        : base(1, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
    {
    }


    // 幸运的获得量取决于实际造成的总伤害，做不成 DynamicVar ⇒ 只能给卡级 tip
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<LuckyPower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(5M, ValueProp.Move)
    ];


    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        var results =
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                .FromCard(this, cardPlay)
                .TargetingAllOpponents(CombatState!)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);


        // 只统计实际扣除的生命值（UnblockedDamage 已扣除格挡，且不含溢出，
        // 见 DamageResult.cs:32-41），与官方 Omnislice 取 DamageResult 的做法一致。
        int totalDamage =
            results.Results
                .SelectMany(r => r)
                .Sum(r => r.UnblockedDamage);


        if (totalDamage > 0)
        {
            await PowerCmd.Apply<LuckyPower>(
                choiceContext,
                Owner.Creature,
                totalDamage,
                Owner.Creature,
                this
            );
        }
    }


    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3M);
    }
}