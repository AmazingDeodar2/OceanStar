using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;

namespace starss.starssCode.Patches;

[HarmonyPatch(typeof(ActModel), nameof(ActModel.GenerateRooms))]
public static class FirstActAncientPoolPatch
{
    [HarmonyPrefix]
    [HarmonyBefore("BaseLib")]
    public static void Prefix(
        ActModel __instance,
        ref List<AncientEventModel>? ____sharedAncientSubset)
    {
        // 同时支持密林和暗港
        if (__instance.Index != 0)
            return;

        // 仅在未初始化时创建，不覆盖已有内容
        if (____sharedAncientSubset == null)
        {
            ____sharedAncientSubset = new List<AncientEventModel>();
        }
    }
}