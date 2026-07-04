using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Combat;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace starss.starssCode.Cards;


public sealed class CthulhuMyth : starssCard
{
    public CthulhuMyth()
        : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    /// 变量对齐 BrightestFlame：MaxHpVar 用来表示扣除的生命上限数值
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new MaxHpVar(15M)
    ];

    

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 原版标准扣生命上限指令，第三个参数为扣除数值，第四个skipVisuals
        await CreatureCmd.LoseMaxHp(
            choiceContext,
            Owner.Creature,
            DynamicVars.MaxHp.BaseValue,
            false
        );
        ICombatState? combatState = Owner.Creature.CombatState;
        if (combatState == null)
            return;
        // 判断当前房间是否为Boss房，非Boss直接胜利结算
        AbstractRoom currentRoom = combatState.RunState.CurrentRoom;
        if (currentRoom != null && currentRoom.RoomType != RoomType.Boss)
        {
            foreach (Creature enemy in combatState.HittableEnemies.ToList())
            {
                await CreatureCmd.Kill(enemy, false);
            }
        }
    }

    /// 升级示例：减少扣除上限代价（可按需自行修改）
    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Ethereal);
    }
}