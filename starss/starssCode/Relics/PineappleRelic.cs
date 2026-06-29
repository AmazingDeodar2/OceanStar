using System.Collections.Generic;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Rooms;
using starss.starssCode.Powers;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace starss.starssCode.Relics;


public sealed class Pineapple : starssRelic
{
    public override RelicRarity Rarity => RelicRarity.Common;

    public override bool HasUponPickupEffect => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new MaxHpVar(5M),
        new PowerVar<LuckyPower>(2M)
    ];

    public override async Task AfterObtained()
    {
        await CreatureCmd.GainMaxHp(
            Owner.Creature,
            DynamicVars.MaxHp.BaseValue
        );
    }

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is not CombatRoom)
            return;

        Flash();

        await PowerCmd.Apply<LuckyPower>(
            new ThrowingPlayerChoiceContext(),
            Owner.Creature,
            2M,
            Owner.Creature,
            null
        );
    }
}