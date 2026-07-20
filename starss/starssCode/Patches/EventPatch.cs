// using System;
// using System.Collections.Generic;
// using System.Linq;
// using Godot;
// using HarmonyLib;
// using MegaCrit.Sts2.Core.Events;
// using MegaCrit.Sts2.Core.Models;
// using MegaCrit.Sts2.Core.Models.Acts;
// using starss.starssCode.Events;
//
// namespace starss.starssCode.Patches;
//
// [HarmonyPatch(
//     typeof(Overgrowth),
//     nameof(Overgrowth.AllEvents),
//     MethodType.Getter
// )]
// public static class OvergrowthAllEventsPatch
// {
//     [HarmonyPostfix]
//     public static void Postfix(ref IEnumerable<EventModel> __result)
//     {
//         GD.Print("[starss] Overgrowth.AllEvents Postfix 已执行");
//
//         try
//         {
//             EventModel friendshipMagic =
//                 ModelDb.Event<FriendshipMagic>();
//
//             GD.Print(
//                 $"[starss] FriendshipMagic 已从 ModelDb 获取，ID：{friendshipMagic.Id}"
//             );
//
//             List<EventModel> events = __result.ToList();
//
//             GD.Print(
//                 $"[starss] 添加前事件数量：{events.Count}"
//             );
//
//             if (events.All(e => e.GetType() != typeof(FriendshipMagic)))
//                 events.Add(friendshipMagic);
//
//             __result = events;
//
//             GD.Print(
//                 $"[starss] 添加后事件数量：{events.Count}，" +
//                 $"是否包含 FriendshipMagic：{events.Any(e => e.GetType() == typeof(FriendshipMagic))}"
//             );
//         }
//         catch (Exception exception)
//         {
//             GD.PrintErr(
//                 $"[starss] 注册 FriendshipMagic 失败：{exception}"
//             );
//         }
//     }
// }