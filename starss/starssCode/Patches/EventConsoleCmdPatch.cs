using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using HarmonyLib;

using MegaCrit.Sts2.Core.DevConsole.ConsoleCommands;
using MegaCrit.Sts2.Core.Models;

using starss.starssCode.Events;

namespace starss.starssCode.Patches;

/// <summary>
/// 只让 LeakySpoonCircle 出现在开发者控制台的 event 命令中。
///
/// 不修改 ModelDb.AllEvents。
/// 不修改任何章节的随机事件池。
/// 不创建新的 EventModel 实例。
/// </summary>
[HarmonyPatch]
public static class EventConsoleCmdPatch
{
    // EventConsoleCmd.Events 是 private static 属性，
    // 因此通过 AccessTools 获取其 getter。
    private static MethodBase TargetMethod()
    {
        return AccessTools.PropertyGetter(
            typeof(EventConsoleCmd),
            "Events"
        ) ?? throw new MissingMethodException(
            "EventConsoleCmd.Events getter not found"
        );
    }

    [HarmonyPostfix]
    private static void Postfix(
        ref IEnumerable<EventModel> __result
    )
    {
        // 只读取 ModelDb 中已经初始化的规范实例。
        // 不使用 Inject，也不使用 new。
        if (!ModelDb.Contains(typeof(LeakySpoonCircle)))
            return;

        EventModel customEvent =
            ModelDb.Event<LeakySpoonCircle>();

        // 避免重复添加同一个事件。
        if (__result.Any(e => e.Id == customEvent.Id))
            return;

        __result = __result.Append(customEvent);
    }
}