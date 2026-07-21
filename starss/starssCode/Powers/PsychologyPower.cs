using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace starss.starssCode.Powers;


public sealed class PsychologyPower : starssPower
{
    private CardModel? _triggeringCard;
    private List<PowerModel>? _doubledPowers;

    public override PowerType Type
        => PowerType.Debuff;

    public override PowerStackType StackType
        => PowerStackType.Single;

    /*
     * 不同玩家使用《心理学》时，需要保留独立实例，
     * 因为每个实例记录的 Applier 不同。
     */
    public override PowerInstanceType InstanceType
        => PowerInstanceType.Instanced;

    private CardModel? TriggeringCard
    {
        get => _triggeringCard;

        set
        {
            AssertMutable();
            _triggeringCard = value;
        }
    }

    private List<PowerModel> DoubledPowers
    {
        get
        {
            AssertMutable();

            _doubledPowers ??= [];

            return _doubledPowers;
        }
    }

    public override Task AfterApplied(
        Creature? applier,
        CardModel? cardSource)
    {
        TriggeringCard = null;
        DoubledPowers.Clear();

        return Task.CompletedTask;
    }

    /*
     * 在负面效果的数量正式变化之前，判断这次效果是否应当翻倍。
     *
     * 这里主要用于：
     * 1. 记录触发翻倍的卡；
     * 2. 记录复合临时能力；
     * 3. 防止 PiercingWail 一类效果重复翻倍。
     */
    public override Task BeforePowerAmountChanged(
        PowerModel power,
        decimal amount,
        Creature target,
        Creature? applier,
        CardModel? cardSource)
    {
        if (TriggeringCard != null)
            return Task.CompletedTask;

        if (cardSource == null)
            return Task.CompletedTask;

        if (applier == null || Applier == null)
            return Task.CompletedTask;

        // 必须施加到《心理学》标记的敌人身上
        if (target != Owner)
            return Task.CompletedTask;

        // 使用《心理学》的玩家自己施加时不翻倍
        if (applier == Applier)
            return Task.CompletedTask;

        // 必须是《心理学》使用者的队友
        if (applier.Side != Applier.Side)
            return Task.CompletedTask;

        // 隐藏能力不视为正常负面效果
        if (!power.IsVisible)
            return Task.CompletedTask;

        if (power.GetTypeForAmount(amount) != PowerType.Debuff)
            return Task.CompletedTask;

        TriggeringCard = cardSource;
        DoubledPowers.Add(power);

        return Task.CompletedTask;
    }

    public override decimal ModifyPowerAmountGivenMultiplicative(
        PowerModel power,
        Creature giver,
        decimal amount,
        Creature? target,
        CardModel? cardSource)
    {
        if (TriggeringCard == null)
            return 1M;

        if (cardSource != TriggeringCard)
            return 1M;

        if (target != Owner)
            return 1M;

        if (giver == Applier)
            return 1M;

        if (Applier == null || giver.Side != Applier.Side)
            return 1M;

        if (power.GetTypeForAmount(amount) != PowerType.Debuff)
            return 1M;

        // /*
        //  * 如果已经翻倍了一个复合临时能力，
        //  * 不再翻倍它内部产生的负面能力，避免变成四倍。
        //  */
        // if (HasDoubledTemporaryPowerSource(power))
        //     return 1M;

        return 2M;
    }

    public override Task AfterCardPlayed(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        if (cardPlay.Card != TriggeringCard)
            return Task.CompletedTask;

        Flash();

        TriggeringCard = null;
        DoubledPowers.Clear();

        return Task.CompletedTask;
    }

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        /*
         * 参照 FlankingPower：
         * 目标敌人所在阵营的回合结束后移除。
         */
        if (!participants.Contains(Owner))
            return;

        await PowerCmd.Remove(this);
    }

    private bool HasDoubledTemporaryPowerSource(PowerModel power)
    {
        return DoubledPowers
            .OfType<ITemporaryPower>()
            .Any(temporaryPower =>
                temporaryPower.InternallyAppliedPower.GetType()
                == power.GetType()
            );
    }
}