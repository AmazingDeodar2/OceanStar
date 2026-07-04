using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using starss.starssCode.Powers;

namespace starss.starssCode.Cards;


public sealed class CloverLeaf : starssCard
{
    public CloverLeaf()
        : base(0, CardType.Skill, CardRarity.Token, TargetType.Self)
    {
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(1),
        new PowerVar<LuckyPower>("Power", 2M)
    ];
    
    public static IEnumerable<CloverLeaf> Create(Player owner, int amount, ICombatState combatState)
    {
        List<CloverLeaf> cloverLeafList = new List<CloverLeaf>();

        for (int index = 0; index < amount; ++index)
            cloverLeafList.Add(combatState.CreateCard<CloverLeaf>(owner));

        return cloverLeafList;
    }
    
    public static async Task<IEnumerable<CloverLeaf>> CreateInHand(
        Player owner,
        int amount,
        ICombatState combatState)
    {
        IEnumerable<CloverLeaf> CloverLeafs = CloverLeaf.Create(owner, amount, combatState);
        IReadOnlyList<CardPileAddResult> combat = await CardPileCmd.AddGeneratedCardsToCombat((IEnumerable<CardModel>) CloverLeafs, PileType.Hand, owner);
        IEnumerable<CloverLeaf> inHand = CloverLeafs;
        CloverLeafs = (IEnumerable<CloverLeaf>) null;
        return inHand;
    }
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(
            choiceContext,
            DynamicVars.Cards.BaseValue,
            Owner
        );

        await PowerCmd.Apply<LuckyPower>(
            choiceContext,
            Owner.Creature,
            DynamicVars["Power"].BaseValue,
            Owner.Creature,
            this
        );
        
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Power"].UpgradeValueBy(1M);
    }
    
    
}