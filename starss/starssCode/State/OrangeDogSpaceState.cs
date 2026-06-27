using MegaCrit.Sts2.Core.Entities.Creatures;
using starss.starssCode.Mechanics;

namespace starss.starssCode.States;

public sealed class OrangeDogSpaceState : StateModel
{
    public override string Id => "starss:orange_dog_space";

    public override string DisplayName => "橘狗空间";

    public OrangeDogSpaceState()
    {
        Duration = int.MaxValue;
    }

    public override bool ShouldClearBlock(Creature creature)
    {
        return Owner.Creature != creature;
    }
}