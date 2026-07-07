using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using starss.starssCode.Powers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace starss.starssCode.Cards;

public sealed class BurnLuck : starssCard
{
    public BurnLuck()
        : base(0, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Luck", 5M),
        new CardsVar(2)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 幸运-5
        await PowerCmd.Apply<LuckyPower>(
            choiceContext,
            Owner.Creature,
            -DynamicVars["Luck"].BaseValue,
            Owner.Creature,
            this
        );

        // 抽牌
        await CardPileCmd.Draw(
            choiceContext,
            DynamicVars.Cards.BaseValue,
            Owner
        );
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Luck"].UpgradeValueBy(-3M);
    }
}