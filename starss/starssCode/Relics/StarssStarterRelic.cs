using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;
using starss.starssCode.Powers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using starss.starssCode.Mechanics;

namespace starss.starssCode.Relics;

public sealed class StarssStarterRelic : starssRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<LuckyPower>(6M)
    ];

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is not CombatRoom)
            return;

        Flash();

        var rollResult = DiceHelper.RollD6(Owner.Creature);

        await DiceUi.ShowRoll(rollResult);

        await PowerCmd.Apply<LuckyPower>(
            new ThrowingPlayerChoiceContext(),
            Owner.Creature,
            rollResult.Value,
            Owner.Creature,
            null
        );
    }
}