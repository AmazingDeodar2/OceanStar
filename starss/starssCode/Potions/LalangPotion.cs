using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using starss.starssCode.Mechanics;

namespace starss.starssCode.Potions;

public sealed class LalangPotion : starssPotion
{
    public override PotionRarity Rarity => PotionRarity.Uncommon;

    public override PotionUsage Usage => PotionUsage.CombatOnly;

    public override TargetType TargetType => TargetType.Self;

    protected override async Task OnUse(
        PlayerChoiceContext choiceContext,
        Creature? target)
    {
        await CreatureCmd.TriggerAnim(
            Owner.Creature,
            "PowerUp",
            Owner.Character.PowerUpAnimDelay
        );

        StateCmd.AddCapacity(
            Owner,
            1
        );
    }

    public override List<(string, string)>? Localization =>
    [
        ("title", "拉郎药水"),
        ("description", "获得一个额外状态栏。"),
        ("flavor", "拉郎就是需要乱炖。")
    ];
}