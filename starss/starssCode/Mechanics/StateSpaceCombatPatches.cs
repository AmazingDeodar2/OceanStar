using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;

namespace starss.starssCode.Mechanics;

public static class StateSpaceCombatPatches
{
    [HarmonyPatch(typeof(PlayerCombatState), MethodType.Constructor, typeof(Player))]
    public static class PlayerCombatStateConstructorPatch
    {
        public static void Postfix(Player player)
        {
            StateRegistry.Get(player);
        }
    }

    [HarmonyPatch(typeof(PlayerCombatState), nameof(PlayerCombatState.AfterCombatEnd))]
    public static class PlayerCombatStateAfterCombatEndPatch
    {
        public static void Prefix(PlayerCombatState __instance)
        {
            StateRegistry.ClearAll();
            StateUi.Clear();
        }
    }
}