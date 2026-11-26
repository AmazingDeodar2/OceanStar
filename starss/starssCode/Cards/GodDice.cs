
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
using starss.starssCode.Extensions;

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

    // 当前神之骰的面数。
    private int CurrentDiceSides =>
        DynamicVars["Sides"].IntValue;

    // 大卡面：沿用 starssCard 的 BigCardImagePath。
    public override string CustomPortraitPath =>
        GetPortraitFilename(CurrentDiceSides)
            .BigCardImagePath();

    // 小卡面：沿用 starssCard 的 CardImagePath。
    public override string PortraitPath =>
        GetPortraitFilename(CurrentDiceSides)
            .CardImagePath();

    // 预加载全部六档大小卡面。
    public override string[] AllPortraitPaths =>
    [
        // 小卡面
        GetPortraitFilename(6).CardImagePath(),
        GetPortraitFilename(12).CardImagePath(),
        GetPortraitFilename(20).CardImagePath(),
        GetPortraitFilename(30).CardImagePath(),
        GetPortraitFilename(50).CardImagePath(),
        GetPortraitFilename(100).CardImagePath(),

        // 大卡面
        GetPortraitFilename(6).BigCardImagePath(),
        GetPortraitFilename(12).BigCardImagePath(),
        GetPortraitFilename(20).BigCardImagePath(),
        GetPortraitFilename(30).BigCardImagePath(),
        GetPortraitFilename(50).BigCardImagePath(),
        GetPortraitFilename(100).BigCardImagePath()
    ];

    // 根据当前骰子面数获取图片文件名。
    private static string GetPortraitFilename(int sides)
    {
        return sides switch
        {
            6 => "god_dice_6.png",
            12 => "god_dice_12.png",
            20 => "god_dice_20.png",
            30 => "god_dice_30.png",
            50 => "god_dice_50.png",
            100 => "god_dice_100.png",
            _ => "god_dice_6.png"
        };
    }

    // =========================================================
    // 动态变量
    // =========================================================

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        // 当前伤害骰的面数。
        new DynamicVar("Sides", 6M),

        // 命运与厄运检定值。
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

            // 复制当前面数，同时继承对应阶段的卡面。
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

        // 已经是 D100 时不再变化。
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
