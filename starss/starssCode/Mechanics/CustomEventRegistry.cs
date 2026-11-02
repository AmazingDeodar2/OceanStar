using Godot;
using MegaCrit.Sts2.Core.Models;
using starss.starssCode.Events;
using System;

namespace starss.starssCode.Mechanics;

public static class CustomEventRegistry
{
    public static void Register()
    {
        Type eventType = typeof(LeakySpoonCircle);

        // 如果尚未注册，则注入 ModelDb
        if (!ModelDb.Contains(eventType))
        {
            ModelDb.Inject(eventType);
        }

        GD.Print(
            "[starss] LeakySpoonCircle registered: " +
            ModelDb.Contains(eventType)
        );

        GD.Print(
            "[starss] LeakySpoonCircle ModelId: " +
            ModelDb.GetId(eventType)
        );
    }
}