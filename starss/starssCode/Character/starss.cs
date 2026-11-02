using System.Collections.Generic;
using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using starss.starssCode.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Entities.Creatures;
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
    public override Color MapDrawingColor => new Color("FFB7C5");
    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Neutral;
    public override int StartingHp => 77;

    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<StarStrike>(),
        ModelDb.Card<StarStrike>(),
        ModelDb.Card<StarStrike>(),
        ModelDb.Card<StarStrike>(),
        ModelDb.Card<DefendStarss>(),
        ModelDb.Card<DefendStarss>(),
        ModelDb.Card<DefendStarss>(),
        ModelDb.Card<DefendStarss>(),
        ModelDb.Card<RollDice>(),
        ModelDb.Card<AreYouSure>()
        
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

    // 战斗立绘改用 spine（%Visuals = SpineSprite），需要自己声明 trigger → 动画名的映射。
    // 不能沿用基类 CharacterModel.GenerateAnimator（CharacterModel.cs:247-265）：它会硬引用
    // low_health_loop / relaxed_loop 两个动画，而我们的 skeleton 里没有。
    // 写法照官方怪物，如 TurretOperator.cs:67-83。
    public override CreatureAnimator GenerateAnimator(MegaSprite controller, Creature creature)
    {
        var idle = new AnimState("idle_loop", isLooping: true);
        var die = new AnimState("die");
        var hurt = new AnimState("hurt");
        var attack = new AnimState("attack");
        var cast = new AnimState("cast");

        // 一次性动作播完自动回 idle；die 不给 NextState（死了就该停在死亡帧）
        hurt.NextState = idle;
        attack.NextState = idle;
        cast.NextState = idle;

        var animator = new CreatureAnimator(idle, controller);
        animator.AddAnyState("Idle", idle);
        animator.AddAnyState("Dead", die);
        animator.AddAnyState("Hit", hurt);
        animator.AddAnyState("Attack", attack);
        animator.AddAnyState("Cast", cast);
        // 官方把 PowerUp 也映射到 cast（CharacterModel.cs:232），此处保持一致
        animator.AddAnyState("PowerUp", cast);
        return animator;
    }

    public override string CustomIconTexturePath => "character_icon_char_name.png".CharacterUiPath();
    public override string CustomCharacterSelectIconPath => "char_select_char_name.png".CharacterUiPath();
    public override string CustomCharacterSelectLockedIconPath => "char_select_char_name_locked.png".CharacterUiPath();
    public override string CustomMapMarkerPath => "map_marker_char_name.png".CharacterUiPath();
    public override string? CustomCharacterSelectBg =>
        "res://starss/scenes/creature_visuals/starss_select.tscn";
    //Battle sprite scene
    public override string CustomVisualPath => "res://starss/scenes/creature_visuals/starss.tscn";
    //能量条场景
    public override string CustomEnergyCounterPath => "res://starss/scenes/creature_visuals/defect_energy_counter.tscn";
}