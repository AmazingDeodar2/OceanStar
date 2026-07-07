using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using starss.starssCode.Mechanics;
using VoidCard = MegaCrit.Sts2.Core.Models.Cards.Void;
namespace starss.starssCode.Cards;


public sealed class Eloquence : starssCard
{
    public Eloquence()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(15M, ValueProp.Unpowered),
        new DoomVar(51M)
    ];
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<VoidCard>()
    ];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(
            Owner.Creature,
            DynamicVars.Block,
            cardPlay
        );

        var check = await DiceHelper.Check(
            Owner.Creature,
            fate: 101,
            doom: DynamicVars["Doom"].IntValue,
            choiceContext: choiceContext,
            sourceCard: this
        );

        if (check.DoomSuccess)
        {
            
            CardCmd.PreviewCardPileAdd(
                await CardPileCmd.AddGeneratedCardToCombat(
                    CombatState!.CreateCard<VoidCard>(Owner),
                    PileType.Discard,
                    Owner
                )
            );
            PileType.Discard.GetPile(Owner).InvokeCardAddFinished();
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(4M);
    }
}