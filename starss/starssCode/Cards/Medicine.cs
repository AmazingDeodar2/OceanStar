using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using starss.starssCode.Powers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace starss.starssCode.Cards;

public sealed class Medicine : starssCard
{
    public Medicine()
        : base(
            0,
            CardType.Skill,
            CardRarity.Rare,
            TargetType.Self
        )
    {
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(
            Owner.Creature,
            "Cast",
            Owner.Character.CastAnimDelay
        );

        await PowerCmd.Apply<MedicinePower>(
            choiceContext,
            Owner.Creature,
            1M,
            Owner.Creature,
            this
        );
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}