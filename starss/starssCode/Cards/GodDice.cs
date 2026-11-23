
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using starss.starssCode.Mechanics;
using starss.starssCode.Cards.Interfaces;

namespace starss.starssCode.Cards;

[Pool(typeof(TokenCardPool))]
public sealed class GodDice : starssCard, IDoomCard, IFateCard
{
    private static readonly int[] DiceStages =
    [
        6,
        12,
        20,
        30,
        50,
        100
    ];

    // 六张卡面的存放目录
    private const string PortraitDirectory =
        "res://starss/images/card_portraits/big/";

    public GodDice()
        : base(
            0,
            CardType.Curse,
            CardRarity.Curse,
            TargetType.AnyEnemy)
    {
    }

    public override CardMultiplayerConstraint MultiplayerConstraint =>
        CardMultiplayerConstraint.MultiplayerOnly;

    // =========================================================
    // 六档卡面
    // =========================================================

    // 注册全部六张卡面，供资源系统预加载
    public override string[] AllPortraitPaths =>
    [
        GetDicePortraitPath(6),
        GetDicePortraitPath(12),
        GetDicePortraitPath(20),
        GetDicePortraitPath(30),
        GetDicePortraitPath(50),
        GetDicePortraitPath(100)
    ];

    // 根据当前骰子面数选择卡面
    public override string PortraitPath =>
        GetDicePortraitPath(DynamicVars["Sides"].IntValue);

    // 对应的大卡面 PNG 路径
    protected override string PortraitPngPath =>
        GetDicePortraitPath(DynamicVars["Sides"].IntValue);

    private static string GetDicePortraitPath(int sides)
    {
        return sides switch
        {
            6 => PortraitDirectory + "god_dice_6.png",
            12 => PortraitDirectory + "god_dice_12.png",
            20 => PortraitDirectory + "god_dice_20.png",
            30 => PortraitDirectory + "god_dice_30.png",
            50 => PortraitDirectory + "god_dice_50.png",
            100 => PortraitDirectory + "god_dice_100.png",
            _ => PortraitDirectory + "god_dice_6.png"
        };
    }

    // =========================================================
    // 动态变量
    // =========================================================

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        // 当前伤害骰的面数
        new DynamicVar("Sides", 6M),

        // 命运与厄运检定值
        new FateVar(50M),
        new DoomVar(51M)
    ];

    // =========================================================
    // 卡牌效果
    // =========================================================

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        int sides = DynamicVars["Sides"].IntValue;

        /*
         * 第一次掷骰：
         * 按当前骰子面数投掷，结果用于造成伤害。
         */
        DiceRollResult damageRoll = RollDamageDie(sides);

        await DiceUi.ShowRoll(damageRoll);

        await CreatureCmd.Damage(
            choiceContext,
            cardPlay.Target,
            damageRoll.Value,
            ValueProp.Move,
            this,
            cardPlay
        );

        /*
         * 第二次掷骰：
         * 独立进行命运与厄运检定。
         * DiceHelper.Check 会处理幸运值等修正。
         */
        DiceCheckResult check = await DiceHelper.Check(
            Owner.Creature,
            fate: DynamicVars["Fate"].IntValue,
            doom: DynamicVars["Doom"].IntValue,
            choiceContext: choiceContext,
            sourceCard: this
        );

        /*
         * 命运成功：
         * 仅改变当前这张神之骰实例的面数。
         *
         * D6 → D12 → D20 → D30 → D50 → D100
         */
        for (int i = 0;
             i < FateEffect.GetRepeatCount(check, Owner.Creature);
             i++)
        {
            AdvanceDiceStage();
        }

        /*
         * 厄运成功：
         * 复制一张神之骰进入弃牌堆。
         */
        if (check.DoomSuccess)
        {
            GodDice copy =
                CombatState.CreateCard<GodDice>(Owner);

            /*
             * 复制当前面数。
             *
             * 因为命运效果先结算，所以若本次命运与厄运
             * 同时成功，复制品会继承提升后的面数。
             */
            copy.DynamicVars["Sides"].BaseValue =
                DynamicVars["Sides"].BaseValue;

            await CardPileCmd.AddGeneratedCardToCombat(
                copy,
                PileType.Discard,
                Owner
            );
        }
    }

    // =========================================================
    // 骰子升级
    // =========================================================

    private void AdvanceDiceStage()
    {
        int currentSides =
            DynamicVars["Sides"].IntValue;

        for (int i = 0; i < DiceStages.Length - 1; i++)
        {
            if (DiceStages[i] != currentSides)
                continue;

            DynamicVars["Sides"].BaseValue =
                DiceStages[i + 1];

            return;
        }

        // 已经是 D100 时不再变化
    }

    // =========================================================
    // 掷骰
    // =========================================================

    private DiceRollResult RollDamageDie(int sides)
    {
        return sides switch
        {
            6 => DiceHelper.RollD6(
                Owner.Creature,
                this
            ),

            12 => DiceHelper.RollD12(
                Owner.Creature,
                this
            ),

            20 => DiceHelper.RollD20(
                Owner.Creature,
                this
            ),

            30 => DiceHelper.RollD30(
                Owner.Creature,
                this
            ),

            50 => DiceHelper.RollD50(
                Owner.Creature,
                this
            ),

            100 => DiceHelper.RollD100(
                Owner.Creature,
                this
            ),

            _ => DiceHelper.RollD6(
                Owner.Creature,
                this
            )
        };
    }

    // =========================================================
    // 常规升级
    // =========================================================

    protected override void OnUpgrade()
    {
        // 神之骰是诅咒牌，不使用常规卡牌升级。
    }
}
