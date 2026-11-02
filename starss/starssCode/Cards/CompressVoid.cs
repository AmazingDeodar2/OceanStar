using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using VoidCard = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace starss.starssCode.Cards;


public sealed class CompressVoid : starssCard
{
    public CompressVoid()
        : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<ArtifactPower>(),
        HoverTipFactory.FromPower<IntangiblePower>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        // 消耗牌堆里的虚空不少于这个数量时，额外获得无实体。
        new DynamicVar("VoidThreshold", 5M)
    ];

    /// <summary>
    /// 条件满足时给卡牌加金色描边光（和官方「拆卸 Dismantle」同款）。
    /// 钩子是 <c>CardModel.ShouldGlowGoldInternal</c>（CardModel.cs:859），由 UI 轮询。
    /// 判定复用 <see cref="HasEnoughVoid"/>，保证「发光条件」和「实际效果」永远是同一条规则。
    /// 要判空：卡面在牌库/非战斗界面展示时 <c>Owner</c> 可能是 null（官方也是这么防的）。
    /// </summary>
    protected override bool ShouldGlowGoldInternal =>
        CombatState != null && Owner != null && HasEnoughVoid;

    /// <summary>消耗牌堆里的虚空张数。</summary>
    private int VoidCount =>
        PileType.Exhaust
            .GetPile(Owner)
            .Cards
            .Count(IsVoid);

    /// <summary>是否满足「额外获得无实体」的条件。</summary>
    private bool HasEnoughVoid =>
        VoidCount >= DynamicVars["VoidThreshold"].IntValue;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 人工制品总是给。
        await PowerCmd.Apply<ArtifactPower>(
            choiceContext,
            Owner.Creature,
            1,
            Owner.Creature,
            this);

        // 虚空够多再追加无实体（和发光用的是同一个判定）。
        if (HasEnoughVoid)
        {
            await PowerCmd.Apply<IntangiblePower>(
                choiceContext,
                Owner.Creature,
                1,
                Owner.Creature,
                this);
        }
    }

    private static bool IsVoid(CardModel card)
    {
        return card is VoidCard;
    }

    protected override void OnUpgrade()
    {
        // 判定阈值 5→3。
        DynamicVars["VoidThreshold"].UpgradeValueBy(-2M);
    }
}
