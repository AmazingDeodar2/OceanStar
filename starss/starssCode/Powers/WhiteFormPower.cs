using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace starss.starssCode.Powers;

public sealed class WhiteFormPower : starssPower
{
    private int _cardsPlayedThisTurn;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardPlayed(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        // 只统计能力拥有者打出的牌
        if (cardPlay.Card.Owner.Creature != Owner)
            return;

        // 自动打出的牌不计数
        if (cardPlay.IsAutoPlay)
            return;

        // 多段打出只在整个系列结束时计作一张
        if (!cardPlay.IsLastInSeries)
            return;

        _cardsPlayedThisTurn++;

        // 每回合只有前三张牌触发
        if (_cardsPlayedThisTurn > 3)
            return;

        Flash();

        await CardPileCmd.Draw(
            choiceContext,
            Amount,
            Owner.Player
        );
    }

    public override Task BeforeSideTurnStart(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        // 只在包含能力拥有者的阵营回合开始时重置
        if (!participants.Contains(Owner))
            return Task.CompletedTask;

        _cardsPlayedThisTurn = 0;

        return Task.CompletedTask;
    }
}