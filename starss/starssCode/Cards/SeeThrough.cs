using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Models.Cards;

namespace starss.starssCode.Cards;


public sealed class SeeThrough : starssCard
{
    public SeeThrough()
        : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Power", 2M)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<VulnerablePower>(),
        HoverTipFactory.FromPower<ArtifactPower>(),
        HoverTipFactory.Static(StaticHoverTip.Block),
        EnergyHoverTip,
        HoverTipFactory.FromCard<Beckon>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        await CreatureCmd.TriggerAnim(
            Owner.Creature,
            "Cast",
            Owner.Character.CastAnimDelay
        );

        VfxCmd.PlayOnCreatureCenter(
            Owner.Creature,
            "vfx/vfx_flying_slash"
        );

        await CreatureCmd.LoseBlock(
            cardPlay.Target,
            cardPlay.Target.Block
        );

        if (cardPlay.Target.HasPower<ArtifactPower>())
        {
            await PowerCmd.Remove<ArtifactPower>(cardPlay.Target);
        }

        await PowerCmd.Apply<VulnerablePower>(
            choiceContext,
            cardPlay.Target,
            DynamicVars["Power"].BaseValue,
            Owner.Creature,
            this
        );

        CardModel callCard = Owner.Creature.CombatState.CreateCard<Beckon>(Owner);

        await CardPileCmd.AddGeneratedCardToCombat(
            callCard,
            PileType.Hand,
            Owner
        );

        PileType.Hand.GetPile(Owner).InvokeCardAddFinished();
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Power"].UpgradeValueBy(1M);
    }
}