using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using starss.starssCode.Mechanics;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Models;

namespace starss.starssCode.Powers;



public sealed class LalangPower : starssPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task AfterStateEntered(PlayerChoiceContext choiceContext, StateModel state)
    {
        Flash();

        Owner.Player.PlayerCombatState.GainEnergy(1M * Amount);
    }
}