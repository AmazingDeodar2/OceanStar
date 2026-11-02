using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using starss.starssCode.Cards.Interfaces;
using starss.starssCode.Mechanics;

namespace starss.starssCode.Cards;


/// <summary>
/// 完全照官方「飞镖 Flechettes」的模型：
/// DamageVar = 每一跳的伤害，CalculatedVar("CalculatedHits") = 跳数，OnPlay 里 WithHitCount。
/// 不要把「跳数 × 单跳伤害」合成一个 CalculatedDamageVar 单跳打出去——那样卡面上看不到次数，
/// 玩家也感受不到「多段」。
/// 注意：CalculatedVar 的 Calculate() 内部会取 DynamicVars.CalculationBase / CalculationExtra
/// （CalculatedVar.cs:96/104），所以这两个 Var **必须**一起声明，缺了运行时直接抛。
/// </summary>
public sealed class Please : starssCard, IDoomCard
{
    public Please()
        : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        // 每一跳的伤害
        new DamageVar(7M, ValueProp.Move),
        // CalculatedVar 的固定搭配（飞镖同款）：0 基 + 1 倍率，倍率就是命中次数
        new CalculationBaseVar(0M),
        new CalculationExtraVar(1M),
        new CalculatedVar("CalculatedHits").WithMultiplier(
            static (card, _) =>
                PileType.Hand.GetPile(card.Owner).Cards.Count(c => c is IDoomCard && c != card)
                + (card is IDoomCard ? 1 : 0)),
        new DoomVar(51M)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .WithHitCount((int)((CalculatedVar)DynamicVars["CalculatedHits"]).Calculate(cardPlay.Target))
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        var check = await DiceHelper.Check(
            Owner.Creature,
            fate: 101,
            doom: DynamicVars["Doom"].IntValue,
            choiceContext: choiceContext,
            sourceCard: this
        );

        if (!check.DoomSuccess)
            return;

        CardSelectorPrefs prefs = new CardSelectorPrefs(
            CardSelectorPrefs.ExhaustSelectionPrompt,
            1
        );

        CardModel card = (await CardSelectCmd.FromHand(
            choiceContext,
            Owner,
            prefs,
            null,
            this
        )).FirstOrDefault();

        if (card != null)
            await CardCmd.Exhaust(choiceContext, card);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3M);
    }
}
