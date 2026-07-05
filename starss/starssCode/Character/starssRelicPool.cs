using BaseLib.Abstracts;
using starss.starssCode.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using starss.starssCode.Relics;

namespace starss.starssCode.Character;

public class starssRelicPool : CustomRelicPoolModel
{
    public override Color LabOutlineColor => Starss.Color;

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
    protected override IEnumerable<RelicModel> GenerateAllRelics()
    {
        yield return ModelDb.Relic<EyebrowPencil>();
        yield return ModelDb.Relic<FourLeafClover>();
        yield return ModelDb.Relic<FoxMask>();
        yield return ModelDb.Relic<Pineapple>();
        yield return ModelDb.Relic<PineappleModel>();
        yield return ModelDb.Relic<SevenSevenSeven>();
        yield return ModelDb.Relic<RabbitFoot>();

        // 后面你的可奖励遗物都手动加：
        // yield return ModelDb.Relic<AnotherRelic>();
    }
}