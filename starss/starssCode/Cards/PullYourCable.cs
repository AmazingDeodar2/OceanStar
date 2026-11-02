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

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<PoisonPower>(),
        HoverTipFactory.FromPower<DoomPower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        // 需要丢弃的张数
        new CardsVar(2),
        // 每抽到一张牌，给全体敌人上的层数
        new PowerVar<PoisonPower>("PoisonPower", 1M),
        new PowerVar<DoomPower>("DoomPower", 1M)
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

        IReadOnlyList<Creature> enemies =
            Owner.Creature.CombatState?.HittableEnemies ?? Array.Empty<Creature>();

        // 3) 逐张结算。
        //    Draw 会返回本次真正抽到的牌，天然只作用于「这一次抽牌」——所以不需要 Power：
        //    官方腐蚀波那种 Power 挂的是 AfterCardDrawn，本回合任何来源的抽牌都会触发，
        //    而且要到回合结束才移除，语义完全不一样。
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
    }

    protected override void OnUpgrade()
    {
        DynamicVars["PoisonPower"].UpgradeValueBy(1M);
        DynamicVars["DoomPower"].UpgradeValueBy(1M);
    }
}
