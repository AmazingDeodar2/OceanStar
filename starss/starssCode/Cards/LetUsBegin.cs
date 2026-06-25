using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using starss.starssCode.Mechanics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace starss.starssCode.Cards;

public sealed class LetUsBegin : starssCard
{
    public LetUsBegin()
        : base(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Innate,
        CardKeyword.Exhaust
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(4M, ValueProp.Move),
        new DynamicVar("Repeat", 2M),
        new FateVar(50M),
        new DynamicVar("Bonus", 1M)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        for (var i = 0; i < DynamicVars["Repeat"].IntValue; i++)
        {
            await AttackRandomEnemy(choiceContext);
        }

        var check = await DiceHelper.Check(
            Owner.Creature,
            fate: 50,
            doom: 101
        );

        if (check.FateSuccess)
        {
            for (var i = 0; i < DynamicVars["Bonus"].IntValue; i++)
            {
                await AttackRandomEnemy(choiceContext);
            }
        }
    }

    private async Task AttackRandomEnemy(PlayerChoiceContext choiceContext)
    {
        var enemies = CombatState!.HittableEnemies.ToList();
        if (enemies.Count == 0)
            return;

        var target = enemies[Random.Shared.Next(enemies.Count)];

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Bonus"].UpgradeValueBy(1M);
    }
}