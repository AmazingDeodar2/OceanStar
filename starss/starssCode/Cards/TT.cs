using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using starss.starssCode.Mechanics;
using starss.starssCode.States;
using System.Collections.Generic;
using System.Threading.Tasks;
using starss.starssCode.Cards.Interfaces;

namespace starss.starssCode.Cards;

public sealed class TT : starssCard,IPcCard
{
    public TT()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        StarssHoverTips.Pc,
        StarssHoverTips.State
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords
    {
        get
        {
            if (IsUpgraded)
                return [CardKeyword.Retain];

            return [];
        }
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(
            Owner.Creature,
            "Cast",
            Owner.Character.CastAnimDelay
        );

        await StateCmd.Enter(choiceContext, Owner, new RatCreviceState());
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}