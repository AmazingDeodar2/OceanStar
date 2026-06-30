using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using System;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Models;

namespace starss.starssCode.Powers;


public sealed class FreeNextCardPower : PowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    
    
    
    public override bool TryModifyEnergyCostInCombatLate(
        CardModel card,
        Decimal originalCost,
        out Decimal modifiedCost)
    {
        modifiedCost = originalCost;

        // 不是持有者的牌，直接不生效
        if (card.Owner.Creature != Owner)
            return false;

        // 仅允许手牌 / 出牌区卡牌生效，和官方逻辑对齐
        PileType? pileType = card.Pile?.Type;
        bool isValidPile = pileType is PileType.Hand or PileType.Play;
        if (!isValidPile)
            return false;

        modifiedCost = 0M;
        return true;
    }

    /// <summary>
    /// 确认本张牌确实吃了免费之后，移除本Power（Single一次性）
    /// 对应官方 BeforeCardPlayed 扣层逻辑
    /// </summary>
    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        // 身份校验不一致直接退出
        if (cardPlay.Card.Owner.Creature != Owner)
            return;

        PileType? pileType = cardPlay.Card.Pile?.Type;
        bool isValidPile = pileType is PileType.Hand or PileType.Play;
        if (!isValidPile)
            return;

        // Single类型，用完直接移除自身
        await PowerCmd.Remove(this);
    }
}