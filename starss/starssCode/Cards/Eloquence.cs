using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using starss.starssCode.Mechanics;
using VoidCard = MegaCrit.Sts2.Core.Models.Cards.Void;
namespace starss.starssCode.Cards;


public sealed class Eloquence : starssCard
{
    private const string IncreaseKey = "Increase";

    private decimal _extraDamageFromPlays;
    
    private decimal ExtraDamageFromPlays
    {
        get => _extraDamageFromPlays;
        set
        {
            AssertMutable();
            _extraDamageFromPlays = value;
        }
    }
    
    public Eloquence()
        : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(7M, ValueProp.Move),

        // 每次使用后增加的伤害
        new DynamicVar(IncreaseKey, 7M),

        new DoomVar(71M)
    ];
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<VoidCard>()
    ];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);


        await DamageCmd.Attack(
                DynamicVars.Damage.BaseValue
            )
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);


        // 类似 Rampage：
        // 使用后增加这张卡本场战斗中的伤害
        DynamicVars.Damage.BaseValue +=
            DynamicVars[IncreaseKey].BaseValue;


        ExtraDamageFromPlays +=
            DynamicVars[IncreaseKey].BaseValue;


        var check = await DiceHelper.Check(
            Owner.Creature,
            fate: 101,
            doom: DynamicVars["Doom"].IntValue,
            choiceContext: choiceContext,
            sourceCard: this
        );


        if (check.DoomSuccess)
        {
            CardCmd.PreviewCardPileAdd(
                await CardPileCmd.AddGeneratedCardToCombat(
                    CombatState!.CreateCard<VoidCard>(Owner),
                    PileType.Discard,
                    Owner
                )
            );

            PileType.Discard
                .GetPile(Owner)
                .InvokeCardAddFinished();
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars[IncreaseKey]
            .UpgradeValueBy(2M);
    }
    
    protected override void AfterDowngraded()
    {
        base.AfterDowngraded();

        // 恢复因为降级导致丢失的成长值
        DynamicVars.Damage.BaseValue +=
            ExtraDamageFromPlays;
    }
}