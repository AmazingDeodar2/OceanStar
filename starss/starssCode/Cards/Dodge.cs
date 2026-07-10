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
        CardSelectorPrefs prefs = new(
            SelectionScreenPrompt,
            1
        );

        CardModel? selectedCard =
            (await CardSelectCmd.FromCombatPile(
                choiceContext,
                PileType.Discard.GetPile(Owner),
                Owner,
                prefs
            )).FirstOrDefault();

        if (selectedCard == null)
            return;

        PileType? pileType = selectedCard.Pile?.Type;

        if (pileType != PileType.Draw &&
            pileType != PileType.Discard)
        {
            return;
        }

        await CardPileCmd.Add(
            selectedCard,
            PileType.Draw,
            CardPilePosition.Top
        );
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(4M);
    }
}