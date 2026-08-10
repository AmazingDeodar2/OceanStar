using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using starss.starssCode.Mechanics;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace starss.starssCode.Powers;


public sealed class RecruitmentProcessPower : starssPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task AfterStateEntered(PlayerChoiceContext choiceContext, StateModel state)
    {
        Flash();

        
        await CreatureCmd.GainBlock(
            Owner, 
            Amount,
            ValueProp.Unpowered, 
            (CardPlay?)null 
        );
    }
}