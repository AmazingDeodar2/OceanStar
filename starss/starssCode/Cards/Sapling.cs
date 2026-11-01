using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using starss.starssCode.Powers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace starss.starssCode.Cards;

public sealed class Sapling : starssCard
{
    public Sapling()
        : base(
            1,
            CardType.Skill,
            CardRarity.Rare,
            TargetType.Self)
    {
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<CloverLeaf>()
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        if (CombatState is null)
            return;

        // 1. 将一张四叶草叶片加入手牌
        CloverLeaf clover =
            CombatState.CreateCard<CloverLeaf>(Owner);

        await CardPileCmd.AddGeneratedCardToCombat(
            clover,
            PileType.Hand,
            Owner
        );

        PileType.Hand.GetPile(Owner)
            .InvokeCardAddFinished();

        // 2. 本场战斗中四叶草叶片额外重放次数 +1
        await PowerCmd.Apply<SaplingPower>(
            choiceContext,
            Owner.Creature,
            1M,
            Owner.Creature,
            this
        );
    }

    // 升级后移除消耗
    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}