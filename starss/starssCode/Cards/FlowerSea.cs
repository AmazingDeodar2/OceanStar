using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace starss.starssCode.Cards;

public sealed class FlowerSea : starssCard
{
    public FlowerSea()
        : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override bool HasEnergyCostX => true;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<CloverLeaf>(IsUpgraded)
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(2),
        new BlockVar(4M, ValueProp.Move)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int x = ResolveEnergyXValue();

        if (CombatState is null)
            return;

        List<CloverLeaf> clovers = new();

        for (int i = 0; i < DynamicVars.Cards.IntValue * x; i++)
        {
            CloverLeaf clover = CombatState.CreateCard<CloverLeaf>(Owner);

            if (IsUpgraded)
                CardCmd.Upgrade(clover);

            clovers.Add(clover);
        }

        await CardPileCmd.AddGeneratedCardsToCombat(
            clovers,
            PileType.Draw,
            Owner
        );
        PileType.Draw.GetPile(Owner).InvokeCardAddFinished();
        var block = new BlockVar(
            DynamicVars.Block.IntValue * x,
            ValueProp.Move
        );
        
        await CreatureCmd.GainBlock(
            Owner.Creature,
            block,
            cardPlay
        );

    }

    protected override void OnUpgrade()
    {
    }
}