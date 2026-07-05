using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using starss.starssCode.Powers;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace starss.starssCode.Potions;

public sealed class LoadedDicePotion : starssPotion
{
    public override PotionRarity Rarity => PotionRarity.Common;

    public override PotionUsage Usage => PotionUsage.CombatOnly;

    public override TargetType TargetType => TargetType.Self;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Luck", 50M)
    ];

    protected override async Task OnUse(
        PlayerChoiceContext choiceContext,
        Creature? target)
    {
        await PowerCmd.Apply<NextCheckLuckPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars["Luck"].BaseValue,
            Owner.Creature,
            null
        );
    }

    public override List<(string, string)>? Localization =>
    [
        ("title", "灌铅骰子"),
        ("description", "下一次检定幸运50。"),
        ("flavor", "它只是比较偏心。")
    ];
}