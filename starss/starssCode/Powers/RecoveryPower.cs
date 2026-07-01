using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace starss.starssCode.Powers;


public sealed class RecoveryPower : starssPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (target != Owner)
            return;

        // 只在自己的回合
        if (CombatState.CurrentSide != Owner.Side)
            return;

        // 必须真的掉了血
        if (result.UnblockedDamage <= 0)
            return;

        // 立即恢复1点生命
        await CreatureCmd.Heal(
            Owner,
            1M
        );

        // 获得敏捷
        DexterityPower dexterityPower = await PowerCmd.Apply<DexterityPower>(
            choiceContext,
            Owner,
            Amount,
            Owner,
            (CardModel?)null
        );
    }
}
