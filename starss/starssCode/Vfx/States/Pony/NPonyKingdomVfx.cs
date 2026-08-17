using Godot;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;


namespace starss.starssCode.Vfx.States;


public partial class NPonyKingdomVfx 
    : NStateFormVfx
{


    protected override string TexturePath =>
        "res://starss/vfx/states/pony/pony.png";



    public static NPonyKingdomVfx? Create(
        Creature target)
    {

        var room =
            NCombatRoom.Instance;


        if(room==null)
            return null;



        var creature =
            room.GetCreatureNode(target);


        if(creature==null)
            return null;



        var vfx =
            new NPonyKingdomVfx();



        vfx.InitSprite();


        vfx.Initialize(
            target.Player
        );


        creature.Visuals
            .AddFormVfx(vfx);



        vfx.FadeIn();


        return vfx;
    }
    
}