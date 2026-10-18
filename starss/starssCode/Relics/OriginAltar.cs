using System.Linq;
using System.Threading.Tasks;

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

using starss.starssCode.Cards;

namespace starss.starssCode.Relics;

public sealed class OriginAltar : RelicModel
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool HasUponPickupEffect => true;

    public override async Task AfterObtained()
    {
        var player = Owner;

        // 1. 找到并移除玩家的初始遗物
        RelicModel? starterRelic = player.Relics.FirstOrDefault(
            relic => relic.Rarity == RelicRarity.Starter
        );

        if (starterRelic != null)
        {
            await RelicCmd.Remove(starterRelic);
        }

        // 2. 获得一件普通遗物
        await ObtainRandomRelic(RelicRarity.Common);

        // 3. 获得一件罕见遗物
        await ObtainRandomRelic(RelicRarity.Uncommon);

        // 4. 获得一件稀有遗物
        await ObtainRandomRelic(RelicRarity.Rare);

        // 5. 创建一张自定义诅咒「寄生」
        CardModel parasite = player.RunState.CreateCard(
            ModelDb.Card<Parasite>(),
            player
        );

        // 6. 将「寄生」加入永久牌组
        CardPileAddResult result = await CardPileCmd.Add(
            parasite,
            PileType.Deck
        );

        // 7. 显示获得卡牌的预览
        CardCmd.PreviewCardPileAdd(
            new[] { result },
            2f
        );
    }

    private async Task ObtainRandomRelic(RelicRarity rarity)
    {
        var player = Owner;

        // 从玩家的正常遗物池抽取
        RelicModel? relic = player.RelicGrabBag.PullFromFront(
            rarity,
            candidate => candidate.Rarity == rarity,
            player.RunState
        );

        if (relic == null)
        {
            return;
        }

        // 创建可变遗物实例
        RelicModel mutableRelic = relic.ToMutable();

        // 正常获得遗物
        await RelicCmd.Obtain(mutableRelic, player);
    }
}