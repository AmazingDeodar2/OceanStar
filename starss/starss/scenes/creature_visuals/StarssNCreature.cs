using Godot;
using System;
using MegaCrit.Sts2.Core.Nodes.Combat;
using starss.starssCode;

namespace starss.scenes.creature_visuals;

/// <summary>
/// starss 战斗立绘（%Visuals 是 AnimatedSprite2D 帧动画）的根脚本。
///
/// 游戏/BaseLib 会用 NCreature.SetAnimationTrigger 驱动这个帧动画做"一次性"动作
/// （attack/cast/hurt/...），但没有人负责"动作播完切回循环 idle"，于是会冻在最后一帧。
/// 这里在节点就绪后给帧动画挂一个常驻 AnimationFinished 监听，播完即回 idle（die 除外）。
/// </summary>
public partial class StarssNCreature : NCreatureVisuals
{
	private bool _hooked;

	public override void _Ready()
	{
		base._Ready();
		HookIdleReturn();
	}

	private void HookIdleReturn()
	{
		try
		{
			if (_hooked || !GodotObject.IsInstanceValid(this))
				return;

			if (Body is not AnimatedSprite2D anim)
				return;
			if (anim.SpriteFrames == null || !anim.SpriteFrames.HasAnimation("idle"))
				return;

			anim.AnimationFinished += OnAnimationFinished;
			_hooked = true;

			MainFile.Logger.Info($"[anim] StarssNCreature 已接管回 idle：{anim.Name} 动画=[{string.Join("/", anim.SpriteFrames.GetAnimationNames())}]");
		}
		catch (Exception ex)
		{
			MainFile.Logger.Error($"[anim] StarssNCreature.HookIdleReturn 异常：{ex}");
		}
	}

	private void OnAnimationFinished()
	{
		try
		{
			if (!GodotObject.IsInstanceValid(this))
				return;
			if (Body is not AnimatedSprite2D anim)
				return;

			// idle 是循环动画，正常不会走到 finish。
			if (anim.Animation == "idle" || anim.Animation == "die")
				return;

			MainFile.Logger.Info($"[anim] StarssNCreature finish fired: {anim.Animation} 播完(playing={anim.IsPlaying()}) -> Play(idle)");
			anim.Play("idle");
		}
		catch (Exception ex)
		{
			MainFile.Logger.Error($"[anim] StarssNCreature.OnAnimationFinished 异常：{ex}");
		}
	}
}
