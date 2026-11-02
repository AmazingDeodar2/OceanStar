using Godot;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace starss.scenes.creature_visuals;

/// <summary>
/// starss 战斗立绘根节点。Spine 模式下动画由 starss.cs 的 GenerateAnimator 驱动，
/// 不再需要帧动画的"播放完回 idle"补丁。这里仅作为 NCreatureVisuals 的子类锚定场景类型。
/// </summary>
public partial class StarssNCreature : NCreatureVisuals
{
}
