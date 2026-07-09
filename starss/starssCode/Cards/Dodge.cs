using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using starss.starssCode.Mechanics;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Models;

namespace starss.starssCode.Cards;


public sealed class Dodge : starssCard
{
    public Dodge()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(10M, ValueProp.Move),
        new FateVar(60M)
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
            fate: DynamicVars["Fate"].IntValue,
            doom: 101,
            choiceContext: choiceContext,
            sourceCard: this
        );

        if (!check.FateSuccess)
            return;
        Dodge dodge = this;
        CardSelectorPrefs prefs = new CardSelectorPrefs(dodge.SelectionScreenPrompt, 1);
        PileType.Discard.GetPile(dodge.Owner);
        CardModel card = (await CardSelectCmd.FromCombatPile(choiceContext, PileType.Discard.GetPile(dodge.Owner), dodge.Owner, prefs)).FirstOrDefault<CardModel>();
        bool flag1 = card != null;
        if (flag1)
        {
            PileType? type = card.Pile?.Type;
            bool flag2;
            if (type.HasValue)
            {
                switch (type.GetValueOrDefault())
                {
                    case PileType.Draw:
                    case PileType.Discard:
                        flag2 = true;
                        goto label_7;
                }
            }
            flag2 = false;
            label_7:
            flag1 = flag2;
        }
        if (!flag1)
            return;
        CardPileAddResult cardPileAddResult = await CardPileCmd.Add(card, PileType.Draw, CardPilePosition.Top);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(4M);
    }
}