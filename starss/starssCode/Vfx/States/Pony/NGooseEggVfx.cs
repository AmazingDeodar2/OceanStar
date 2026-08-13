using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using starss.starssCode.Vfx.States;

public partial class NGooseEggVfx 
    : NStateFormVfx
{
    protected override string TexturePath =>
        "res://starss/vfx/states/pony/gratitude.png";
    
    public static NGooseEggVfx? Create(
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
            new NGooseEggVfx();



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