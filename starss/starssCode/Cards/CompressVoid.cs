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
using MegaCrit.Sts2.Core.Models;
using VoidCard = MegaCrit.Sts2.Core.Models.Cards.Void;
using BeckonCard =  MegaCrit.Sts2.Core.Models.Cards.Beckon;

namespace starss.starssCode.Cards;


public sealed class CompressVoid : starssCard
{
    private const string CalculatedDamageKey = "CalculatedDamage";

    public CompressVoid()
        : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(3M, ValueProp.Move),
        new CalculationBaseVar(0M),
        new CalculationExtraVar(1M),
        new CalculatedVar(CalculatedDamageKey)
            .WithMultiplier((card, _) => GetVoidAndCallCards(card.Owner).Count())
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var cardsToExhaust = GetVoidAndCallCards(Owner).ToList();

        foreach (CardModel card in cardsToExhaust)
        {
            await CardCmd.Exhaust(choiceContext, card);
        }

        int count = cardsToExhaust.Count;
        if (count <= 0 || cardPlay.Target == null)
            return;

        decimal damage = DynamicVars.Damage.BaseValue * count;

        await DamageCmd.Attack(damage)
            .FromCard(this,cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_blunt", tmpSfx: "blunt_attack.mp3")
            .Execute(choiceContext);
    }

    private static IEnumerable<CardModel> GetVoidAndCallCards(Player owner)
    {
        return owner.PlayerCombatState.AllCards
            .Where(c =>
                c.Pile.Type != PileType.Exhaust &&
                (c is VoidCard || c is BeckonCard)
            );
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1M);
    }
}