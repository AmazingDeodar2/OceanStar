using System.Collections.Generic;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;
using starss.starssCode.Powers;
using System.Threading.Tasks;
using starss.starssCode.Mechanics;

namespace starss.starssCode.Cards;

public sealed class Lalang : starssCard
{
    public Lalang()
        : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
    }
    
    

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(
            Owner.Creature,
            "PowerUp",
            Owner.Character.PowerUpAnimDelay
        );
        

        StateCmd.AddCapacity(Owner, 1);
    }
    protected override void OnUpgrade() => this.AddKeyword(CardKeyword.Innate);
}