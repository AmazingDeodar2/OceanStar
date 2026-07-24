using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace starss.starssCode.Cards;


public sealed class ChooseGod : starssCard
{
    public ChooseGod()
        : base(
            1,
            CardType.Skill,
            CardRarity.Uncommon,
            TargetType.AnyAlly)
    {
    }
    public override CardMultiplayerConstraint MultiplayerConstraint
    {
        get => CardMultiplayerConstraint.MultiplayerOnly;
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        // 升级后：给予其他所有玩家一张神之骰。
        if (IsUpgraded)
        {
            foreach (var teammateCreature in CombatState.GetTeammatesOf(Owner.Creature))
            {
                Player teammate = teammateCreature.Player;

                if (teammate == null || teammate == Owner)
                    continue;

                CardModel godDice = CombatState.CreateCard<GodDice>(teammate);

                await CardPileCmd.AddGeneratedCardToCombat(
                    godDice,
                    PileType.Hand,
                    teammate
                );
            }

            return;
        }

        // 未升级：获取玩家选择的队友。
        Player selectedPlayer = cardPlay.Target.Player;

        // 防止选择自己或目标异常。
        if (selectedPlayer == null || selectedPlayer == Owner)
            return;

        CardModel selectedGodDice =
            CombatState.CreateCard<GodDice>(selectedPlayer);

        await CardPileCmd.AddGeneratedCardToCombat(
            selectedGodDice,
            PileType.Hand,
            selectedPlayer
        );
    }

    protected override void OnUpgrade()
    {
    }
}