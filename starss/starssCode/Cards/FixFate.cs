using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using starss.starssCode.Powers;

namespace starss.starssCode.Cards;

/// <summary>
/// 修正命运（稀有能力，3 费 / 升级 2 费）：
/// 非命运的攻击牌与技能牌费用变为 0，打出后消耗，并获得 2 点永久幸运。
///
/// 效果全部落在 <see cref="FixFatePower"/> 上
/// （CorruptionPower 的费用/消耗写法 + LuckyPower 施加）。
/// </summary>
public sealed class FixFate : starssCard
{
    public FixFate()
        : base(3, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<FixFatePower>(1M),
        new DynamicVar("Luck", 2M)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<FixFatePower>(
            choiceContext,
            Owner.Creature,
            DynamicVars["FixFatePower"].BaseValue,
            Owner.Creature,
            this
        );
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
