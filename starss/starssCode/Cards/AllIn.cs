using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using starss.starssCode.Mechanics;

namespace starss.starssCode.Cards;

public sealed class AllIn : starssCard
{
    public AllIn()
        : base(0, CardType.Skill, CardRarity.Rare, TargetType.AllEnemies)
    {
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords
    {
        get
        {
            return
            [
                CardKeyword.Exhaust
            ];
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(100M, ValueProp.Move),
        new FateVar(1M),
        new DoomVar(91M)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
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
                .FromCard(this,cardPlay)
                .TargetingAllOpponents(CombatState!)
                .WithHitFx("vfx/vfx_attack_blunt")
                .Execute(choiceContext);
        }

        if (check.DoomSuccess)
        {
            
            var hpLoss = Owner.Creature.CurrentHp - 1M;
            
            if (hpLoss > 0)
            {
                await CreatureCmd.Damage(
                    choiceContext,
                    Owner.Creature,
                    hpLoss,
                    ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move,
                    this,
                    cardPlay
                );
            }
        }
    }

    protected override void OnUpgrade() => this.RemoveKeyword(CardKeyword.Exhaust);
}