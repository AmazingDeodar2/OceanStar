using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using starss.starssCode.Mechanics;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace starss.starssCode.Cards;

public sealed class AnyoneWillDo : starssCard
{
    public AnyoneWillDo()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords
        => new[] { CardKeyword.Exhaust };

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(
            Owner.Creature,
            "Cast",
            Owner.Character.CastAnimDelay
        );

        StateModel? state =
            StateRandomHelper.GetRandomDifferentState(Owner);

        // 极端情况下，七种状态全部存在，不再进入状态。
        if (state == null)
            return;

        await StateCmd.Enter(
            choiceContext,
            Owner,
            StateRandomHelper.GetRandomDifferentState(Owner)
        );
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}