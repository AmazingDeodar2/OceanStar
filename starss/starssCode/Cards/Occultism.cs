using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using starss.starssCode.Mechanics;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Models.Cards;
using RegretCard = MegaCrit.Sts2.Core.Models.Cards.Regret;

namespace starss.starssCode.Cards;


public sealed class Occultism : starssCard
{
    public Occultism()
        : base(
            1,
            CardType.Skill,
            CardRarity.Rare,
            TargetType.AnyAlly)
    {
    }

    public override CardMultiplayerConstraint MultiplayerConstraint
        => CardMultiplayerConstraint.MultiplayerOnly;

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DoomVar(61M)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<RegretCard>()
    ];

    public DiceRollResult RollD6Result { get; private set; }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(
            cardPlay.Target,
            nameof(cardPlay.Target)
        );

        Player? selectedPlayer = cardPlay.Target.Player;

        if (selectedPlayer == null)
            return;

        // “其他玩家”：排除自己。
        if (selectedPlayer == Owner)
            return;

        await CreatureCmd.TriggerAnim(
            Owner.Creature,
            "Cast",
            Owner.Character.CastAnimDelay
        );

        // 投掷一枚六面骰并展示结果。
        RollD6Result = DiceHelper.RollD6(
            Owner.Creature,
            this
        );

        await DiceUi.ShowRoll(RollD6Result);

        int roll = RollD6Result.Value;

        // 根据骰点给予目标玩家神秘奖励。
        await GiveMysteriousReward(
            choiceContext,
            selectedPlayer,
            roll
        );

        // 厄运60检定。
        DiceCheckResult result = await DiceHelper.Check(
            Owner.Creature,
            fate: 101,
            doom: DynamicVars["Doom"].IntValue,
            choiceContext: choiceContext,
            sourceCard: this
        );

        // 厄运成功：将一张悔恨加入自己的手牌。
        if (result.DoomSuccess)
        {
            CardModel regret =
                CombatState.CreateCard<RegretCard>(Owner);

            await CardPileCmd.AddGeneratedCardToCombat(
                regret,
                PileType.Hand,
                Owner
            );

            PileType.Hand
                .GetPile(Owner)
                .InvokeCardAddFinished();
        }
    }

    private async Task GiveMysteriousReward(
        PlayerChoiceContext choiceContext,
        Player selectedPlayer,
        int roll)
    {
        
        switch (roll)
        {
            case 1:
                CardModel brightFlame =
                    CombatState.CreateCard<BrightestFlame>(
                        selectedPlayer
                    );

                
                CardCmd.ApplyKeyword(
                    brightFlame,
                    CardKeyword.Exhaust
                );

                
                if (IsUpgraded)
                {
                    CardCmd.Upgrade(brightFlame);
                }

                await CardPileCmd.AddGeneratedCardToCombat(
                    brightFlame,
                    PileType.Hand,
                    selectedPlayer
                );

                PileType.Hand
                    .GetPile(selectedPlayer)
                    .InvokeCardAddFinished();

                break;

            case 2:
                CardModel feedingFrenzy =
                    CombatState.CreateCard<FeedingFrenzy>(
                        selectedPlayer
                    );

                
                CardCmd.ApplyKeyword(
                    feedingFrenzy,
                    CardKeyword.Exhaust
                );

                
                if (IsUpgraded)
                {
                    CardCmd.Upgrade(feedingFrenzy);
                }

                await CardPileCmd.AddGeneratedCardToCombat(
                    feedingFrenzy,
                    PileType.Hand,
                    selectedPlayer
                );

                PileType.Hand
                    .GetPile(selectedPlayer)
                    .InvokeCardAddFinished();

                break;

            case 3:
                CardModel wish =
                    CombatState.CreateCard<Wish>(
                        selectedPlayer
                    );

                if (IsUpgraded)
                {
                    CardCmd.Upgrade(wish);
                }

                await CardPileCmd.AddGeneratedCardToCombat(
                    wish,
                    PileType.Hand,
                    selectedPlayer
                );

                PileType.Hand
                    .GetPile(selectedPlayer)
                    .InvokeCardAddFinished();

                break;

            case 4:
                CardModel apotheosis =
                    CombatState.CreateCard<Apotheosis>(
                        selectedPlayer
                    );

                if (IsUpgraded)
                {
                    CardCmd.Upgrade(apotheosis);
                }

                await CardPileCmd.AddGeneratedCardToCombat(
                    apotheosis,
                    PileType.Hand,
                    selectedPlayer
                );

                PileType.Hand
                    .GetPile(selectedPlayer)
                    .InvokeCardAddFinished();

                break;

            case 5:
                CardModel apparition =
                    CombatState.CreateCard<Apparition>(
                        selectedPlayer
                    );

                if (IsUpgraded)
                {
                    CardCmd.Upgrade(apparition);
                }

                await CardPileCmd.AddGeneratedCardToCombat(
                    apparition,
                    PileType.Hand,
                    selectedPlayer
                );

                PileType.Hand
                    .GetPile(selectedPlayer)
                    .InvokeCardAddFinished();

                break;

            case 6:
                CardModel abundance =
                    CombatState.CreateCard<Abundance>(
                        selectedPlayer
                    );

                if (IsUpgraded)
                {
                    CardCmd.Upgrade(abundance);
                }

                await CardPileCmd.AddGeneratedCardToCombat(
                    abundance,
                    PileType.Hand,
                    selectedPlayer
                );

                PileType.Hand
                    .GetPile(selectedPlayer)
                    .InvokeCardAddFinished();

                break;
        }
    }

    protected override void OnUpgrade()
    {
        
    }
}