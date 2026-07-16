using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using starss.starssCode.States;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using starss.starssCode.Cards.Interfaces;
using starss.starssCode.Mechanics;
using starss.starssCode.Powers;

namespace starss.starssCode.Cards;


public sealed class TimeReversal : starssCard
{
    public TimeReversal()
        : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        // 打出时立即获得的幸运
        new PowerVar<LuckyPower>("Luck", 50M),
        // 每层“时光倒流”每回合减少的幸运
        new DynamicVar("LuckLoss", 10M),
        // 每次使用获得1层负面效果
        new PowerVar<TimeReversalPower>("Power", 1M)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<LuckyPower>(),
        HoverTipFactory.FromPower<TimeReversalPower>()
    ];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 立即获得50点幸运，升级后为60点
        await PowerCmd.Apply<LuckyPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars["Luck"].BaseValue,
            Owner.Creature,
            this
        );

        // 获得1层“时光倒流”负面效果
        await PowerCmd.Apply<TimeReversalPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars["Power"].BaseValue,
            Owner.Creature,
            this
        );
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Luck"].UpgradeValueBy(10M);
    }
}