using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using starss.starssCode;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using starss.starssCode.Mechanics;
using VoidCard = MegaCrit.Sts2.Core.Models.Cards.Void;
using BeckonCard = MegaCrit.Sts2.Core.Models.Cards.Beckon;
namespace starss.starssCode.Cards;


    public sealed class AbyssJudgment : starssCard
    {
        public AbyssJudgment()
            : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
        {
        }

        protected override IEnumerable<DynamicVar> CanonicalVars =>
        [
            new CalculationBaseVar(6M),
            new ExtraDamageVar(4M),
            new CalculatedDamageVar(ValueProp.Move)
                .WithMultiplier((card, _) => CountVoidAndBeckon(card))
        ];

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            ArgumentNullException.ThrowIfNull(cardPlay.Target);

            await DamageCmd.Attack(DynamicVars.CalculatedDamage)
                .FromCard(this, cardPlay)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }

        private static decimal CountVoidAndBeckon(CardModel card)
        {
            return
                PileType.Hand.GetPile(card.Owner).Cards.Count(IsVoidOrBeckon) +
                PileType.Draw.GetPile(card.Owner).Cards.Count(IsVoidOrBeckon) +
                PileType.Discard.GetPile(card.Owner).Cards.Count(IsVoidOrBeckon) +
                PileType.Exhaust.GetPile(card.Owner).Cards.Count(IsVoidOrBeckon);
        }

        private static bool IsVoidOrBeckon(CardModel card)
        {
            return card is VoidCard || card is BeckonCard;
        }

        protected override void OnUpgrade()
        {
            DynamicVars.ExtraDamage.UpgradeValueBy(1M);
        }
    }
