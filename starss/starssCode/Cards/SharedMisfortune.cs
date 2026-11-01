using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using starss.starssCode.Mechanics;
using starss.starssCode.Powers;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.ValueProps;

namespace starss.starssCode.Cards;

public sealed class SharedMisfortune : starssCard
{
    public SharedMisfortune()
        : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
    }

    // 触发条件是"命运没被触发 / 厄运被触发"，两个概念都挂
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        StarssHoverTips.Fate,
        StarssHoverTips.Doom
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(10M, ValueProp.Unpowered)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<SharedMisfortunePower>(
            choiceContext,
            Owner.Creature,
            DynamicVars.Damage.BaseValue,
            Owner.Creature,
            this
        );
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2M);
    }
}