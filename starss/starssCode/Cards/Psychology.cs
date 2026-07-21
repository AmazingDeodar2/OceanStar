using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using starss.starssCode.Powers;
using System;
using System.Threading.Tasks;

namespace starss.starssCode.Cards;


public sealed class Psychology : starssCard
{
    public Psychology()
        : base(
            1,
            CardType.Skill,
            CardRarity.Uncommon,
            TargetType.AnyEnemy)
    {
    }

    public override CardMultiplayerConstraint MultiplayerConstraint
        => CardMultiplayerConstraint.MultiplayerOnly;

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(
            cardPlay.Target,
            nameof(cardPlay.Target)
        );

        await CreatureCmd.TriggerAnim(
            Owner.Creature,
            "Cast",
            Owner.Character.CastAnimDelay
        );

        await PowerCmd.Apply<PsychologyPower>(
            choiceContext,
            cardPlay.Target,
            1M,
            Owner.Creature,
            this
        );
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}