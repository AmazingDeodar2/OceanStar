using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using starss.starssCode.Mechanics;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Models.Powers;

namespace starss.starssCode.Cards;

public sealed class AreYouSure : starssCard
{
    public AreYouSure()
        : base(1, CardType.Skill, CardRarity.Basic, TargetType.AnyEnemy)
    {
    }

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(8M, ValueProp.Move),
        new FateVar(50M),
        new PowerVar<VulnerablePower>(1M),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(
            Owner.Creature,
            DynamicVars.Block,
            cardPlay
        );

        var check = await DiceHelper.Check(
            Owner.Creature,
            fate: 50,
            doom: 101
        );

        if (check.FateSuccess)
        {
            
            await PowerCmd.Apply<VulnerablePower>(
                choiceContext,
                cardPlay.Target,
                DynamicVars.Vulnerable.BaseValue,
                Owner.Creature,
                this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3M);
        DynamicVars.Vulnerable.UpgradeValueBy(1M);
    }
}