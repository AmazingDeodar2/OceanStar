using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using starss.starssCode.Mechanics;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Models;

namespace starss.starssCode.Cards;


public sealed class Satchel : starssCard
{
    public Satchel()
        : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
    }

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(8M, ValueProp.Unpowered),
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
            fate: (int)DynamicVars["Fate"].BaseValue,
            doom: 0,
            choiceContext: choiceContext,
            sourceCard: this
        );

        if (!check.FateSuccess)
            return;

        await DiceHelper.OnFateTriggered(choiceContext, this);

        if (IsUpgraded)
        {
            foreach (CardModel card in PileType.Hand.GetPile(Owner).Cards
                         .Where(c => c.IsUpgradable))
            {
                CardCmd.Upgrade(card);
            }
        }
        else
        {
            CardModel card = await CardSelectCmd.FromHandForUpgrade(
                choiceContext,
                Owner,
                this
            );

            if (card != null)
                CardCmd.Upgrade(card);
        }
    }

    protected override void OnUpgrade()
    {
    }
}