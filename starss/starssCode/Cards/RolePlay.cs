using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Models;
using starss.starssCode.Cards.Interfaces;
using starss.starssCode.Mechanics;

namespace starss.starssCode.Cards;


public sealed class RolePlay : starssCard
{
    public RolePlay()
        : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        StarssHoverTips.Pc
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords
        => new[] { CardKeyword.Exhaust };

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(
            Owner.Creature,
            "Cast",
            Owner.Character.CastAnimDelay);

        CardModel? card = PcCardHelper.GetRandomPcCard(Owner);

        if (card == null)
            return;

        card.SetToFreeThisTurn();

        await CardPileCmd.AddGeneratedCardToCombat(
            card,
            PileType.Hand,
            Owner);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
