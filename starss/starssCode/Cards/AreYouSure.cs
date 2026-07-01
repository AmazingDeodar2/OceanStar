using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using starss.starssCode.Mechanics;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace starss.starssCode.Cards;

public sealed class AreYouSure : starssCard
{
    public AreYouSure()
        : base(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
    }

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(6M, ValueProp.Move),
        new DynamicVar("BonusBlock", 4M),
        new FateVar(50M)
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
            
            await CreatureCmd.GainBlock(
                Owner.Creature,
                DynamicVars["BonusBlock"].BaseValue,
                ValueProp.Unpowered,
                null
            );
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3M);
    }
}