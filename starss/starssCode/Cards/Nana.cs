using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using starss.starssCode.States;
using System.Collections.Generic;
using System.Threading.Tasks;
using starss.starssCode.Cards.Interfaces;
using starss.starssCode.Mechanics;

namespace starss.starssCode.Cards;


public sealed class Nana : starssCard, IPcCard
{
    public Nana()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(
            Owner.Creature,
            "Cast",
            Owner.Character.CastAnimDelay
        );

        await StateCmd.Enter(
            choiceContext,
            Owner,
            new JellyfishWorldState()
        );
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}