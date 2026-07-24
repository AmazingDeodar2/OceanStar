using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using VoidCard = MegaCrit.Sts2.Core.Models.Cards.Void;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using starss.starssCode.Mechanics;

namespace starss.starssCode.Cards;

public sealed class OneeSanTime : starssCard
{
    public OneeSanTime()
        : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(4),
        new FateVar(30M),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        EnergyHoverTip,
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayerCmd.GainEnergy(
            DynamicVars.Energy.IntValue,
            Owner
        );
        var check = await DiceHelper.Check(
            Owner.Creature,
            fate: DynamicVars["Fate"].IntValue,
            doom: 101,
            choiceContext: choiceContext,
            sourceCard: this
        );
        if (check.FateSuccess)
        {
            return;
        }

        EnergyCost.AddThisCombat(1);
        
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Energy.UpgradeValueBy(1M);
    }
}