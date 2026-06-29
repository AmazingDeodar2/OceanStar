using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.ValueProps;
using starss.starssCode.Mechanics;

namespace starss.starssCode.Cards;


public sealed class FirstAid : starssCard
{
    public FirstAid()
        : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
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
        var fateCheck = await DiceHelper.Check(
            Owner.Creature,
            fate: DynamicVars["Fate"].IntValue,
            doom: 101,
            choiceContext: choiceContext,
            sourceCard: this
        );

        if (fateCheck.FateSuccess)
        {
            await DiceHelper.OnFateTriggered(choiceContext, this);

            await CreatureCmd.Heal(
                Owner.Creature,
                DynamicVars.Heal.BaseValue
            );
        }

        var doomCheck = await DiceHelper.Check(
            Owner.Creature,
            fate: 101,
            doom: DynamicVars.Doom.IntValue,
            choiceContext: choiceContext,
            sourceCard: this
        );

        if (doomCheck.DoomSuccess)
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
                this
            );
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Heal.UpgradeValueBy(2M);
    }
}