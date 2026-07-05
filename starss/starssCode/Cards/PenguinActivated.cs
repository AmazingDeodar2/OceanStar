using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace starss.starssCode.Cards;


public sealed class PenguinActivated : starssCard
{
    public PenguinActivated()
        : base(3, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(25M, ValueProp.Move),
        new BlockVar(10M, ValueProp.Unpowered)
    ];
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Beckon>()
    ];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this,cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);

        await CreatureCmd.GainBlock(
            Owner.Creature,
            DynamicVars.Block,
            cardPlay
        );

        
        CardModel callCard = Owner.Creature.CombatState.CreateCard<Beckon>(Owner);
        await CardPileCmd.AddGeneratedCardToCombat(
            callCard,
            PileType.Discard,
            Owner
        );
        PileType.Discard.GetPile(Owner).InvokeCardAddFinished();
        
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(5M);
    }
}