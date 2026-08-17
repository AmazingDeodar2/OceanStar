using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Vfx.Forms;
using starss.starssCode.Cards;
using starss.starssCode.Mechanics;
using starss.starssCode.Vfx.States;


namespace starss.starssCode.States;


public class PonyLandState : StateModel
{

    public override string Id => "starss:pony_land";


    private static readonly Dictionary<Player, int> NextGoldByPlayer = new();



    public PonyLandState()
    {
        Duration = int.MaxValue;
    }



    protected override NFormVfx? CreateVfx()
    {
        return NPonyKingdomVfx.Create(
            Owner.Creature
        );
    }



    public override async Task OnEnter(
        PlayerChoiceContext choiceContext)
    {
        var gold = GetNextGoldAmount(Owner);


        if (gold > 0)
        {
            await PlayerCmd.GainGold(
                gold,
                Owner
            );
        }
    }



    public override Task OnExit(
        PlayerChoiceContext choiceContext)
    {
        return Task.CompletedTask;
    }



    public override int ModifyDiceRoll(
        Creature creature,
        CardModel? sourceCard,
        int roll)
    {
        if (sourceCard is DesertEagle)
            return 96 + (roll - 1) * 4 / 99;


        return roll;
    }



    private static int GetNextGoldAmount(
        Player player)
    {
        if (!NextGoldByPlayer.TryGetValue(player, out var gold))
            gold = 7;


        NextGoldByPlayer[player] =
            Math.Max(0, gold - 1);


        return gold;
    }



    public static void ClearGoldCounters()
    {
        NextGoldByPlayer.Clear();
    }



    public override string DisplayName =>
        "小马国度";

}