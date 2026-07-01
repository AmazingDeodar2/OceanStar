using Godot;
using HarmonyLib;

using MegaCrit.Sts2.Core.Nodes.Combat;
using HarmonyLib;

namespace starss.starssCode.Patches;

[HarmonyPatch(typeof(NCreature), "SetAnimationTrigger")]
public static class StarssAnimationPatch
{
	public static void Postfix(NCreature __instance, string trigger)
	{
		if (__instance.Entity == null)
			return;

		if (!__instance.Entity.IsPlayer)
			return;

		if (__instance.Entity.ModelId.ToString() != "CHARACTER.STARSS")
			return;

		var anim = __instance.GetNodeOrNull<AnimatedSprite2D>("%Visuals");
		if (anim == null)
			return;

		switch (trigger)
		{
			case "Hit":
				Play(anim, "hurt");
				break;

			case "Attack":
				Play(anim, "attack");
				break;

			case "Cast":
				Play(anim, "cast");
				break;

			case "Dead":
				Play(anim, "die");
				break;

			default:
				Play(anim, "idle");
				break;
		}
	}

	private static void Play(AnimatedSprite2D anim, string name)
	{
		anim.Frame = 0;
		anim.Play(name);

		if (name != "die")
		{
			anim.AnimationFinished += () =>
			{
				anim.Play("idle");
			};
		}
	}
	
}
