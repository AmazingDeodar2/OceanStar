using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using starss.starssCode.Vfx.States;

public partial class NRatCreviceVfx : NStateFormVfx
{
    protected override string TexturePath =>
        "res://starss/vfx/states/pony/TT.png";

    public static NRatCreviceVfx? Create(Creature target)
    {
        var room = NCombatRoom.Instance;

        if (room == null)
            return null;

        var creature = room.GetCreatureNode(target);

        if (creature == null)
            return null;

        var vfx = new NRatCreviceVfx();

        vfx.InitSprite();

        vfx.Initialize(target.Player);

        // 新方式：绕过 FormHolder
        vfx.AttachTo(creature.Visuals);

        // 激活动画
        vfx.SetActive(true);

        return vfx;
    }
}