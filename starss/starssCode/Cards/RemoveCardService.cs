using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;

namespace starss.starssCode.Cards.ReputationServices;

[Pool(typeof(TokenCardPool))]
public sealed class RemoveCardService :
    starssCard,
    IReputationService
{
    public RemoveCardService()
        : base(
            0,
            CardType.Status,
            CardRarity.Token,
            TargetType.None)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Price", 100M)
    ];

    public async Task OnChosen()
    {
        int price = DynamicVars["Price"].IntValue;

        // 金币不足，暂时不执行服务。
        if (Owner.Gold < price)
            return;

        // 只有战斗房间才能追加奖励。
        if (Owner.RunState.CurrentRoom is not CombatRoom combatRoom)
            return;

        await PlayerCmd.LoseGold(
            price,
            Owner
        );

        combatRoom.AddExtraReward(
            Owner,
            new CardRemovalReward(Owner)
        );
    }

    protected override Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        return Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
    }
}