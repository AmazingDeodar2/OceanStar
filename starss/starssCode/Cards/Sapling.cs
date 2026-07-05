using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using starss.starssCode.Powers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace starss.starssCode.Cards;


public sealed class Sapling : starssCard
{
    public Sapling()
        : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<CloverLeaf>(IsUpgraded)
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState is null)
            return;

        int amount = DynamicVars.Cards.IntValue;

        var power = Owner.Creature.GetPower<SaplingPower>();
        if (power != null)
            amount *= power.Amount;

        List<CloverLeaf> clovers = [];

        for (int i = 0; i < amount; i++)
        {
            CloverLeaf clover = CombatState.CreateCard<CloverLeaf>(Owner);

            if (IsUpgraded)
                CardCmd.Upgrade(clover);

            clovers.Add(clover);
        }

        await CardPileCmd.AddGeneratedCardsToCombat(
            clovers,
            PileType.Draw,
            Owner
        );
        PileType.Draw.GetPile(Owner).InvokeCardAddFinished();
        int current = Owner.Creature.GetPowerAmount<SaplingPower>();
        int increase = current <= 0 ? 2 : current;

        await PowerCmd.Apply<SaplingPower>(
            choiceContext,
            Owner.Creature,
            increase,
            Owner.Creature,
            this
        );
    }

    protected override void OnUpgrade()
    {
    }
}