using System.Collections.Generic;
using System.Threading.Tasks;

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

using starss.starssCode.Powers;

namespace starss.starssCode.Cards;


public sealed class Estimate : starssCard
{
    public Estimate()
        : base(
            2,
            CardType.Skill,
            CardRarity.Uncommon,
            TargetType.AnyAlly)
    {
    }

    public override CardMultiplayerConstraint MultiplayerConstraint
        => CardMultiplayerConstraint.MultiplayerOnly;

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        // 获取选择的队友
        Player? target = cardPlay.Target?.Player;

        // 防止目标无效或选择自己
        if (target == null || target == Owner)
            return;

        // 金币不足10，不产生任何效果
        if (target.Gold < 10)
            return;

        // 队友支付10金币
        await PlayerCmd.LoseGold(
            10,
            target,
            GoldLossType.Spent
        );

        // 自己获得10金币
        await PlayerCmd.GainGold(
            10,
            Owner
        );

        // 给予队友额外回合资格
        await PowerCmd.Apply<EstimatePower>(
            choiceContext,
            target.Creature,
            1,
            Owner.Creature,
            this
        );
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}