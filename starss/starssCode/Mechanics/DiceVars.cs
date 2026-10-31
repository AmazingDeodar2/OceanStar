using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using starss.starssCode.Powers;

namespace starss.starssCode.Mechanics;

// 骰点变量都挂了 hover tip（BaseLib 的 WithTooltip）。
// 文案在 localization/eng/static_hover_tips.json。
// 键的推导：命名空间首段 "starss" -> "STARSS-" + Slugify(var.Name)
// （BaseLib Extensions/TypePrefix.cs:6-14 + Extensions/DynamicVarExtensions.cs:65）。
// 注意：两个幸运变量都叫 "Luck"，会推出同一个键，所以必须显式传 locKey 区分。

public sealed class FateVar : DynamicVar
{
    public FateVar(decimal value) : base("Fate", value)
    {
        this.WithTooltip(); // STARSS-FATE
    }

    public override void UpdateCardPreview(
        CardModel card,
        CardPreviewMode previewMode,
        Creature? target,
        bool runGlobalHooks)
    {
        PreviewValue = DiceHelper.ApplyFate(
            (int)BaseValue,
            card.Owner.Creature
        );
    }
}

public sealed class DoomVar : DynamicVar
{
    public DoomVar(decimal value) : base("Doom", value)
    {
        this.WithTooltip(); // STARSS-DOOM
    }

    public override void UpdateCardPreview(
        CardModel card,
        CardPreviewMode previewMode,
        Creature? target,
        bool runGlobalHooks)
    {
        PreviewValue = DiceHelper.ApplyDoom(
            (int)BaseValue,
            card.Owner.Creature
        );
    }
}

// 永久幸运：施加 LuckyPower（如 BurnLuck 消耗、DisasterRelief 获得）
public sealed class LuckVar : DynamicVar
{
    public LuckVar(decimal value) : base("Luck", value)
    {
        this.WithTooltip("STARSS-LUCK");
    }
}

// 临时幸运：只参与下一次投点判定，判定后整层移除（NextCheckLuckPower）
public sealed class NextCheckLuckVar : DynamicVar
{
    public NextCheckLuckVar(decimal value) : base("Luck", value)
    {
        this.WithTooltip("STARSS-NEXT_CHECK_LUCK");
    }
}
