using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using starss.starssCode.Cards.Interfaces;
using starss.starssCode.Powers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Models.Cards;

namespace starss.starssCode.Cards;

public sealed class VoidShield : starssCard
{
    public VoidShield()
        : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<BarrierPower>("Power", 6M)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int voidAndCallCount = Owner.PlayerCombatState.ExhaustPile.Cards.Count(card =>
            card is MegaCrit.Sts2.Core.Models.Cards.Void || card is Beckon
        );

        decimal amount = DynamicVars["Power"].BaseValue + voidAndCallCount;

        await PowerCmd.Apply<BarrierPower>(
            choiceContext,
            Owner.Creature,
            amount,
            Owner.Creature,
            this
        );
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Power"].UpgradeValueBy(2M);
    }
}