using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using starss.starssCode.Cards;

namespace starss.starssCode.Powers;

public sealed class GoodLuckTemporaryStrengthPower : TemporaryStrengthPower
{
    public override AbstractModel OriginModel => ModelDb.Card<GoodLuck>();
}

public sealed class GoodLuckTemporaryDexterityPower : TemporaryDexterityPower
{
    public override AbstractModel OriginModel => ModelDb.Card<GoodLuck>();
}