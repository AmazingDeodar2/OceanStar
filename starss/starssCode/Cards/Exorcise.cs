using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace starss.starssCode.Cards;


public sealed class Exorcise : starssCard
{
    public Exorcise()
        : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<CloverLeaf>(IsUpgraded)
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(2)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        List<CardModel> selectedCards =
            (await CardSelectCmd.FromHandForDiscard(
                choiceContext,
                Owner,
                new CardSelectorPrefs(SelectionScreenPrompt, 0, 999999999),
                (Func<CardModel, bool>)null,
                this
            )).ToList();

        foreach (CardModel card in selectedCards)
        {
            await CardCmd.DiscardAndDraw(choiceContext, selectedCards, 0);
        }

        List<CloverLeaf> clovers = [];

        for (int i = 0; i < DynamicVars.Cards.IntValue; i++)
        {
            CloverLeaf clover = CombatState!.CreateCard<CloverLeaf>(Owner);

            if (IsUpgraded)
                CardCmd.Upgrade(clover);

            clovers.Add(clover);
        }

        await CardPileCmd.AddGeneratedCardsToCombat(
            clovers,
            PileType.Draw,
            Owner
        );
    }

    protected override void OnUpgrade()
    {
    }
}