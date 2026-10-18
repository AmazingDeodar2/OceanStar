
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace starss.starssCode.Relics;

public sealed class MetamorphosisPact : RelicModel
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool HasUponPickupEffect => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[]
        {
            new MaxHpVar(16m)
        };

    // 获得时失去16点最大生命值
    public override async Task AfterObtained()
    {
        await CreatureCmd.LoseMaxHp(
            new ThrowingPlayerChoiceContext(),
            Owner.Creature,
            DynamicVars.MaxHp.BaseValue,
            false
        );
    }

    // 战斗胜利后触发
    public override async Task AfterCombatVictory(CombatRoom room)
    {
        // 只在精英战斗胜利后触发
        if (room.RoomType != RoomType.Elite)
            return;

        // 没有可选卡牌时不触发
        if (PileType.Deck.GetPile(Owner).Cards.Count == 0)
            return;

        var selectedCards = await CardSelectCmd.FromDeckForRemoval(
            Owner,
            new CardSelectorPrefs(
                SelectionScreenPrompt,
                1
            )
            {
                Cancelable = false,
                RequireManualConfirmation = true
            }
        );

        CardModel? selectedCard = selectedCards.FirstOrDefault();

        if (selectedCard == null)
            return;

        // 将选择的卡牌变化为随机卡牌
        await CardCmd.Transform(
            new List<CardTransformation>
            {
                new CardTransformation(selectedCard)
            },
            Owner.PlayerRng.Transformations
        );

        Flash();
    }
}
