
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using starss.starssCode.Powers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace starss.starssCode.Mechanics;

[HarmonyPatch(typeof(CardModel), nameof(CardModel.SpendResources))]
public static class MedicineSpendResourcesPatch
{
    private static readonly MethodInfo SpendStarsMethod =
        AccessTools.Method(typeof(CardModel), "SpendStars")
        ?? throw new MissingMethodException(
            "CardModel.SpendStars not found"
        );

    [HarmonyPrefix]
    public static bool Prefix(
        CardModel __instance,
        ref Task<(int, int)> __result)
    {
        if (__instance.Owner.Creature.GetPower<MedicinePower>() == null)
            return true;

        __result = SpendWithHealth(__instance);
        return false;
    }

    private static async Task<(int, int)> SpendWithHealth(
        CardModel card)
    {
        int energyToSpend = Math.Max(
            0,
            card.EnergyCost.GetAmountToSpend()
        );

        int starsToSpend = Math.Max(
            0,
            card.GetStarCostWithModifiers()
        );

        // X 费牌必须保存支付时的 X 值。
        if (card.EnergyCost.CostsX)
        {
            card.EnergyCost.CapturedXValue = energyToSpend;
        }

        // 不调用原版 LoseEnergy。
        // 用生命支付费用，允许支付致死。
        if (energyToSpend > 0)
        {
            await PayHealth(card, energyToSpend);
        }

        // 保留原版能量支付后的事件。
        await Hook.AfterEnergySpent(
            card.CombatState,
            card,
            energyToSpend
        );

        // 正常支付星星。
        var starsTask = (Task)SpendStarsMethod.Invoke(
            card,
            new object[] { starsToSpend }
        )!;

        await starsTask;

        return (energyToSpend, starsToSpend);
    }

    private static async Task PayHealth(
        CardModel card,
        int amount)
    {
        Creature creature = card.Owner.Creature;

        if (amount <= 0 || creature.IsDead)
            return;

        // 直接失去生命，不经过格挡。
        var result = creature.LoseHpInternal(
            amount,
            ValueProp.Unblockable | ValueProp.Unpowered
        );

        if (result.UnblockedDamage > 0)
        {
            await Hook.AfterCurrentHpChanged(
                card.RunState,
                creature.CombatState,
                creature,
                -result.UnblockedDamage
            );
        }

        // 按原版死亡流程处理。
        if (result.WasTargetKilled && creature.IsDead)
        {
            await CreatureCmd.Kill(
                (IReadOnlyCollection<Creature>)
                new List<Creature> { creature }
            );
        }
    }
}
