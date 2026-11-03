using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace starss.starssCode.Cards;


public sealed class PullYourCable : starssCard
{
    public PullYourCable()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    // 「每抽到一张牌 → 全体敌人中毒+灾厄」已按需求暂时关闭，tip 一并注释。
    // 恢复效果时取消下面整块注释即可。
    // protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    // [
    //     HoverTipFactory.FromPower<PoisonPower>(),
    //     HoverTipFactory.FromPower<DoomPower>()
    // ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        // 需要丢弃的张数
        new CardsVar(2)
        // 效果已关闭；恢复时补回下面两行（并给上一行补逗号）
        // new PowerVar<PoisonPower>("PoisonPower", 1M),
        // new PowerVar<DoomPower>("DoomPower", 1M)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(
            Owner.Creature,
            "Cast",
            Owner.Character.CastAnimDelay
        );

        // 1) 弃牌：玩家从手牌里选。多张必须走集合重载（不要循环调单张版本）。
        await CardCmd.Discard(
            choiceContext,
            await CardSelectCmd.FromHandForDiscard(
                choiceContext,
                Owner,
                new CardSelectorPrefs(
                    CardSelectorPrefs.DiscardSelectionPrompt,
                    DynamicVars.Cards.IntValue
                ),
                null,
                this
            )
        );

        // 2) 抽满手牌。差值必须在弃牌**之后**算，否则会把刚弃掉的牌又算进手牌数。
        int num = CardPile.MaxCardsInHand - PileType.Hand.GetPile(Owner).Cards.Count;

        if (num <= 0)
            return;

        await CardPileCmd.Draw(choiceContext, num, Owner);

        // 3) 【已注释】每抽到一张牌，给予所有敌人中毒与灾厄。
        //    恢复时取消整块注释，并把上面的 Draw 改回 foreach 形式。
        /*
        IReadOnlyList<Creature> enemies =
            Owner.Creature.CombatState?.HittableEnemies ?? Array.Empty<Creature>();

        foreach (CardModel _ in await CardPileCmd.Draw(choiceContext, num, Owner))
        {
            await PowerCmd.Apply<PoisonPower>(
                choiceContext,
                enemies,
                DynamicVars["PoisonPower"].BaseValue,
                Owner.Creature,
                this
            );

            await PowerCmd.Apply<DoomPower>(
                choiceContext,
                enemies,
                DynamicVars["DoomPower"].BaseValue,
                Owner.Creature,
                this
            );
        }
        */
    }

    protected override void OnUpgrade()
    {
        // 升级：获得「保留」（项目先例 Exorcise.cs:68 / Gratitude.cs:55）
        AddKeyword(CardKeyword.Retain);

        // 原升级（中毒/灾厄层数 +1）随效果一起注释
        // DynamicVars["PoisonPower"].UpgradeValueBy(1M);
        // DynamicVars["DoomPower"].UpgradeValueBy(1M);
    }
}
