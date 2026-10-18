using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace starss.starssCode.Cards;


[Pool(typeof(TokenCardPool))]
public sealed class Parasite : starssCard
{
    public Parasite()
        : base(
            -1,
            CardType.Curse,
            CardRarity.Curse,
            TargetType.None
        )
    {
    }

    // 诅咒牌不可升级
    public override int MaxUpgradeLevel => 0;

    // 不能打出、虚无
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        new[]
        {
            CardKeyword.Unplayable,
            CardKeyword.Ethereal
        };

    // 告诉游戏：这张牌在手牌中时具有回合结束效果
    public override bool HasTurnEndInHandEffect => true;

    // 回合结束时，若仍在手牌中，失去1点生命
    protected override async Task OnTurnEndInHand(
        PlayerChoiceContext choiceContext)
    {
        await CreatureCmd.Damage(
            choiceContext,
            Owner.Creature,
            1m,
            ValueProp.Unblockable |
            ValueProp.Unpowered |
            ValueProp.Move,
            this,
            null
        );
    }
}