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
using starss.starssCode.Cards;
using starss.starssCode.Relics;

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
        var rollResult = RollD100(creature, sourceCard);

        if (showUi)
            await DiceUi.ShowRoll(rollResult);

        var fateEnabled = fate > 0 && fate <= 100;
        var doomEnabled = doom > 0 && doom <= 100;

        var finalFate = fateEnabled ? ApplyFate(fate, creature) : fate;
        var finalDoom = doomEnabled ? ApplyDoom(doom, creature) : doom;

        var result = new DiceCheckResult(
            rollResult.Value,
            finalFate,
            finalDoom,
            fateEnabled && rollResult.Value <= finalFate,
            doomEnabled && rollResult.Value >= finalDoom,
            rollResult
        );
        if (result.FateSuccess && choiceContext != null && sourceCard != null)
            await OnFateTriggered(choiceContext, sourceCard);

        if (result.DoomSuccess && choiceContext != null && sourceCard != null)
            await OnDoomTriggered(choiceContext, sourceCard);

        if (consumeTemporaryLuck)
            await ConsumeNextCheckLuck(creature);

        return result;
    }

    public static DiceRollResult RollD100(Creature creature, CardModel? sourceCard = null)
    {
        return RollDice(creature, 100, sourceCard);
    }

    public static DiceRollResult RollD6(Creature creature, CardModel? sourceCard = null)
    {
        return RollDice(creature, 6, sourceCard);
    }
    
    public static DiceRollResult RollD3(Creature creature, CardModel? sourceCard = null)
    {
        return RollDice(creature, 3, sourceCard);
    }
    
    public static DiceRollResult RollD10(Creature creature, CardModel? sourceCard = null)
    {
        return RollDice(creature, 10, sourceCard);
    }
    
    public static DiceRollResult RollD20(Creature creature, CardModel? sourceCard = null)
    {
        return RollDice(creature, 20, sourceCard);
    }

    private static DiceRollResult RollDice(Creature creature, int sides, CardModel? sourceCard)
    {
        var rewardDice = GetRewardDice(creature);

        // 基础投1次；每层奖励骰额外投1次。
        var rollCount = 1 + rewardDice;

        var rolls = new List<int>();
        var rng = sourceCard?.Owner.RunState.Rng.Niche
                  ?? creature.CombatState.RunState.Rng.Niche;

        for (var i = 0; i < rollCount; i++)
        {
            rolls.Add((int)rng.NextFloat(sides) + 1);
        }

        var modifiedRolls = rolls
            .Select(roll => StateCmd.ModifyDiceRoll(creature, sourceCard, roll))
            .ToList();
        // 奖励骰：取最低值。
        var value = modifiedRolls.Min();

        var blackForm = creature.GetPower<BlackFormPower>();
        if (blackForm != null)
            value = RemapRoll(value, 51);

        return new DiceRollResult(value, modifiedRolls);
    }

    private static int RemapRoll(int roll, int minRoll)
    {
        return minRoll + (roll - 1) * (101 - minRoll) / 100;
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
        return baseDoom + GetLuck(creature)/2;
    }

    public static async Task ConsumeNextCheckLuck(Creature creature)
    {
        var tempLuck = creature.GetPower<NextCheckLuckPower>();
        if (tempLuck == null)
            return;

        await PowerCmd.Remove(tempLuck);
    }

    public static async Task OnFateTriggered(
        PlayerChoiceContext choiceContext,
        CardModel sourceCard)
    {
        var creature = sourceCard.Owner.Creature;

        var power = creature.GetPower<FearlessFatePower>();

        if (power != null)
        {
            await CreatureCmd.GainBlock(
                creature,
                power.Amount,
                ValueProp.Unpowered,
                null
            );
        }

        await FateRelicHelper.OnFateTriggered(
            choiceContext,
            sourceCard.Owner
        );
        
        await TriggerFateCards(
            choiceContext,
            sourceCard
        );
    }

    public static async Task OnDoomTriggered(
        PlayerChoiceContext choiceContext,
        CardModel sourceCard)
    {
        var creature = sourceCard.Owner.Creature;
        var power = creature.GetPower<SharedMisfortunePower>();

        if (power != null)
        {
            var combatState = sourceCard.CombatState;
            if (combatState != null)
            {
                var target = sourceCard.Owner.RunState.Rng.CombatTargets
                    .NextItem(combatState.HittableEnemies);

                if (target != null)
                {
                    VfxCmd.PlayOnCreatureCenter(
                        target,
                        "vfx/vfx_attack_blunt"
                    );

                    await CreatureCmd.Damage(
                        choiceContext,
                        target,
                        power.Amount,
                        ValueProp.Unpowered,
                        creature
                    );
                }
            }
        }

        var blessing = creature.GetPower<MisfortuneBlessingPower>();

        if (blessing != null)
        {
            await PlayerCmd.GainEnergy(1M, sourceCard.Owner);

            await CardPileCmd.Draw(
                choiceContext,
                1M,
                sourceCard.Owner
            );

            await PowerCmd.Remove(blessing);
        }
        
        var relic = sourceCard.Owner.GetRelic<EyebrowPencil>();

        if (relic != null)
        {
            relic.Flash();

            await CreatureCmd.GainBlock(
                creature,
                3M,
                ValueProp.Unpowered,
                null
            );
        }
    }
    
    private static async Task TriggerFateCards(
        PlayerChoiceContext choiceContext,
        CardModel sourceCard)
    {
        foreach (var card in sourceCard.Owner.PlayerCombatState.AllCards)
        {
            if (card is FateStrike fateStrike)
                await fateStrike.OnFateTriggered(choiceContext);
        }
    }
}