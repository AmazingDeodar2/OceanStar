using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using starss.starssCode.Mechanics;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace starss.starssCode.Cards;


public sealed class WitnessChance : starssCard
{
    public WitnessChance()
        : base(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
    }

    

    protected override IEnumerable<DynamicVar> CanonicalVars => new List<DynamicVar>();

    // 存储四颗骰子结果，方便外部读取、弹窗展示点数
    public DiceRollResult RollD3Result { get; private set; }
    public DiceRollResult RollD6Result { get; private set; }
    public DiceRollResult RollD10Result { get; private set; }
    public DiceRollResult RollD20Result { get; private set; }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var user = Owner.Creature;
        var target = cardPlay.Target!;

        // 分别掷 三面 / 六面 / 十面 / 二十面
        RollD3Result = DiceHelper.RollD3(user, this);
        RollD6Result = DiceHelper.RollD6(user, this);
        RollD10Result = DiceHelper.RollD10(user, this);
        RollD20Result = DiceHelper.RollD20(user, this);

        // 单颗骰子最终有效点数（奖励骰取最小之后的值）
        int valD3 = RollD3Result.Value;
        int valD6 = RollD6Result.Value;
        int valD10 = RollD10Result.Value;
        int valD20 = RollD20Result.Value;

        int totalDmg = valD3 + valD6 + valD10 + valD20;

        // 如果你需要后续弹窗打印每一颗原始投点列表：
        // RollD3Result.Rolls 就是该骰子全部投出的数组

        var attackCmd = await DamageCmd.Attack((decimal)totalDmg)
            .FromCard(this)
            .Targeting(cardPlay.Target!)
            .WithHitFx("vfx/vfx_attack_random")
            .Execute(choiceContext);

        bool targetKilled = attackCmd.Results
            .SelectMany(x => x)
            .Any(r => r.WasTargetKilled);

        // 3. 获取当前战斗房间，斩杀则添加额外奖励（原版TheHunt同款AddExtraReward）
        if (targetKilled && CombatState.RunState.CurrentRoom is CombatRoom combatRoom)
        {
            // 额外一轮卡牌奖励，配置同TheHunt房间卡牌奖励生成规则
            combatRoom.AddExtraReward(Owner, new CardReward(
                CardCreationOptions.ForRoom(Owner, combatRoom.RoomType),
                1,
                Owner
            ));
        }
    }

    protected override void OnUpgrade()
    {
        this.EnergyCost.UpgradeBy(-1);
    }
}