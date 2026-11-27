
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
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
        // 没有 MedicinePower 时，使用原版资源支付逻辑。
        if (__instance.Owner.Creature.GetPower<MedicinePower>() == null)
            return true;

        // 存在 MedicinePower 时，使用生命代替能量支付。
        __result = SpendWithHealth(__instance);

        return false;
    }

    private static async Task<(int, int)> SpendWithHealth(
        CardModel card)
    {
        // 获取实际需要支付的能量。
        int energyToSpend = Math.Max(
            0,
            card.EnergyCost.GetAmountToSpend()
        );

        // 获取实际需要支付的星星。
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

        // 无需支付或角色已经死亡时，不进行处理。
        if (amount <= 0 || creature.IsDead)
            return;

        // 直接失去生命，不经过格挡。
        // v0.114.0：LoseHpInternal(decimal amount, bool dryRun = false): HpLossResult（Creature.cs:458），
        // 返回值的 hpLost 字段就是实际扣血量（HpLossResult.cs:5）。官方 Kill 内同款写法 CreatureCmd.cs:730-731。
        var result = creature.LoseHpInternal(
            amount,
            dryRun: false
        );

        var hpLost = result.hpLost;

        // 触发生命值变化事件。
        if (hpLost > 0)
        {
            await Hook.AfterCurrentHpChanged(
                card.RunState,
                creature.CombatState,
                creature,
                -hpLost
            );
        }

        // 按原版死亡流程处理。
        if (creature.IsDead)
        {
            await CreatureCmd.Kill(
                (IReadOnlyCollection<Creature>)
                new List<Creature> { creature }
            );
        }
    }
}
