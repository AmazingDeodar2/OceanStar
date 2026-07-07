using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using starss.starssCode.Mechanics;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace starss.starssCode.Cards;


public sealed class AlmostThere : starssCard
{
    public AlmostThere()
        : base(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(12M, ValueProp.Move),
        new CardsVar(4),
        new FateVar(50M)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        // 加入四叶草
        var clovers = CloverLeaf.Create(
            Owner,
            (int)DynamicVars.Cards.BaseValue,
            CombatState!
        );

        await CardPileCmd.AddGeneratedCardsToCombat(
            clovers,
            PileType.Draw,
            Owner
        );
        PileType.Draw.GetPile(Owner).InvokeCardAddFinished();

        // 命运检定
        var check = await DiceHelper.Check(
            Owner.Creature,
            fate: DynamicVars["Fate"].IntValue,
            doom: 101,
            choiceContext: choiceContext,
            sourceCard: this
        );

        // 命运成功，追加一次伤害
        if (check.FateSuccess)
        {
            await DamageCmd.Attack(8)
                .FromCard(this, cardPlay)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }
    }

   
    protected override void OnUpgrade()
    {
        DynamicVars["Fate"].UpgradeValueBy(20M);
    }
}
