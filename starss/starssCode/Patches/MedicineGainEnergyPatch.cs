using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Hooks;
using starss.starssCode.Powers;
using System;
using System.Threading.Tasks;

namespace starss.starssCode.Mechanics;

[HarmonyPatch(typeof(PlayerCmd), nameof(PlayerCmd.GainEnergy))]
public static class MedicineGainEnergyPatch
{
    [HarmonyPrefix]
    public static bool Prefix(
        decimal amount,
        Player player,
        ref Task __result)
    {
        if (player.Creature.GetPower<MedicinePower>() == null)
            return true;

        __result = GainHealthInstead(amount, player);
        return false;
    }

    private static async Task GainHealthInstead(
        decimal amount,
        Player player)
    {
        if (amount <= 0M || CombatManager.Instance.IsEnding)
            return;

        var combatState = player.Creature.CombatState;

        // 保留原版能量获取修正。
        var finalAmount = Hook.ModifyEnergyGain(
            combatState,
            player,
            amount,
            out var modifiers
        );

        await Hook.AfterModifyingEnergyGain(
            combatState,
            modifiers
        );

        if (finalAmount <= 0M)
            return;

        // 之前讨论过的 CreatureCmd.Heal 重载。
        // 请以当前 sts2.dll 的实际签名为准。
        await CreatureCmd.Heal(
            player.Creature,
            finalAmount,
            false
        );
    }
}