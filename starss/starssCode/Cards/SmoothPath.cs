using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using starss.starssCode.Powers;

namespace starss.starssCode.Cards;

/// <summary>
/// 困难成功（投点 ≤ 命运值/2）时，命运效果额外触发 1 次；升级后额外触发 2 次。
/// 数值落地在能力层数上（<see cref="SmoothPathPower"/>），
/// 执行逻辑集中在 <c>FateEffect.Run</c>。
/// </summary>
public sealed class SmoothPath : starssCard
{
    public SmoothPath()
        : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Innate
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CardKeyword.Innate)
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<SmoothPathPower>(1M)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<SmoothPathPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars["SmoothPathPower"].BaseValue,
            Owner.Creature,
            this
        );
    }

    protected override void OnUpgrade()
    {
        DynamicVars["SmoothPathPower"].UpgradeValueBy(1M);
    }
}
