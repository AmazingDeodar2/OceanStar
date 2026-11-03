namespace starss.starssCode.Cards.Interfaces;

/// <summary>
/// 命运牌标记接口。
///
/// 与 <see cref="IDoomCard"/> 完全同构：官方 <c>CardTag</c> 是封闭枚举，mod 无法扩展，
/// 所以沿用「空标记接口 + <c>card is IFateCard</c>」的套路，不产生任何 UI。
///
/// 判定口径：<c>CanonicalVars</c> 里带 <c>FateVar</c>（即这张牌真的会掷命运）的卡。
/// 现有 20 张：AllIn / AreYouSure / Barrier / Brawl / ChaosStrike / DesertEagle /
/// Dodge / FoxFire / GodDice / Humanology / LetUsBegin / Library / Listen /
/// LuckyFinish / OneeSanTime / SanCheck / Satchel / Scout / Stealth / WildIdea。
///
/// 消费点：
/// 1. 「坦途」（SmoothPathPower）——困难成功时让命运效果多触发 N 次；
/// 2. 「修正命运」（FixFatePower）——打出命运牌时按卡面伤害锻造。
///
/// 新增命运牌时记得同时实现本接口，否则上述两张牌对它不生效。
/// </summary>
public interface IFateCard
{
}
