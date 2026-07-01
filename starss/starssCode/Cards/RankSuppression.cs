using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Cards;
using starss.starssCode.Powers;

namespace starss.starssCode.Cards;


public sealed class RankSuppression : starssCard
{
    public RankSuppression()
        : base(1, CardType.Skill, CardRarity.Common, TargetType.AllEnemies)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("StrengthLoss", 8M)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        EnergyHoverTip,
        HoverTipFactory.FromCard<Beckon>()
    ];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(
            Owner.Creature,
            "Cast",
            Owner.Character.CastAnimDelay
        );

        foreach (Creature enemy in CombatState!.HittableEnemies)
        {
            await PowerCmd.Apply<RankSuppressionPower>(
                choiceContext,
                enemy,
                DynamicVars["StrengthLoss"].BaseValue,
                Owner.Creature,
                this
            );
        }

        CardModel callCard = Owner.Creature.CombatState.CreateCard<Beckon>(Owner);

        await CardPileCmd.AddGeneratedCardToCombat(
            callCard,
            PileType.Hand,
            Owner
        );

        PileType.Hand.GetPile(Owner).InvokeCardAddFinished();
    }

    protected override void OnUpgrade()
    {
        DynamicVars["StrengthLoss"].UpgradeValueBy(3M);
    }
}