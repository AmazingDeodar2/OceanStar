using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using starss.starssCode.Powers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Models;

namespace starss.starssCode.Cards;


public sealed class DisasterRelief : starssCard
{
    public DisasterRelief()
        : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Luck", 5M)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        CardModel? selected = (await CardSelectCmd.FromCombatPile(
            choiceContext,
            PileType.Draw.GetPile(Owner),
            Owner,
            new CardSelectorPrefs(
                CardSelectorPrefs.TransformSelectionPrompt,
                1
            )
        )).FirstOrDefault();

        if (selected != null)
        {
            CardPileAddResult? result =
                await CardCmd.TransformTo<CloverLeaf>(selected);

            if (IsUpgraded && result.HasValue)
            {
                CardCmd.Upgrade(result.Value.cardAdded);
            }
        }

        await PowerCmd.Apply<LuckyPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars["Luck"].BaseValue,
            Owner.Creature,
            this
        );
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Luck"].UpgradeValueBy(3M);
    }
}