using System.Collections.Generic;
using System.Linq;

using HarmonyLib;

using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;

namespace starss.starssCode.Patches;

[HarmonyPatch(typeof(ActModel), nameof(ActModel.GenerateRooms))]
public static class FirstActAncientPoolPatch
{
    // 第一阶段：确保第一幕有共享先古之民池
    [HarmonyPrefix]
    [HarmonyPriority(Priority.First)]
    public static void Prefix(
        ActModel __instance,
        ref List<AncientEventModel>? ____sharedAncientSubset)
    {
        if (__instance.Index != 0)
            return;

        ____sharedAncientSubset ??= new List<AncientEventModel>();
    }

    // 第二阶段：调整涅奥权重
    [HarmonyPrefix]
    [HarmonyPriority(Priority.Last)]
    public static void AdjustWeights(
        ActModel __instance,
        List<AncientEventModel>? ____sharedAncientSubset)
    {
        if (__instance.Index != 0 ||
            ____sharedAncientSubset == null)
            return;

        // 获取已经注册的原版涅奥
        var neow = __instance.AllAncients
            .OfType<Neow>()
            .FirstOrDefault();

        if (neow == null)
            return;

        // 防止重复添加
        if (____sharedAncientSubset.Count(
                ancient => ancient is Neow) >= 2)
            return;

        // 直接重复引用已有模型，不调用构造函数
        ____sharedAncientSubset.Add(neow);
        ____sharedAncientSubset.Add(neow);
    }
}