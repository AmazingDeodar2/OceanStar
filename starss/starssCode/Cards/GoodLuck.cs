using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using starss.starssCode.Powers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace starss.starssCode.Cards;

public sealed class GoodLuck : starssCard
{
    public GoodLuck()
        : base(0, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
    }
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<StrengthPower>("Power", 1M)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>(),
        HoverTipFactory.FromPower<DexterityPower>(),
        HoverTipFactory.FromPower<VigorPower>(),
        HoverTipFactory.FromPower<PlatingPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(
            Owner.Creature,
            "Cast",
            Owner.Character.CastAnimDelay
        );

        var amount = DynamicVars["Power"].BaseValue;

        await PowerCmd.Apply<GoodLuckTemporaryStrengthPower>(
            choiceContext,
            Owner.Creature,
            amount,
            Owner.Creature,
            this
        );

        await PowerCmd.Apply<GoodLuckTemporaryDexterityPower>(
            choiceContext,
            Owner.Creature,
            amount,
            Owner.Creature,
            this
        );

        await PowerCmd.Apply<VigorPower>(
            choiceContext,
            Owner.Creature,
            amount,
            Owner.Creature,
            this
        );

        await PowerCmd.Apply<PlatingPower>(
            choiceContext,
            Owner.Creature,
            amount,
            Owner.Creature,
            this
        );
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}