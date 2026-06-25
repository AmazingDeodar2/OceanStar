using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using starss.starssCode.Powers;

namespace starss.starssCode.Mechanics;

public sealed class FateVar(decimal value) : DynamicVar("Fate", value)
{
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

public sealed class DoomVar(decimal value) : DynamicVar("Doom", value)
{
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
