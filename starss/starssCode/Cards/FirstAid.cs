using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.ValueProps;
using starss.starssCode.Mechanics;

namespace starss.starssCode.Cards;


public sealed class FirstAid : starssCard
{
    public FirstAid()
        : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new FateVar(60M),
        new DoomVar(80M),
        new HealVar(6M)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        
        var check = await DiceHelper.Check(
            Owner.Creature,
            fate: DynamicVars["Fate"].IntValue,
            doom: DynamicVars["Doom"].IntValue,
            choiceContext: choiceContext,
            sourceCard: this
        );
        
        if (check.FateSuccess)
        {
            

            await CreatureCmd.Heal(
                Owner.Creature,
                DynamicVars.Heal.BaseValue
            );
        }
        if (check.HardSuccess)
        {
            

            await CreatureCmd.Heal(
                Owner.Creature,
                DynamicVars.Heal.BaseValue
            );
        }

        if (check.DoomSuccess)
        {
            
            VfxCmd.PlayOnCreatureCenter(
                Owner.Creature,
                "vfx/vfx_bloody_impact"
            );

            await CreatureCmd.Damage(
                choiceContext,
                Owner.Creature,
                DynamicVars.Heal.BaseValue,
                ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move,
                this,
                cardPlay
            );
            
            
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Heal.UpgradeValueBy(2M);
    }
}