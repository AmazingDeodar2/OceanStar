using System.Collections.Generic;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;
using starss.starssCode.Powers;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using VoidCard = MegaCrit.Sts2.Core.Models.Cards.Void;
namespace starss.starssCode.Cards;

public sealed class Abyss : starssCard
{
	public Abyss()
		: base(3, CardType.Power, CardRarity.Rare, TargetType.Self)
	{
	}
	protected override IEnumerable<IHoverTip> ExtraHoverTips =>
	[
		EnergyHoverTip,
		HoverTipFactory.FromCard<VoidCard>()
	];
	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await CreatureCmd.TriggerAnim(
			Owner.Creature,
			"PowerUp",
			Owner.Character.PowerUpAnimDelay
		);

		for (int i = 0; i < 3; i++)
		{
			
			CardModel voidCard = Owner.Creature.CombatState.CreateCard<VoidCard>(Owner);
			voidCard.Owner = Owner;

			CardCmd.PreviewCardPileAdd(
				await CardPileCmd.AddGeneratedCardToCombat(
					CombatState!.CreateCard<VoidCard>(Owner),
					PileType.Discard,
					Owner
				)
			);
		}
		PileType.Discard.GetPile(Owner).InvokeCardAddFinished();
		await PowerCmd.Apply<AbyssPower>(
			choiceContext,
			Owner.Creature,
			4M,
			Owner.Creature,
			this
		);
	}

	protected override void OnUpgrade()
	{
		EnergyCost.UpgradeBy(-1);
	}
}
