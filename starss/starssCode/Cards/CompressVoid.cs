using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using starss.starssCode.Cards.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using VoidCard = MegaCrit.Sts2.Core.Models.Cards.Void;
using BeckonCard =  MegaCrit.Sts2.Core.Models.Cards.Beckon;

namespace starss.starssCode.Cards;


public sealed class CompressVoid : starssCard
{
    private const string CalculatedDamageKey = "CalculatedDamage";

    public CompressVoid()
        : base(1, CardType.Skill,CardRarity.Uncommon, TargetType.Self)
    {
    }
    
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];
    
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int voidCount =
            PileType.Exhaust
                .GetPile(Owner)
                .Cards
                .Count(IsVoid);


        // 少于5张虚空
        if (voidCount < 5)
        {
            await PowerCmd.Apply<ArtifactPower>(
                choiceContext,
                Owner.Creature,
                1,
                Owner.Creature,
                this);

            return;
        }


        // 5~7张虚空
        if (voidCount < 8)
        {
            await PowerCmd.Apply<IntangiblePower>(
                choiceContext,
                Owner.Creature,
                1,
                Owner.Creature,
                this);

            return;
        }


        // 8张以上
        foreach (PowerModel power in Owner.Creature.Powers.ToList())
        {
            if (power.Type == PowerType.Debuff)
            {
                await PowerCmd.Remove(power);
            }
        }
    }

    private static IEnumerable<CardModel> GetVoidAndCallCards(Player owner)
    {
        return owner.PlayerCombatState.AllCards
            .Where(c =>
                c.Pile.Type != PileType.Exhaust &&
                (c is VoidCard || c is BeckonCard)
            );
    }

    private static bool IsVoid(CardModel card)
    {
        return card is VoidCard;
    }
    
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}