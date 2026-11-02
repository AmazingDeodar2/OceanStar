using MegaCrit.Sts2.Core.Commands; 
using MegaCrit.Sts2.Core.Entities.Cards; 
using MegaCrit.Sts2.Core.Entities.Players; 
using MegaCrit.Sts2.Core.Entities.Powers; 
using MegaCrit.Sts2.Core.GameActions.Multiplayer; 
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.HoverTips; 
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers; 
using MegaCrit.Sts2.Core.ValueProps; 
using System.Collections.Generic; 
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Models;

namespace starss.starssCode.Powers;


public sealed class TimeReversalPower : starssPower
{
    public override PowerType Type => PowerType.Debuff; 
    public override PowerStackType StackType => PowerStackType.Counter;
    
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => 
        [ 
            HoverTipFactory.FromPower<LuckyPower>()
        ];

    /// <summary>
    /// 每层每回合失去的幸运。默认值从打出它的「时光倒流」卡牌拷进来（见 <see cref="AfterApplied"/>）。
    /// 以前这里是一个硬编码 const，卡上的 Var 实际没被使用——现在两者统一。
    /// </summary>
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("TimeReversalLuckLoss", 10M)
    ];

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        if (cardSource != null && cardSource.DynamicVars.ContainsKey("TimeReversalLuckLoss"))
        {
            DynamicVars["TimeReversalLuckLoss"].BaseValue =
                cardSource.DynamicVars["TimeReversalLuckLoss"].BaseValue;
        }

        return Task.CompletedTask;
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        // 只在这个能力拥有者自己的回合开始时触发。
        if (player != Owner.Player) return; 
        
        Flash();
        
        // 每层减少一点幸运。例如 Amount 为 3，则本次减少 3 × 该值。
        decimal luckLoss = DynamicVars["TimeReversalLuckLoss"].BaseValue * Amount;

        await PowerCmd.Apply<LuckyPower>(
            new ThrowingPlayerChoiceContext(),
            Owner,
            -luckLoss,
            Owner,
            null
        );
        
    }
}
