using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Models;

namespace starss.starssCode.Powers;


public sealed class GodhoodPower : PowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount =>
        Math.Max(0, Amount - GetInternalData<Data>().zeroCostSkillsPlayed);

    protected override object InitInternalData() => new Data();

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        SetZeroCostSkillsPlayed(
            CombatManager.Instance.History.Entries
                .OfType<CardPlayStartedEntry>()
                .Count(e =>
                    e.CardPlay.Card.Type == CardType.Skill &&
                    e.CardPlay.Card.Owner.Creature == Owner &&
                    e.CardPlay.Resources.EnergyValue == 0 &&
                    e.HappenedThisTurn(CombatState))
        );

        return Task.CompletedTask;
    }

    public override (PileType, CardPilePosition) ModifyCardPlayResultPileTypeAndPosition(
        CardModel card,
        bool isAutoPlay,
        ResourceInfo resources,
        PileType pileType,
        CardPilePosition position)
    {
        if (card.Owner.Creature != Owner)
            return (pileType, position);

        if (card.Type != CardType.Skill)
            return (pileType, position);

        if (resources.EnergyValue > 0)
            return (pileType, position);

        if (GetInternalData<Data>().zeroCostSkillsPlayed >= Amount)
            return (pileType, position);

        return (PileType.Hand, CardPilePosition.Top);
    }

    public override Task AfterModifyingCardPlayResultPileOrPosition(
        CardModel card,
        PileType pileType,
        CardPilePosition position)
    {
        Flash();
        SetZeroCostSkillsPlayed(GetInternalData<Data>().zeroCostSkillsPlayed + 1);
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }

    public override Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains(Owner))
            return Task.CompletedTask;

        SetZeroCostSkillsPlayed(0);
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }

    private void SetZeroCostSkillsPlayed(int value)
    {
        GetInternalData<Data>().zeroCostSkillsPlayed = value;
        InvokeDisplayAmountChanged();
    }

    private class Data
    {
        public int zeroCostSkillsPlayed;
    }
}