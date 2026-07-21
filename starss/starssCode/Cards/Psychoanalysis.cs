using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Models;

namespace starss.starssCode.Cards;


public sealed class Psychoanalysis : starssCard
{
    public Psychoanalysis()
        : base(
            1,
            CardType.Skill,
            CardRarity.Uncommon,
            TargetType.Self)
    {
    }

    public override CardMultiplayerConstraint MultiplayerConstraint
        => CardMultiplayerConstraint.MultiplayerOnly;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<CloverLeaf>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        int exhaustedCount = 0;

        

        IEnumerable<Creature> teammates =
            CombatState
                .GetTeammatesOf(Owner.Creature)
                .Where(creature => creature != Owner.Creature);

        foreach (Creature teammate in teammates)
        {
            Player? teammatePlayer = teammate.Player;

            if (teammatePlayer == null)
                continue;

            exhaustedCount += await ExhaustStatusAndCurseCards(
                choiceContext,
                teammatePlayer
            );
        }

        if (exhaustedCount <= 0)
            return;

        List<CardModel> clovers = [];

        for (int i = 0; i < exhaustedCount; i++)
        {
            CloverLeaf clover =
                CombatState.CreateCard<CloverLeaf>(Owner);

            if (IsUpgraded)
                CardCmd.Upgrade(clover);

            clovers.Add(clover);
        }

        await CardPileCmd.AddGeneratedCardsToCombat(
            clovers,
            PileType.Draw,
            Owner,
            CardPilePosition.Random
        );

        PileType.Draw
            .GetPile(Owner)
            .InvokeCardAddFinished();
    }

    private static async Task<int> ExhaustStatusAndCurseCards(
        PlayerChoiceContext choiceContext,
        Player player)
    {
        int count = 0;

        List<CardModel> cards = CardPile
            .GetCards(
                player,
                PileType.Hand,
                PileType.Draw,
                PileType.Discard
            )
            .Where(card =>
                card.Type == CardType.Status ||
                card.Type == CardType.Curse)
            .ToList();

        foreach (CardModel card in cards)
        {
            await CardCmd.Exhaust(choiceContext, card);
            count++;
        }

        return count;
    }

    protected override void OnUpgrade()
    {
        // 升级效果由 OnPlay 中的 IsUpgraded 控制。
    }
}