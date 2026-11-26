using System;
using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;

namespace starss.starssCode.Events;

[HarmonyPatch(typeof(ModelDb), nameof(ModelDb.Preload))]
public static class EventRegistrationDebugPatch
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        try
        {
            Type eventType = typeof(LeakySpoonCircle);

            bool registered = ModelDb.Contains(eventType);

            MainFile.Logger.Info(
                $"[starss][event] ModelDb.Contains = {registered}"
            );

            if (registered)
            {
                var model = ModelDb.Event<LeakySpoonCircle>();

                MainFile.Logger.Info(
                    $"[starss][event] ModelId = {model.Id}"
                );
            }

            bool inAllEvents = ModelDb.AllEvents
                .Any(e => e is LeakySpoonCircle);

            MainFile.Logger.Info(
                $"[starss][event] AllEvents.Contains = {inAllEvents}"
            );

            bool inSharedEvents = ModelDb.AllSharedEvents
                .Any(e => e is LeakySpoonCircle);

            MainFile.Logger.Info(
                $"[starss][event] AllSharedEvents.Contains = {inSharedEvents}"
            );
        }
        catch (Exception ex)
        {
            MainFile.Logger.Error(
                $"[starss][event] 注册状态检查失败：{ex}"
            );
        }
    }
}