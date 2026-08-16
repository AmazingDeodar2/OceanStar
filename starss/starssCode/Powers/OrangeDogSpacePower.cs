using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using starss.starssCode.Mechanics;
using starss.starssCode.States;

namespace starss.starssCode.Powers;


public sealed class OrangeDogSpacePower : starssPower
{
    private decimal _absorbedDamage;


    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;


    public override decimal ModifyHpLostAfterOstyLate(
        Creature target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (target != Owner)
            return amount;


        if (amount <= 0)
            return amount;


        // 记录本次吸收的伤害
        _absorbedDamage = amount;


        // 取消伤害
        return 0M;
    }


    public override async Task AfterModifyingHpLostAfterOsty()
    {
        if (_absorbedDamage <= 0)
            return;

        int damage = (int)_absorbedDamage;

        _absorbedDamage = 0;


        var lucky = Owner.GetPower<LuckyPower>();

        if (lucky == null)
            return;


        Flash();


        await PowerCmd.Apply<LuckyPower>(
            new BlockingPlayerChoiceContext(),
            Owner,
            -damage,
            Owner,
            null
        );


        var currentLucky =
            Owner.GetPower<LuckyPower>();

        if (currentLucky == null || currentLucky.Amount <= 0)
        {
            var space =
                StateRegistry.Get(Owner.Player);

            var state =
                space.Get<OrangeDogSpaceState>();

            if (state != null)
            {
                await space.Exit(
                    new BlockingPlayerChoiceContext(),
                    state
                );
            }
        }
    }
}