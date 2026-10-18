using System.Linq;
using BaseLib.Patches.Content;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using starss.starssCode.Ancients;

namespace starss.starssCode.Patches;

[HarmonyPatch(typeof(ActModel), "GenerateRooms")]
public static class AncientSpawnDebugPatch
{
    [HarmonyPrefix]
    [HarmonyPriority(Priority.Last)]
    public static void Prefix(
        ActModel __instance,
        System.Collections.Generic.List<AncientEventModel>? ____sharedAncientSubset)
    {
        if (__instance.Index != 0)
            return;

        bool registered = CustomContentDictionary.CustomAncients
            .Any(a => a is StarssAncient);

        bool inPool = ____sharedAncientSubset?
            .Any(a => a is StarssAncient) ?? false;

        MainFile.Logger.Info(
            $"[starss][ancient] 第一幕生成: " +
            $"Registered={registered}, " +
            $"InSharedPool={inPool}, " +
            $"PoolCount={____sharedAncientSubset?.Count ?? -1}"
        );
    }
}