using System.Collections.Generic;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using starss.starssCode.Powers;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using starss.starssCode.Mechanics;

namespace starss.starssCode.Cards;

public sealed class Barrier : starssCard
{
    public Barrier()
        : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<BarrierPower>(
            choiceContext,
            Owner.Creature,
            1M,
            Owner.Creature,
            this
        );
    }
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new FateVar(40M),
        new DoomVar(61M),
        new EnergyVar(1)
    ];
    protected override void OnUpgrade() => this.AddKeyword(CardKeyword.Innate);
}