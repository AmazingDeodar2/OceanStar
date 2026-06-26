using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;

namespace starss.starssCode.Mechanics.Patches;

[HarmonyPatch(typeof(Hook), nameof(Hook.AfterCardGeneratedForCombat))]
public static class StateAfterCardGeneratedPatch
{
    public static async void Postfix(
        ICombatState combatState,
        CardModel card,
        Player? creator)
    {
        await StateCmd.AfterCardGeneratedForCombat(
            card,
            creator);
    }
}