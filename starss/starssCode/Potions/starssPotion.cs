using BaseLib.Abstracts;
using BaseLib.Utils;
using starss.starssCode.Character;

namespace starss.starssCode.Potions;

[Pool(typeof(starssPotionPool))]
public abstract class starssPotion : CustomPotionModel;