using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

namespace starss.starssCode.Relics;

public sealed class CreepingVine : RelicModel
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool HasUponPickupEffect => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[]
        {
            new CardsVar(3)
        };

    public override async Task AfterObtained()
    {
        // 1. 创建普通卡牌选择界面
        CardSelectorPrefs prefs = new CardSelectorPrefs(
            CardSelectorPrefs.TransformSelectionPrompt,
            DynamicVars.Cards.IntValue
        );

        // 2. 玩家从牌组中选择三张可以转化的牌
        // 不使用 FromDeckForTransformation，
        // 避免出现随机转化预览动画
        List<CardModel> selectedCards =
            (await CardSelectCmd.FromDeckGeneric(
                Owner,
                prefs,
                card => card.Type != CardType.Quest
                        && card.IsTransformable
            )).ToList();

        // 3. 将选中的卡牌转化为「笨拙」
        foreach (CardModel original in selectedCards)
        {
            // 使用游戏原版的卡牌创建方式
            // 确保新卡牌与原卡牌属于同一玩家
            CardModel clumsy =
                original.CardScope.CreateCard<Clumsy>(
                    original.Owner
                );

            // None：不显示转化结果动画
            await CardCmd.Transform(
                original,
                clumsy,
                CardPreviewStyle.None
            );
        }

        // 4. 播放遗物闪光效果
        Flash();
    }
}