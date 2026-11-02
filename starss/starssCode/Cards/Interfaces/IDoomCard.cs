namespace starss.starssCode.Cards.Interfaces;

/// <summary>
/// 「厄运牌」标记接口。
///
/// 官方 <c>CardTag</c> 是封闭枚举（只有 None / Strike / Defend / Minion / OstyAttack / Shiv，
/// 见 <c>Entities/Cards/CardTag.cs:15-47</c>），Mod 没法往里加「命运 / 厄运」，
/// 所以沿用本项目 <see cref="IPcCard"/> 的套路：空标记接口 + <c>card is IDoomCard</c>。
///
/// 两点要注意：
/// 1. 它是**纯代码层判定**，与 UI 完全无关 —— 不会在卡面产生任何 tip、角标或关键字。
///    卡面提示只有 override <c>ExtraHoverTips</c> 才会出现；命运/厄运的解释来自
///    <c>DiceVars</c> 里 FateVar / DoomVar 的 <c>WithTooltip()</c>，跟这个接口无关。
/// 2. 接口可以多实现，所以「既是命运牌又是厄运牌」（例如混乱打击）直接
///    <c>IDoomCard, IFateCard</c> 即可，比 enum 灵活。
/// </summary>
public interface IDoomCard
{
}
