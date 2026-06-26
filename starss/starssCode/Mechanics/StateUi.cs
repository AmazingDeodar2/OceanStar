using Godot;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using System.Linq;

namespace starss.starssCode.Mechanics;

public static class StateUi
{
    private static Label? _label;

    public static void Refresh(StateSpace stateSpace)
    {
        var room = NCombatRoom.Instance;
        if (room == null)
            return;

        if (_label == null || !GodotObject.IsInstanceValid(_label))
        {
            _label = new Label
            {
                Position = new Vector2(780, 180),
                Scale = new Vector2(2.5f, 2.5f),
                ZIndex = 999
            };

            room.AddChild(_label);
        }

        if (stateSpace.States.Count == 0)
        {
            _label.Visible = false;
            _label.Text = "";
            return;
        }

        _label.Text = string.Join(
            "\n",
            stateSpace.States.Select(state => $"【{state.DisplayName}】")
        );

        _label.Visible = true;
    }

    public static void Clear()
    {
        if (_label != null && GodotObject.IsInstanceValid(_label))
        {
            _label.Visible = false;
            _label.QueueFree();
        }

        _label = null;
    }
}