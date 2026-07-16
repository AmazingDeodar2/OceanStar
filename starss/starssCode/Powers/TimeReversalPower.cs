using MegaCrit.Sts2.Core.Commands; 
using MegaCrit.Sts2.Core.Entities.Cards; 
using MegaCrit.Sts2.Core.Entities.Players; 
using MegaCrit.Sts2.Core.Entities.Powers; 
using MegaCrit.Sts2.Core.GameActions.Multiplayer; 
using MegaCrit.Sts2.Core.HoverTips; 
using MegaCrit.Sts2.Core.Models.Powers; 
using MegaCrit.Sts2.Core.ValueProps; 
using System.Collections.Generic; 
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Models;

namespace starss.starssCode.Powers;


public sealed class TimeReversalPower : starssPower
{
    private const decimal LuckLossPerStack = 10M;
    
    public override PowerType Type => PowerType.Debuff; 
    public override PowerStackType StackType => PowerStackType.Counter;
    
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => 
        [ 
            HoverTipFactory.FromPower<LuckyPower>()
        ];

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        // 只在这个能力拥有者自己的回合开始时触发。
        if (player != Owner.Player) return; 
        
        Flash();
        
        // 每层减少10点幸运。
        // 例如Amount为3，则本次减少30点幸运。
        decimal luckLoss = LuckLossPerStack * Amount;

        await PowerCmd.Apply<LuckyPower>(
            new ThrowingPlayerChoiceContext(),
            Owner,
            -luckLoss,
            Owner,
            null
        );
        
    }
}
