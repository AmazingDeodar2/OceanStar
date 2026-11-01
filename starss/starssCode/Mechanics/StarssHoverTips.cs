using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;

namespace starss.starssCode.Mechanics;

/// <summary>
/// 海星自定义静态 hover tip —— 用于官方 <c>StaticHoverTip</c> 枚举里没有的概念
/// （该枚举只有 Channeling/Transform/Block/Energy/Stun 等几十个固定值，塞不进我们的骰点术语）。
///
/// 用法：卡牌/遗物/药水里 override <c>ExtraHoverTips</c>，返回 <c>StarssHoverTips.Doom</c> 之类即可。
/// 注意它是**卡级**提示（显示在卡牌下方那排图标），与 DiceVars 里用 BaseLib WithTooltip 做的
/// **变量级**提示（悬停描述里的数字）是两套东西，通常两处都要挂。
/// </summary>
public static class StarssHoverTips
{
    public const string Table = "static_hover_tips";

    /// <param name="key">
    /// <c>static_hover_tips.json</c> 里的完整键（不含 .title/.description 后缀），
    /// 例如 <c>"STARSS-FATE"</c>。
    /// </param>
    public static IHoverTip FromKey(string key)
    {
        return new HoverTip(
            new LocString(Table, $"{key}.title"),
            new LocString(Table, $"{key}.description")
        );
    }

    // 骰点系统四个概念：与 DiceVars 的 WithTooltip / *Var 共用同一批键，改文案只改一处。

    /// <summary>投点不高于此值时触发。每 1 点幸运，此值 +1。</summary>
    public static IHoverTip Fate => FromKey("STARSS-FATE");

    /// <summary>投点不低于此值时触发。每 2 点幸运，此值 +1。</summary>
    public static IHoverTip Doom => FromKey("STARSS-DOOM");

    /// <summary>永久幸运。</summary>
    public static IHoverTip Luck => FromKey("STARSS-LUCK");

    /// <summary>临时幸运，只作用于下一次投点判定。</summary>
    public static IHoverTip NextCheckLuck => FromKey("STARSS-NEXT_CHECK_LUCK");
}
