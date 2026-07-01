using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using starss.starssCode.Relics;

namespace starss.starssCode.Patches;

[HarmonyPatch(typeof(TouchOfOrobas), "GetUpgradedStarterRelic")]
public static class TouchOfOrobasStarssPatch
{
    public static bool Prefix(
        RelicModel starterRelic,
        ref RelicModel __result)
    {
        if (starterRelic.Id != ModelDb.Relic<StarssStarterRelic>().Id)
            return true;

        __result = ModelDb.Relic<FateDiceRelic>();
        return false;
    }
}