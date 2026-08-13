using MegaCrit.Sts2.Core.Entities.Cards;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Relics;

namespace starss.starssCode.Cards.ReputationServices;

[Pool(typeof(TokenCardPool))]
public sealed class CircletPackageService :
    starssCard,
    IReputationService
{
    public CircletPackageService()
        : base(
            0,
            CardType.Status,
            CardRarity.Token,
            TargetType.None)
    {
    }

    public async Task OnChosen()
    {
        // 先给予头环。
        // Circlet 的具体类型命名需要按你当前版本确认。
        await RelicCmd.Obtain<Circlet>(Owner);

        // 从当前角色的已解锁卡池中抽取3张不同的稀有牌。
        CardModel? selectedCard =
            await CardSelectCmd.FromChooseACardScreen(
                new BlockingPlayerChoiceContext(),
                CardFactory.GetDistinctForCombat(
                    Owner,
                    Owner.Character.CardPool
                        .GetUnlockedCards(
                            Owner.UnlockState,
                            Owner.RunState.CardMultiplayerConstraint
                        )
                        .Where(card => card.Rarity == CardRarity.Rare),
                    3,
                    Owner.RunState.Rng.CombatCardGeneration
                ).ToList(),
                Owner,
                true
            );

        if (selectedCard == null)
            return;

        selectedCard.SetToFreeThisTurn();

        await CardPileCmd.AddGeneratedCardToCombat(
            selectedCard,
            PileType.Hand,
            Owner
        );

        PileType.Hand
            .GetPile(Owner)
            .InvokeCardAddFinished();
    }

    protected override Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        return Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
    }
}