using System;
using System.Runtime.CompilerServices;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace starss.starssCode.Patches;

/// <summary>
/// starss 的战斗立绘没有 spine，Body(%Visuals) 是 AnimatedSprite2D 帧动画。
/// 游戏侧的动作 trigger（Attack/Cast/Hit/...）由 BaseLib 的 CustomAnimationPatch 转发
/// 成该帧动画上的"一次性"播放，但没有任何一方负责"播完切回 idle"，
/// 导致攻击/受击/施法等动作放完后冻在最后一帧。
///
/// 本补丁负责补上"回 idle"：找到 starss 的帧动画节点后挂一个常驻 AnimationFinished
/// 监听，任意一次性动画自然播完即切回循环 idle（die 除外，死亡停在死亡帧）。
///
/// 挂点：
/// 1) NCreatureVisuals._Ready —— 立绘节点就绪时（若场景脚本缺类被 BaseLib 转成裸
///    NCreatureVisuals，则走这里）；优先直取 Body，Body 非帧动画时对 starss 场景递归找。
/// 2) NCreature.SetAnimationTrigger —— 任意动作触发时兜底补挂。
///
/// 注意：Godot 发出 animation_finished 那一刻节点可能仍在 playing，回调里不能用
/// IsPlaying() 判定是否该回 idle，否则会被永远跳过。
/// </summary>
public static class StarssAnimationPatch
{
	/// <summary>同一个 AnimatedSprite2D 只挂一次监听（节点销毁后条目随 GC 自动清除）。</summary>
	private static readonly ConditionalWeakTable<AnimatedSprite2D, object> _attached = new();

	[HarmonyPatch(typeof(NCreatureVisuals), nameof(NCreatureVisuals._Ready))]
	[HarmonyPostfix]
	public static void OnVisualsReady(NCreatureVisuals __instance)
	{
		try
		{
			if (!GodotObject.IsInstanceValid(__instance))
				return;

			string scene = __instance.SceneFilePath ?? "?";
			AnimatedSprite2D? anim = __instance.Body as AnimatedSprite2D;
			if (anim == null && (scene.Contains("starss") || scene.Contains("creature_visuals")))
			{
				MainFile.Logger.Info($"[anim] 诊断: visuals._Ready 的 Body 非帧动画({__instance.Body?.GetType().Name}:{__instance.Body?.Name}) scene={scene}，递归查找帧动画");
				anim = FindFrameAnim(__instance);
			}
			if (anim == null)
				return;

			EnsureIdleReturn(anim, $"visuals_ready:{scene}");
		}
		catch (Exception ex)
		{
			MainFile.Logger.Error($"[anim] OnVisualsReady 异常：{ex}");
		}
	}

	[HarmonyPatch(typeof(NCreature), nameof(NCreature.SetAnimationTrigger))]
	[HarmonyPostfix]
	public static void OnSetAnimationTrigger(NCreature __instance, string trigger)
	{
		try
		{
			if (!GodotObject.IsInstanceValid(__instance))
				return;

			AnimatedSprite2D? anim = __instance.Body as AnimatedSprite2D;
			// 非 spine 生物（帧立绘）在 Body 未解析成功时，从 Visuals 子树里递归找帧动画兜底
			if (anim == null && !__instance.HasSpineAnimation && GodotObject.IsInstanceValid(__instance.Visuals))
				anim = FindFrameAnim(__instance.Visuals);
			if (anim == null)
				return;

			EnsureIdleReturn(anim, $"trigger:{trigger}");
		}
		catch (Exception ex)
		{
			MainFile.Logger.Error($"[anim] OnSetAnimationTrigger 异常：{ex}");
		}
	}

	private static AnimatedSprite2D? FindFrameAnim(Node from)
	{
		if (!GodotObject.IsInstanceValid(from))
			return null;
		if (from is AnimatedSprite2D spr && spr.SpriteFrames != null && spr.SpriteFrames.HasAnimation("idle"))
			return spr;

		foreach (Node child in from.GetChildren())
		{
			AnimatedSprite2D? found = FindFrameAnim(child);
			if (found != null)
				return found;
		}
		return null;
	}

	private static void EnsureIdleReturn(AnimatedSprite2D anim, string via)
	{
		if (!GodotObject.IsInstanceValid(anim) || anim.SpriteFrames == null)
			return;

		// 只处理带循环 idle 的帧立绘，避免误伤不使用该约定的演出。
		if (!anim.SpriteFrames.HasAnimation("idle"))
			return;

		if (_attached.TryGetValue(anim, out _))
			return;

		anim.AnimationFinished += () => OnAnimationFinished(anim);
		_attached.Add(anim, new object());

		string anims = string.Join("/", anim.SpriteFrames.GetAnimationNames());
		MainFile.Logger.Info($"[anim] starss 帧立绘已接管回 idle（via {via}）：{anim.Name} 动画=[{anims}]");
	}

	private static void OnAnimationFinished(AnimatedSprite2D anim)
	{
		try
		{
			if (!GodotObject.IsInstanceValid(anim))
				return;

			// idle 是循环动画，正常不会走到 finish。
			if (anim.Animation == "idle")
				return;

			// 死亡动画播完保持死亡帧，不回 idle。
			if (anim.Animation == "die")
				return;

			MainFile.Logger.Info($"[anim] finish fired: {anim.Animation} 播完(playing={anim.IsPlaying()}) -> Play(idle)");
			anim.Play("idle");
		}
		catch (Exception ex)
		{
			MainFile.Logger.Error($"[anim] OnAnimationFinished 异常：{ex}");
		}
	}
}
