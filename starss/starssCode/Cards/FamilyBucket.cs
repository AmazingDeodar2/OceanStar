using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using starss.starssCode.Powers;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Models.Cards;

namespace starss.starssCode.Cards;

public sealed class FamilyBucket : starssCard
{
    public FamilyBucket()
        : base(0, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get
        {
            return new DynamicVar[]
            {
                new PowerVar<LuckyPower>(2M),
                new EnergyVar(2)
            };
        }
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(
            Owner.Creature,
            "PowerUp",
            Owner.Character.PowerUpAnimDelay
        );

        await PowerCmd.Apply<LuckyPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars["LuckyPower"].BaseValue,
            Owner.Creature,
            this
        );

        Owner.PlayerCombatState.GainEnergy(DynamicVars.Energy.BaseValue);

        await CardPileCmd.Draw(choiceContext, 2M, Owner);

        await PowerCmd.Apply<StrengthPower>(
            choiceContext,
            Owner.Creature,
            1M,
            Owner.Creature,
            this
        );

        await PowerCmd.Apply<DexterityPower>(
            choiceContext,
            Owner.Creature,
            1M,
            Owner.Creature,
            this
        );

        await PowerCmd.Apply<PlatingPower>(
            choiceContext,
            Owner.Creature,
            2M,
            Owner.Creature,
            this
        );

        await PowerCmd.Apply<VigorPower>(
            choiceContext,
            Owner.Creature,
            2M,
            Owner.Creature,
            this
        );

        CardModel callCard = Owner.Creature.CombatState.CreateCard<Beckon>(Owner);

        await CardPileCmd.AddGeneratedCardToCombat(
            callCard,
            PileType.Hand,
            Owner
        );
    }

    protected override void OnUpgrade()
    {
        DynamicVars["LuckyPower"].UpgradeValueBy(3M);
        DynamicVars.Energy.UpgradeValueBy(1M);
    }
}