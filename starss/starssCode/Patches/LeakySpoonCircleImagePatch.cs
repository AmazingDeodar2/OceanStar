using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Models;
using starss.starssCode.Events;

namespace starss.starssCode.Patches;

[HarmonyPatch(typeof(EventModel), nameof(EventModel.GetAssetPaths))]
public static class LeakySpoonCircleImagePatch
{
    private const string OriginalPath =
        "res://images/events/leaky_spoon_circle.png";

    private const string CustomPath =
        "res://starss/images/events/leaky_spoon_circle.png";

    [HarmonyPostfix]
    private static void Postfix(
        EventModel __instance,
        ref IEnumerable<string> __result)
    {
        if (__instance is not LeakySpoonCircle)
            return;

        __result = __result.Select(path =>
            path == OriginalPath ? CustomPath : path
        ).ToArray();
    }
}

[HarmonyPatch(typeof(EventModel), nameof(EventModel.CreateInitialPortrait))]
public static class LeakySpoonCirclePortraitPatch
{
    private const string CustomPath =
        "res://starss/images/events/leaky_spoon_circle.png";

    [HarmonyPrefix]
    private static bool Prefix(
        EventModel __instance,
        ref Texture2D __result)
    {
        if (__instance is not LeakySpoonCircle)
            return true;

        __result = PreloadManager.Cache.GetTexture2D(CustomPath);

        return false;
    }
}