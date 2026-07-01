using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Ancients;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using starss.starssCode.Character;

namespace starss.starssCode.Patches;

[HarmonyPatch(typeof(TheArchitect), "DefineDialogues")]
public static class TheArchitectDialoguePatch
{
    public static void Postfix(AncientDialogueSet __result)
    {
        string starssKey = ModelDb.Character<Starss>().Id.Entry;

        if (__result.CharacterDialogues.ContainsKey(starssKey))
            return;

        __result.CharacterDialogues[starssKey] =
        [
            new AncientDialogue(["", "", ""])
            {
                VisitIndex = 0,
                EndAttackers = ArchitectAttackers.Both
            },

            new AncientDialogue(["", "", ""])
            {
                VisitIndex = 1,
                EndAttackers = ArchitectAttackers.Both
            },

            new AncientDialogue(["", "", ""])
            {
                VisitIndex = 2,
                EndAttackers = ArchitectAttackers.Both
            }
        ];
    }
}