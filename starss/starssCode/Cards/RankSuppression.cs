using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using starss.starssCode.Powers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace starss.starssCode.Cards;


public sealed class RankSuppression : starssCard
{
    public RankSuppression()
        : base(
            1,
            CardType.Skill,
            CardRarity.Common,
            TargetType.Self)
    {
    }


    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new RepeatVar(3)
    ];


    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(
            Owner.Creature,
            "PowerUp",
            Owner.Character.PowerUpAnimDelay
        );


        await PowerCmd.Apply<RankSuppressionPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars.Repeat.BaseValue,
            Owner.Creature,
            this
        );
    }


    protected override void OnUpgrade()
    {
        DynamicVars.Repeat.UpgradeValueBy(2M);
    }
}