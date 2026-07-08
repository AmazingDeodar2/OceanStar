using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using starss.starssCode.Cards.Interfaces;
using starss.starssCode.Mechanics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using VoidCard = MegaCrit.Sts2.Core.Models.Cards.Void;
using BeckonCard = MegaCrit.Sts2.Core.Models.Cards.Beckon;
namespace starss.starssCode.Cards;


public sealed class SpinningStrike : starssCard
{
    public SpinningStrike()
        : base(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
    }
    protected override HashSet<CardTag> CanonicalTags =>
    [
        CardTag.Strike
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(10M, ValueProp.Move),
        new FateVar(50M),
        new DynamicVar("BonusDamage", 4M)
    ];

    

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!HasVoidOrCall())
            return;

        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this,cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        var check = await DiceHelper.Check(
            Owner.Creature,
            fate: DynamicVars["Fate"].IntValue,
            doom: 101,
            choiceContext: choiceContext,
            sourceCard: this
        );

        if (check.FateSuccess)
        {
            
            await DamageCmd.Attack(DynamicVars["BonusDamage"].BaseValue)
                .FromCard(this,cardPlay)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }
    }

    private bool HasVoidOrCall()
    {
        return
            PileType.Hand.GetPile(Owner).Cards.Any(card =>
                card is VoidCard || card is BeckonCard)
            ||
            PileType.Discard.GetPile(Owner).Cards.Any(card =>
                card is VoidCard || card is BeckonCard);
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3M);
    }
}