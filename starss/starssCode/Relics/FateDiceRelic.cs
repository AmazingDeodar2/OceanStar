using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;
using starss.starssCode.Mechanics;
using starss.starssCode.Powers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace starss.starssCode.Relics;


public sealed class FateDiceRelic : starssRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<LuckyPower>("Power", 20M)
    ];

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is not CombatRoom)
            return;

        Flash();

        var rollResult = DiceHelper.RollD20(Owner.Creature);

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