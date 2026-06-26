using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using starss.starssCode.Mechanics;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Models;

namespace starss.starssCode.Powers;


public sealed class RecruitmentProcessPower : PowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task AfterStateEntered(PlayerChoiceContext choiceContext, StateModel state)
    {
        Flash();

        await PowerCmd.Apply<LuckyPower>(
            choiceContext,
            Owner,
            Amount,
            Owner,
            null
        );
    }
}