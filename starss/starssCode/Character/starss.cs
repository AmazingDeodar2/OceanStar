using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using starss.starssCode.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;
using starss.starssCode.Relics;
using starss.starssCode.Cards;
namespace starss.starssCode.Character;

public class Starss : PlaceholderCharacterModel
{
    public const string CharacterId = "starss";

    public static readonly Color Color = new("FFB7C5");

    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Neutral;
    public override int StartingHp => 77;

    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<StarStrike>(),
        ModelDb.Card<StarStrike>(),
        ModelDb.Card<StarStrike>(),
        ModelDb.Card<StarStrike>(),
        ModelDb.Card<StarCurse>(),
        ModelDb.Card<DefendIronclad>(),
        ModelDb.Card<DefendIronclad>(),
        ModelDb.Card<DefendIronclad>(),
        ModelDb.Card<DefendIronclad>(),
        ModelDb.Card<DefendIronclad>()
    ];

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<StarssStarterRelic>()
    ];

    public override CardPoolModel CardPool => ModelDb.CardPool<starssCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<starssRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<starssPotionPool>();

    /*  PlaceholderCharacterModel will utilize placeholder basegame assets for most of your character assets until you
        override all the other methods that define those assets.
        These are just some of the simplest assets, given some placeholders to differentiate your character with.
        You don't have to, but you're suggested to rename these images. */
    public override Control CustomIcon
    {
        get
        {
            var icon = NodeFactory<Control>.CreateFromResource(CustomIconTexturePath);
            icon.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
            return icon;
        }
    }

    public override string CustomIconTexturePath => "character_icon_char_name.png".CharacterUiPath();
    public override string CustomCharacterSelectIconPath => "char_select_char_name.png".CharacterUiPath();
    public override string CustomCharacterSelectLockedIconPath => "char_select_char_name_locked.png".CharacterUiPath();
    public override string CustomMapMarkerPath => "map_marker_char_name.png".CharacterUiPath();
    public override string? CustomCharacterSelectBg =>
        "res://starss/scenes/creature_visuals/starss.tscn";
}