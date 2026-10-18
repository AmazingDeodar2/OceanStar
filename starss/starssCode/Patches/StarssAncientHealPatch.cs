using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using starss.starssCode;
using Godot;
using starss.starssCode.Ancients;

namespace starss.starssCode.Patches;

[HarmonyPatch(typeof(AncientEventModel), "BeforeEventStarted")]
public static class StarssAncientHealPatch
{
    [HarmonyPrefix]
    private static void Prefix(
        AncientEventModel __instance,
        bool isPreFinished)
    {
        if (isPreFinished)
            return;

        if (__instance is not StarssAncient)
            return;

        var creature = __instance.Owner?.Creature;
        if (creature == null)
            return;

        creature.SetCurrentHpInternal(0M);

        GD.Print(
            "[starss Ancient] HP reset before healing. " +
            $"MaxHP={creature.MaxHp}"
        );
    }
}