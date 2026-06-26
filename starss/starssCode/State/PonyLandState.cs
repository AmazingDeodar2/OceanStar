using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using starss.starssCode.Cards;
using starss.starssCode.Mechanics;

namespace starss.starssCode.States;

public class PonyLandState : StateModel
{
    private readonly decimal strengthAmount;

    public override string Id => "starss:pony_land";

    public PonyLandState(decimal strengthAmount)
    {
        this.strengthAmount = strengthAmount;
        Duration = int.MaxValue;
    }

    public override async Task OnEnter(PlayerChoiceContext choiceContext)
    {
        await PowerCmd.Apply<StrengthPower>(
            choiceContext,
            Owner.Creature,
            strengthAmount,
            Owner.Creature,
            null
        );
    }

    public override async Task OnExit(PlayerChoiceContext choiceContext)
    {
        await PowerCmd.Apply<StrengthPower>(
            choiceContext,
            Owner.Creature,
            -strengthAmount,
            Owner.Creature,
            null
        );
    }
    public override int ModifyDiceRoll(Creature creature, CardModel? sourceCard, int roll)
    {
        if (sourceCard is DesertEagle)
            return 96 + (roll - 1) * 4 / 99;

        return roll;
    }
    public override string DisplayName => "小马国度";
}