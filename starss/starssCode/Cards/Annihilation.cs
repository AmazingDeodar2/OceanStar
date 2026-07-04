using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Models;

namespace starss.starssCode.Cards;


public sealed class Annihilation : starssCard
{
    public Annihilation()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(4M, ValueProp.Unpowered)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var handCards = PileType.Hand.GetPile(Owner).Cards
            .Where(c => c != this)
            .ToList();

        foreach (CardModel card in handCards)
        {
            await CardCmd.Exhaust(choiceContext, card);

            await CreatureCmd.GainBlock(
                Owner.Creature,
                DynamicVars.Block.BaseValue,
                ValueProp.Unpowered,
                cardPlay
            );
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2M);
    }
}