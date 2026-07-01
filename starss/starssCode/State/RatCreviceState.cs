using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using starss.starssCode.Mechanics;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Models;

namespace starss.starssCode.States;

public sealed class RatCreviceState : StateModel
{
    private readonly LocString _selectText = new LocString("gameplay_ui", "CHOOSE_CARD_HEADER");

    public override string Id => "starss:rat_crevice";

    public override string DisplayName => "老鼠狭缝";
    
    public RatCreviceState()
    {
        Duration = int.MaxValue;
    }

    

    public override async Task OnEnter(PlayerChoiceContext choiceContext)
    {
        // 匹配FromCombatPile内部防御：战斗收尾直接退出，防止UI错乱卡死
        if (CombatManager.Instance.IsEnding || CombatManager.Instance.IsOverOrEnding)
            return;

        var drawPile = PileType.Draw.GetPile(Owner);
        // 牌堆为空提前跳过，避免引擎触发软锁日志、空选择界面挂住
        if (drawPile.Cards.Count == 0)
            return;

        // 移除爆红的AllowCancel赋值，仅保留基础构造
        CardSelectorPrefs prefs = new CardSelectorPrefs(_selectText, 1);

        var selectedCards = await CardSelectCmd.FromCombatPile(
            choiceContext,
            drawPile,
            Owner,
            prefs
        );

        var picked = selectedCards.FirstOrDefault();
        if (picked == null)
            return;

        // State上下文必须携带choiceContext，解决指令入队卡死问题
        picked.RemoveFromCurrentPile();
        await CardPileCmd.Add(picked, PileType.Hand);
    }


    public override async Task OnExit(PlayerChoiceContext choiceContext)
    {
        var discardPile = PileType.Discard.GetPile(Owner);
        if (discardPile.Cards.Count <= 0)
            return;

        CardSelectorPrefs prefs = new CardSelectorPrefs(_selectText, 1);

        var selectedCards = await CardSelectCmd.FromCombatPile(
            choiceContext,
            discardPile,
            Owner,
            prefs
        );
        CardModel? picked = selectedCards.FirstOrDefault();
        if (picked == null)
            return;

        picked.RemoveFromCurrentPile();
        await CardPileCmd.Add(picked, PileType.Hand);
    }
}