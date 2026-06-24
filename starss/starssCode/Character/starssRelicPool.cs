using BaseLib.Abstracts;
using starss.starssCode.Extensions;
using Godot;

namespace starss.starssCode.Character;

public class starssRelicPool : CustomRelicPoolModel
{
    public override Color LabOutlineColor => Starss.Color;

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}