using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using starss.starssCode.Cards;

namespace starss.starssCode.Potions;


public sealed class CloverBrew : starssPotion
{
    public override PotionRarity Rarity => PotionRarity.Rare;

    public override PotionUsage Usage => PotionUsage.CombatOnly;

    public override TargetType TargetType => TargetType.Self;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(3)
    ];

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<CloverLeaf>(true)
    ];
    public override List<(string, string)>? Localization =>
    [
        ("title", "四叶草佳酿"),
        ("description", "将3张四叶草叶片+放入手牌。"),
        ("flavor", "幸运被酿成了可以饮用的形状。")
    ];
    protected override async Task OnUse(
        PlayerChoiceContext choiceContext,
        Creature? target)
    {
        foreach (var card in await CloverLeaf.CreateInHand(
                     Owner,
                     DynamicVars.Cards.IntValue,
                     Owner.Creature.CombatState))
        {
            CardCmd.Upgrade(card);
        }
    }
}