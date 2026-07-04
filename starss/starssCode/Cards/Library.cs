using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using starss.starssCode.Mechanics;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace starss.starssCode.Cards;


public sealed class Library : starssCard
{
    public Library()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(2),
        new FateVar(60M)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var check = await DiceHelper.Check(
            Owner.Creature,
            fate: DynamicVars["Fate"].IntValue,
            doom: 101,
            choiceContext: choiceContext,
            sourceCard: this
        );
        await CardPileCmd.Draw(
            choiceContext,
            DynamicVars.Cards.BaseValue,
            Owner
        );
        if (check.FateSuccess)
        {
            await CardPileCmd.Draw(
                choiceContext,
                2,
                Owner
            );

            
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1M);
    }
}