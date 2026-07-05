using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using starss.starssCode.Mechanics;
using starss.starssCode.Powers;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Models.Cards;

namespace starss.starssCode.Cards;


public sealed class OneMoreTime : starssCard
{
    public OneMoreTime()
        : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Beckon>()
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Cards", 1M),
        new DoomVar(80M)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<OneMoreTimePower>(
            choiceContext,
            Owner.Creature,
            DynamicVars["Cards"].BaseValue,
            Owner.Creature,
            this
        );

        var check = await DiceHelper.Check(
            Owner.Creature,
            fate: 0,
            doom: DynamicVars["Doom"].IntValue
        );

        if (check.DoomSuccess)
        {
            CardModel callCard = Owner.Creature.CombatState.CreateCard<Beckon>(Owner);
            await CardPileCmd.AddGeneratedCardToCombat(
                callCard,
                PileType.Discard,
                Owner
                );

            PileType.Discard.GetPile(Owner).InvokeCardAddFinished();
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Cards"].UpgradeValueBy(1M);
    }
}