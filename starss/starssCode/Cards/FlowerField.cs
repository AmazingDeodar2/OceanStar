using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.HoverTips;

namespace starss.starssCode.Cards;


public sealed class FlowerField : starssCard
{
    public FlowerField()
        : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<CloverLeaf>(IsUpgraded)
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(8M, ValueProp.Move),
        new CardsVar(2)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        
        await CreatureCmd.GainBlock(
            Owner.Creature,
            DynamicVars.Block,
            cardPlay
        );

        if (CombatState is null)
            return;

        for (int i = 0; i < DynamicVars.Cards.IntValue; i++)
        {
            var clover = CombatState.CreateCard<CloverLeaf>(Owner);

            if (IsUpgraded)
                CardCmd.Upgrade(clover);

            await CardPileCmd.AddGeneratedCardToCombat(
                clover,
                PileType.Draw,
                Owner
            );
        }
        PileType.Draw.GetPile(Owner).InvokeCardAddFinished();
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2M);
    }
}