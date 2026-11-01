using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using starss.starssCode.Powers;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.ValueProps;
// Void 与 System.Void 撞名，项目内统一用别名引用（见 Eloquence.cs / VoidStrike.cs）
using VoidCard = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace starss.starssCode.Cards;

public sealed class MakkaPakka : starssCard
{
    public MakkaPakka()
        : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
    }

    // MakkaPakkaPower 监听 AfterCardGeneratedForCombat，只对官方「虚空」牌生效
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<VoidCard>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        get
        {
            return new DynamicVar[]
            {
                new DamageVar("Damage", 6M, ValueProp.Unpowered)
            };
        }
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(
            Owner.Creature,
            "PowerUp",
            Owner.Character.PowerUpAnimDelay
        );

        await PowerCmd.Apply<MakkaPakkaPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars.Damage.BaseValue,
            Owner.Creature,
            this
        );
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Damage"].UpgradeValueBy(2M);
    }
}