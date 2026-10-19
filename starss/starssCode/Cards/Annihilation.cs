
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Models;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using VoidCard = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace starss.starssCode.Cards;

public sealed class Annihilation : starssCard
{
    public Annihilation()
        : base(
            1,
            CardType.Skill,
            CardRarity.Uncommon,
            TargetType.Self)
    {
    }

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(4M, ValueProp.Move)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        // 与原版 Stoke 保持一致，先播放施法动画
        await CreatureCmd.TriggerAnim(
            Owner.Creature,
            "Cast",
            Owner.Character.CastAnimDelay
        );

        // 获取当前手牌快照，排除湮灭自身
        List<CardModel> handCards = PileType.Hand
            .GetPile(Owner)
            .Cards
            .Where(card => card != this)
            .ToList();

        int exhaustedCount = handCards.Count;

        // 第一步：先消耗全部目标手牌
        foreach (CardModel card in handCards)
        {
            await CardCmd.Exhaust(
                choiceContext,
                card
            );
        }

        // 第二步：每消耗一张牌，获得一次格挡
        for (int i = 0; i < exhaustedCount; i++)
        {
            await CreatureCmd.GainBlock(
                Owner.Creature,
                DynamicVars.Block.BaseValue,
                ValueProp.Move,
                cardPlay
            );
        }

        // 第三步：生成与消耗数量相同的虚空
        if (exhaustedCount > 0)
        {
            List<CardModel> voidCards = new();

            for (int i = 0; i < exhaustedCount; i++)
            {
                CardModel voidCard =
                    CombatState.CreateCard<VoidCard>(Owner);

                voidCards.Add(voidCard);
            }

            // 与 Stoke 一样，一次性加入手牌
            await CardPileCmd.AddGeneratedCardsToCombat(
                voidCards,
                PileType.Hand,
                Owner
            );
        }

        // 不再手动调用 InvokeCardAddFinished()
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2M);
    }
}
