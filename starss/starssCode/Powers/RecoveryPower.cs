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


public sealed class RecoveryPower : starssPower
{
    public override PowerType Type => PowerType.Buff; 
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => 
        [ 
            HoverTipFactory.FromPower<LuckyPower>(), 
            HoverTipFactory.FromPower<VigorPower>(), 
            HoverTipFactory.Static(StaticHoverTip.Block) 
        ];

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        // 只在这个能力拥有者自己的回合开始时触发。
        if (player != Owner.Player) return; 
        
        Flash();
        
        decimal stacks = Amount;
        // 每层再生失去1点幸运。
        await PowerCmd.Apply<LuckyPower>(
            choiceContext,
            Owner,
            -1M * stacks, 
            Owner,
            (CardModel?)null 
            );
        // 每层再生获得6点格挡。
       
        await CreatureCmd.GainBlock(
            Owner, 
            6M * stacks,
            ValueProp.Unpowered, 
            (CardPlay?)null 
            );
        
        
        // 每层再生获得3点原版活力。
        await PowerCmd.Apply<VigorPower>( 
            choiceContext, 
            Owner, 
            3M * stacks, 
            Owner, 
            (CardModel?)null
            );
        
    }
}
