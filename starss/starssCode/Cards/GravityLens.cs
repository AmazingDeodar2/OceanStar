using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Models;
using VoidCard = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace starss.starssCode.Cards;

public sealed class GravityLens : starssCard
{
    public GravityLens()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(3M, ValueProp.Unpowered),
        new EnergyVar(3)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(
            Owner.Creature,
            "Cast",
            Owner.Character.CastAnimDelay
        );

        var statusCards = PileType.Hand.GetPile(Owner).Cards
            .Where(c => c != null && c.Type == CardType.Status)
            .ToList();

        if (statusCards.Count <= 0)
            return;

        bool hasVoid = statusCards.Any(c => c is VoidCard);

        foreach (CardModel card in statusCards)
        {
            await CardCmd.Exhaust(choiceContext, card);
        }

        await CreatureCmd.GainBlock(
            Owner.Creature,
            DynamicVars.Block.BaseValue * statusCards.Count,
            ValueProp.Unpowered,
            cardPlay
        );

        if (hasVoid)
        {
            Owner.PlayerCombatState.GainEnergy(3M);
        }
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(1M);
    }
}