using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using starss.starssCode.Mechanics;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace starss.starssCode.Cards;


public sealed class Reputation : starssCard
{
    private const string DivisorKey = "Divisor";
    
    private const string GoldKey = "Gold";

    public Reputation()
        : base(
            2,
            CardType.Attack,
            CardRarity.Rare,
            TargetType.AnyEnemy)
    {
    }
    
    

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        // 当前金币的 1/5；升级后改为 1/3。
        new DynamicVar(DivisorKey, 5M),

        new FateVar(20M),
        new DoomVar(81M),
        new DynamicVar(GoldKey, 20M)
    ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        int divisor = DynamicVars[DivisorKey].IntValue;

        // 使用向下取整，例如：
        // 99 金币时，未升级造成 19 点伤害。
        int damage = Owner.Gold / DynamicVars["Divisor"].IntValue;

        await DamageCmd
            .Attack(damage)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        DiceCheckResult check = await DiceHelper.Check(
            Owner.Creature,
            DynamicVars["Fate"].IntValue,
            DynamicVars["Doom"].IntValue,
            choiceContext,
            this
        );

        // 命运和厄运根据你的检定系统分别判断。
        if (check.FateSuccess)
        {
            await PlayerCmd.GainGold(
                DynamicVars[GoldKey].BaseValue,
                Owner
            );
        }

        if (check.DoomSuccess)
        {
            await PlayerCmd.LoseGold(
                DynamicVars[GoldKey].BaseValue,
                Owner
            );
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Divisor"].UpgradeValueBy(-2M);
    }
}