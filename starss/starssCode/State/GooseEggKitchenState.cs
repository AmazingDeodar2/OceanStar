using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using starss.starssCode.Mechanics;
using System.Threading.Tasks;

namespace starss.starssCode.States;

public sealed class GooseEggKitchenState : StateModel
{
    public override string Id => "starss:goose_egg_kitchen";

    public override string DisplayName => "鹅蛋厨房";

    public GooseEggKitchenState()
    {
        Duration = int.MaxValue;
    }

    public override async Task OnEnter(PlayerChoiceContext choiceContext)
    {
        await ProcureRandomPotion();
    }

    public override async Task OnExit(PlayerChoiceContext choiceContext)
    {
        await ProcureRandomPotion();
    }

    private async Task ProcureRandomPotion()
    {
        await PotionCmd.TryToProcure(
            PotionFactory.CreateRandomPotionInCombat(
                Owner,
                Owner.RunState.Rng.CombatPotionGeneration
            ).ToMutable(),
            Owner
        );
    }
}