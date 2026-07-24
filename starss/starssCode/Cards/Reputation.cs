using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using starss.starssCode.Cards.ReputationServices;
using System.Collections.Generic;
using System.Threading.Tasks;
using starss.starssCode.States;

namespace starss.starssCode.Cards;

public sealed class Reputation : starssCard
{
    private const int RandomRelicBasePrice = 150;
    private const int RemoveCardBasePrice = 100;

    public Reputation()
        : base(
            1,
            CardType.Skill,
            CardRarity.Rare,
            TargetType.Self)
    {
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(
            Owner.Creature,
            "Cast",
            Owner.Character.CastAnimDelay
        );

        bool isInPonyLand =
            StateRegistry
                .Get(Owner)
                .Has<PonyLandState>();

        int randomRelicPrice = isInPonyLand
            ? RandomRelicBasePrice / 2
            : RandomRelicBasePrice;

        int cardRemovalPrice = isInPonyLand
            ? RemoveCardBasePrice / 2
            : RemoveCardBasePrice;

        RandomRelicService randomRelicService =
            CombatState.CreateCard<RandomRelicService>(Owner);

        randomRelicService.DynamicVars["Price"].BaseValue =
            randomRelicPrice;

        RemoveCardService removeCardService =
            CombatState.CreateCard<RemoveCardService>(Owner);

        removeCardService.DynamicVars["Price"].BaseValue =
            cardRemovalPrice;

        CircletPackageService circletPackageService =
            CombatState.CreateCard<CircletPackageService>(Owner);

        List<CardModel> services =
        [
            randomRelicService,
            removeCardService,
            circletPackageService
        ];

        CardModel? selectedService =
            await CardSelectCmd.FromChooseACardScreen(
                choiceContext,
                services,
                Owner
            );

        if (selectedService is not IReputationService service)
            return;

        await service.OnChosen();
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}