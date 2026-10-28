using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using starss.starssCode.Powers;
using System;

namespace starss.starssCode.Mechanics;

[HarmonyPatch(
    typeof(PlayerCombatState),
    nameof(PlayerCombatState.HasEnoughResourcesFor)
)]
public static class MedicineResourcePatch
{
    [HarmonyPrefix]
    public static bool Prefix(
        CardModel card,
        ref UnplayableReason reason,
        ref bool __result)
    {
        if (card.Owner.Creature.GetPower<MedicinePower>() == null)
            return true;

        // 医学期间，忽略能量是否足够。
        // 星星仍需正常支付。
        int starsToSpend = Math.Max(
            0,
            card.GetStarCostWithModifiers()
        );

        reason = UnplayableReason.None;

        if (starsToSpend > card.Owner.PlayerCombatState.Stars)
            reason |= UnplayableReason.StarCostTooHigh;

        __result = reason == UnplayableReason.None;
        return false;
    }
}