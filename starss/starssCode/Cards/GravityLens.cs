using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using VoidCard = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace starss.starssCode.Cards;

public sealed class GravityLens : starssCard
{
    

    public GravityLens()
        : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<CloverLeaf>(IsUpgraded)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var cardsToTransform = GetStatusAndCurseCards(Owner).ToList();

        foreach (CardModel original in cardsToTransform)
        {
            CardModel clover = CombatState!.CreateCard<CloverLeaf>(Owner);

            if (IsUpgraded)
                CardCmd.Upgrade(clover);

            await CardCmd.Transform(original, clover);
        }
    }

    private static IEnumerable<CardModel> GetStatusAndCurseCards(Player owner)
    {
        return owner.PlayerCombatState.AllCards
            .Where(c =>
                c.Pile.Type != PileType.Exhaust &&
                (c.Type is CardType.Status or CardType.Curse)
            );
    }
    protected override void OnUpgrade()
    {
        
    }
    
}