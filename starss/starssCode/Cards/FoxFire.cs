using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using starss.starssCode.Mechanics;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;



namespace starss.starssCode.Cards;


public sealed class FoxFire : starssCard
{
    public FoxFire()
        : base(7, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
    {
    }

    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(7M, ValueProp.Move),
        new DynamicVar("Hits", 7M),
        new FateVar(40M),
        new DoomVar(81M),
        new PowerVar<VulnerablePower>(2M),
        new PowerVar<WeakPower>(2M)
    ];

    public override async Task AfterCardDrawn(
        PlayerChoiceContext choiceContext,
        CardModel card,
        bool fromHandDraw)
    {
        if (card != this)
            return;

        EnergyCost.UpgradeBy(-2);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        
        
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this,cardPlay)
            .WithHitCount(DynamicVars["Hits"].IntValue)
            .TargetingAllOpponents(CombatState!)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        var check = await DiceHelper.Check(
            Owner.Creature,
            fate: DynamicVars["Fate"].IntValue,
            doom: DynamicVars["Doom"].IntValue,
            choiceContext: choiceContext,
            sourceCard: this
        );
        if (check.FateSuccess)
        {
           

            foreach (Creature enemy in CombatState!.HittableEnemies)
            {
                await PowerCmd.Apply<VulnerablePower>(
                    choiceContext,
                    enemy,
                    DynamicVars.Vulnerable.BaseValue,
                    Owner.Creature,
                    this
                );
            }
            
        }

        if (check.DoomSuccess)
        {
            
            await PowerCmd.Apply<WeakPower>(
                choiceContext,
                Owner.Creature,
                DynamicVars.Weak.BaseValue,
                Owner.Creature,
                this
            );
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-2);
    }
}