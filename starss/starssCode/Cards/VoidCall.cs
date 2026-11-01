using System.Collections.Generic;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;
using starss.starssCode.Powers;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
// Void 与 System.Void 撞名，项目内统一用别名引用（见 Eloquence.cs / VoidStrike.cs）
using VoidCard = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace starss.starssCode.Cards;

public sealed class VoidCall : starssCard
{
    public VoidCall()
        : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(
            Owner.Creature,
            "PowerUp",
            Owner.Character.PowerUpAnimDelay);

        await PowerCmd.Apply<VoidCallPower>(
            choiceContext,
            Owner.Creature,
            1M,
            Owner.Creature,
            this);
    }
    // VoidCallPower 监听 AfterCardDrawn，虚空牌与呼唤牌都会触发 ⇒ 两张牌的预览都要给
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<VoidCard>(),
        HoverTipFactory.FromCard<Beckon>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(1)
    ];
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}