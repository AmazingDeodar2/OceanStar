using Godot;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models.RelicPools;
using starss.starssCode.Relics;

namespace starss.starssCode;

public static class StarssRelicRegistration
{
    private static bool _registered;

    public static void Register()
    {
        if (_registered)
            return;

        ModHelper.AddModelToPool<EventRelicPool, DivineTouch>();
        ModHelper.AddModelToPool<EventRelicPool, OriginAltar>();
        ModHelper.AddModelToPool<EventRelicPool, ShapeshiftingGate>();

        ModHelper.AddModelToPool<EventRelicPool, MarkedCard>();
        ModHelper.AddModelToPool<EventRelicPool, CreepingVine>();
        ModHelper.AddModelToPool<EventRelicPool, StableStone>();

        ModHelper.AddModelToPool<EventRelicPool, MetamorphosisPact>();
        ModHelper.AddModelToPool<EventRelicPool, GoldenCrucible>();
        ModHelper.AddModelToPool<EventRelicPool, FishingNet>();
        ModHelper.AddModelToPool<EventRelicPool, GreenGodHeart>();

        _registered = true;

        GD.Print("[starss] Registered 10 Ancient relics to EventRelicPool.");
    }
}