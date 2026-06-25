using MegaCrit.Sts2.Core.Entities.Creatures;
using starss.starssCode.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace starss.starssCode.Mechanics;


public readonly record struct DiceRollResult(int Value, IReadOnlyList<int> Rolls);

public readonly record struct DiceCheckResult(
    int Roll,
    int Fate,
    int Doom,
    bool FateSuccess,
    bool DoomSuccess,
    DiceRollResult RollResult
);
public static class DiceHelper
{
    public static async Task<DiceCheckResult> Check(
        Creature creature,
        int fate,
        int doom,
        PlayerChoiceContext? choiceContext = null,
        CardModel? sourceCard = null,
        bool showUi = true,
        bool consumeTemporaryLuck = true)
    {
        var rollResult = RollD100(creature);

        if (showUi)
            await DiceUi.ShowRoll(rollResult);

        var finalFate = ApplyFate(fate, creature);
        var finalDoom = ApplyDoom(doom, creature);

        var result = new DiceCheckResult(
            rollResult.Value,
            finalFate,
            finalDoom,
            rollResult.Value <= finalFate,
            rollResult.Value >= finalDoom,
            rollResult
        );
        if (result.FateSuccess)
            await OnFateTriggered(creature);
        
        if (result.FateSuccess)
            await OnFateTriggered(creature);
        
        if (result.DoomSuccess && choiceContext != null && sourceCard != null)
            await OnDoomTriggered(choiceContext, sourceCard);
        
        if (consumeTemporaryLuck)
            await ConsumeNextCheckLuck(creature);

        return result;
    }
    public static DiceRollResult RollD100(Creature creature)
    {
        return RollDice(creature, 100);
    }

    public static DiceRollResult RollD6(Creature creature)
    {
        return RollDice(creature, 6);
    }

    private static DiceRollResult RollDice(Creature creature, int sides)
    {
        var rewardDice = GetRewardDice(creature);

        // 基础投1次；每层奖励骰额外投1次。
        var rollCount = 1 + rewardDice;

        var rolls = new List<int>();

        for (var i = 0; i < rollCount; i++)
            rolls.Add(Random.Shared.Next(1, sides + 1));

        // 奖励骰：取最低值。
        return new DiceRollResult(rolls.Min(), rolls);
    }

    public static int GetLuck(Creature creature)
    {
        var permanentLuck = creature.GetPower<LuckyPower>()?.Amount ?? 0M;
        var temporaryLuck = creature.GetPower<NextCheckLuckPower>()?.Amount ?? 0M;

        return (int)(permanentLuck + temporaryLuck);
    }

    public static int GetRewardDice(Creature creature)
    {
        var rewardDice = creature.GetPower<RewardDicePower>();
        return rewardDice == null ? 0 : (int)rewardDice.Amount;
    }

    public static int ApplyFate(int baseFate, Creature creature)
    {
        return baseFate + GetLuck(creature);
    }

    public static int ApplyDoom(int baseDoom, Creature creature)
    {
        return baseDoom;
    }
    
    public static async Task ConsumeNextCheckLuck(Creature creature)
    {
        var tempLuck = creature.GetPower<NextCheckLuckPower>();
        if (tempLuck == null)
            return;

        await PowerCmd.Remove(tempLuck);
    }
    
    public static async Task OnFateTriggered(Creature creature)
    {
        var power = creature.GetPower<FearlessFatePower>();
        if (power == null)
            return;

        await CreatureCmd.GainBlock(
            creature,
            power.Amount,
            ValueProp.Unpowered,
            null
        );
    }
    
    public static async Task OnDoomTriggered(
        PlayerChoiceContext choiceContext,
        CardModel sourceCard)
    {
        var creature = sourceCard.Owner.Creature;
        var power = creature.GetPower<SharedMisfortunePower>();

        if (power == null)
            return;

        await DamageCmd.Attack(power.Amount)
            .FromCard(sourceCard)
            .TargetingAllOpponents(sourceCard.CombatState!)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);
    }
}