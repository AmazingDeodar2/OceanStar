using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;

namespace starss.starssCode.Cards;

public sealed class RankSuppression : starssCard
{
    public RankSuppression()
        : base(
            2,
            CardType.Attack,
            CardRarity.Uncommon,
            TargetType.AnyEnemy)
    {
    }
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    // 显示击晕和呼唤的悬浮提示
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        StunIntent.GetStaticHoverTip(),
        HoverTipFactory.FromCard<Beckon>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        // 确保选中了有效目标
        ArgumentNullException.ThrowIfNull(
            cardPlay.Target,
            nameof(cardPlay.Target)
        );

        // 1. 击晕目标敌人
        await CreatureCmd.Stun(cardPlay.Target);

        // 2. 创建一张「呼唤」
        CardModel beckonCard =
            Owner.Creature.CombatState.CreateCard<Beckon>(Owner);

        // 3. 将「呼唤」加入手牌
        await CardPileCmd.AddGeneratedCardToCombat(
            beckonCard,
            PileType.Hand,
            Owner
        );

        // 4. 通知手牌区域刷新
        PileType.Hand.GetPile(Owner).InvokeCardAddFinished();
    }

    // 升级后：2费 → 1费
    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}