using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Ancients;
using MegaCrit.Sts2.Core.Models;
using starss.starssCode.Character;

namespace starss.starssCode.Patches;

[HarmonyPatch(
    typeof(AncientDialogueSet),
    nameof(AncientDialogueSet.GetValidDialogues)
)]
public static class StarssAncientDialogueFilterPatch
{
    [HarmonyPostfix]
    private static void Postfix(
        AncientDialogueSet __instance,
        ModelId characterId,
        ref IEnumerable<AncientDialogue> __result)
    {
        string starssId =
            ModelDb.Character<Starss>().Id.Entry;

        if (characterId.Entry != starssId)
            return;

        if (!__instance.CharacterDialogues.TryGetValue(
                starssId,
                out IReadOnlyList<AncientDialogue>? starssDialogues))
        {
            return;
        }

        HashSet<AncientDialogue> starssSet =
            starssDialogues.ToHashSet();

        List<AncientDialogue> filtered =
            __result
                .Where(starssSet.Contains)
                .ToList();

        // 只有确实找到专属对白时才覆盖结果，
        // 避免某个访问次数没有专属对白时出现空对话。
        if (filtered.Count > 0)
            __result = filtered;
    }
}