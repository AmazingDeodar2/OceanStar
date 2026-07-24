using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace starss.starssCode.Cards.ReputationServices;

[Pool(typeof(TokenCardPool))]
public sealed class RandomRelicService :
    starssCard,
    IReputationService
{
    public RandomRelicService()
        : base(
            0,
            CardType.Status,
            CardRarity.Token,
            TargetType.None)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Price", 150M)
    ];

    public async Task OnChosen()
    {
        int price = DynamicVars["Price"].IntValue;

        // TODO：这里替换成你项目实际的金币读取方式
        if (Owner.Gold < price)
            return;

        // TODO：这里替换成实际扣钱 API
        await PlayerCmd.LoseGold(
            price,
            Owner
        );

        var relic = RelicFactory.PullNextRelicFromFront(Owner);

        await RelicCmd.Obtain(
            relic.ToMutable(),
            Owner
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