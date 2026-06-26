using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using starss.starssCode.Mechanics;

namespace starss.starssCode.States;

public class FlatDomainState : StateModel
{
    public override string Id => "starss:flat_domain";

    public override string DisplayName => "平板领域";

    public override async Task AfterCardGeneratedForCombat(
        CardModel card,
        Player? creator)
    {
        if (creator != Owner)
            return;

        if (card is MegaCrit.Sts2.Core.Models.Cards.Void)
        {
            await CardCmd.Exhaust(
                new ThrowingPlayerChoiceContext(),
                card,
                skipVisuals: true
            );
        }
    }

    public override Task OnExit(PlayerChoiceContext choiceContext)
    {
        Owner.PlayerCombatState.GainEnergy(1);
        return Task.CompletedTask;
    }
}