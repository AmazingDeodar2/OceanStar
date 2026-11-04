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

	// 新 spine（starspine2.skel，Spine 4.2.43 导出）已包含 low_health_loop / relaxed_loop，
	// 基类 CharacterModel.GenerateAnimator（CharacterModel.cs:247-265）的全套标准接线可直接复用
	// （idle/低血量双 Idle 条件、Relaxed、Dead、一次性动作自动回 idle）。
	// 这里只重写 AnimationStates 虚属性（CharacterModel.cs:227-233）追加两个 mod 自定义 trigger：
	//   PcPower → pcpower：7 张 IPcCard 牌打出时的专属动画
	//   Defend  → defend ：DefendStarss 获得格挡时的动画（官方无此 trigger，自定义触发）
	protected override List<(AnimState, string)> AnimationStates => new List<(AnimState, string)>
	{
		(new AnimState("cast"), "Cast"),
		(new AnimState("attack"), "Attack"),
		(new AnimState("hurt"), "Hit"),
		(new AnimState("cast"), "PowerUp"),
		(new AnimState("pcpower"), "PcPower"),
		(new AnimState("defend"), "Defend")
	};

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
	//商店立绘场景：root 挂官方 NMerchantCharacter（_Ready 直接按名播 relaxed_loop，NMerchantCharacter.cs:10-16），
	//MerchantAnimPath getter 由 BaseLib 补丁接管（CustomCharacterModel.cs:524-536）；不写则走占位角色默认
	public override string CustomMerchantAnimPath =>
		"res://starss/scenes/merchant/starss_merchant.tscn";
}
