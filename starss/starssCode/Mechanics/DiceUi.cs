using Godot;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using System.Linq;
using System.Threading.Tasks;

namespace starss.starssCode.Mechanics;

public static class DiceUi
{
    public static async Task ShowRoll(DiceRollResult result)
    {
        var room = NCombatRoom.Instance;
        if (room == null)
            return;

        var rollText = result.Rolls.Count > 1
            ? $"🎲 {string.Join(" / ", result.Rolls)} → {result.Value}"
            : $"🎲 {result.Value}";

        var label = new Label
        {
            Text = rollText,
            Position = new Vector2(780, 120),
            Scale = new Vector2(3.6f, 3.6f),
            ZIndex = 999
        };

        room.AddChild(label);

        await Task.Delay(1000);

        if (GodotObject.IsInstanceValid(label))
            label.QueueFree();
    }
}