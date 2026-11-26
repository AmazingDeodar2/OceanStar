
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.HoverTips;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Potions;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;
using starss.starssCode.Cards;

namespace starss.starssCode.Events;

/// <summary>
/// 漏勺是一个圈
///
/// 七位候选人物：
/// 1. 木马
/// 2. 3w
/// 3. 淇淇
/// 4. TT
/// 5. 雪松
/// 6. 娜娜
/// 7. 鹅鹅
///
/// 每次进入事件，随机显示三个不同选项。
/// </summary>
public sealed class LeakySpoonCircle : EventModel
{
    private enum CharacterOption
    {
        WoodenHorse = 1,
        ThreeW = 2,
        Qiqi = 3,
        TT = 4,
        Xuesong = 5,
        Nana = 6,
        Goose = 7
    }

    private IReadOnlyList<CharacterOption>? _selectedOptions;

    
    private IReadOnlyList<CharacterOption> SelectedOptions
    {
        get
        {
            AssertMutable();

            if (_selectedOptions == null)
            {
                _selectedOptions = Enumerable
                    .Range(1, 7)
                    .ToList()
                    .StableShuffle(Rng)
                    .Take(3)
                    .Select(id => (CharacterOption)id)
                    .ToList();
            }

            return _selectedOptions;
        }
    }


    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        MainFile.Logger.Info(
            "[starss][event] 开始生成漏勺事件选项"
        );

        try
        {
            var options = new List<EventOption>(3);

            foreach (CharacterOption character in SelectedOptions)
            {
                MainFile.Logger.Info(
                    $"[starss][event] 正在创建选项：{character}"
                );

                options.Add(CreateOption(character));

                MainFile.Logger.Info(
                    $"[starss][event] 选项创建成功：{character}"
                );
            }

            MainFile.Logger.Info(
                $"[starss][event] 选项生成完成，共 {options.Count} 个"
            );

            return options;
        }
        catch (Exception ex)
        {
            MainFile.Logger.Error(
                $"[starss][event] 选项生成失败：{ex}"
            );

            throw;
        }
    }

    private EventOption CreateOption(CharacterOption character)
    {
        switch (character)
        {
            case CharacterOption.WoodenHorse:

                // 木马：至少拥有100金币
                if (Owner.Gold < 100)
                {
                    return new EventOption(
                        this,
                        null,
                        "LEAKY_SPOON_CIRCLE.pages.INITIAL.options.WOODEN_HORSE_LOCKED",
                        Array.Empty<IHoverTip>()
                    );
                }

                return new EventOption(
                    this,
                    WoodenHorse,
                    "LEAKY_SPOON_CIRCLE.pages.INITIAL.options.WOODEN_HORSE",
                    Array.Empty<IHoverTip>()
                );

            case CharacterOption.ThreeW:
                return new EventOption(
                    this,
                    ThreeW,
                    "LEAKY_SPOON_CIRCLE.pages.INITIAL.options.THREE_W",
                    Array.Empty<IHoverTip>()
                );

            case CharacterOption.Qiqi:
                return new EventOption(
                    this,
                    Qiqi,
                    "LEAKY_SPOON_CIRCLE.pages.INITIAL.options.QIQI",
                    Array.Empty<IHoverTip>()
                );

            case CharacterOption.TT:
                return new EventOption(
                    this,
                    TT,
                    "LEAKY_SPOON_CIRCLE.pages.INITIAL.options.TT",
                    Array.Empty<IHoverTip>()
                );

            case CharacterOption.Xuesong:
                return new EventOption(
                    this,
                    Xuesong,
                    "LEAKY_SPOON_CIRCLE.pages.INITIAL.options.XUESONG",
                    Array.Empty<IHoverTip>()
                );

            case CharacterOption.Nana:
                return new EventOption(
                    this,
                    Nana,
                    "LEAKY_SPOON_CIRCLE.pages.INITIAL.options.NANA",
                    Array.Empty<IHoverTip>()
                );

            case CharacterOption.Goose:
                return new EventOption(
                    this,
                    Goose,
                    "LEAKY_SPOON_CIRCLE.pages.INITIAL.options.GOOSE",
                    Array.Empty<IHoverTip>()
                );

            default:
                throw new ArgumentOutOfRangeException(
                    nameof(character),
                    character,
                    null
                );
        }
    }

    // =========================================================
    // 七个事件选项
    // 当前仅显示结果页面，暂不执行奖励
    // =========================================================

    // 木马：支付100金币，获得300金币
    private async Task WoodenHorse()
    {
        if (Owner.Gold < 100)
            return;

        await PlayerCmd.LoseGold(
            100,
            Owner,
            GoldLossType.Spent
        );

        await PlayerCmd.GainGold(
            300,
            Owner
        );

        await FinishOption("WOODEN_HORSE");
    }

    // 3w：进入精英战斗，额外获得一个遗物
    private Task ThreeW()
    {
        return FinishOption("THREE_W");
    }

    // 淇淇：获得随机涅奥遗物
    private Task Qiqi()
    {
        return FinishOption("QIQI");
    }

    // TT：获得三组其他角色卡牌奖励
    // TT：获得三组其他角色的卡牌奖励。
    private async Task TT()
    {
        // 获取所有角色专属卡池，排除当前角色的卡池。
        var otherCardPools = ModelDb.AllCharacterCardPools
            .Where(pool => pool.Id != Owner.Character.CardPool.Id)
            .Distinct()
            .ToList();

        // 防止候选卡池为空。
        if (otherCardPools.Count == 0)
        {
            await FinishOption("TT");
            return;
        }

        // 创建三组卡牌奖励。
        var rewards = new List<Reward>(3);

        for (int i = 0; i < 3; i++)
        {
            // 使用普通战斗的卡牌稀有度概率。
            var options = CardCreationOptions
                .ForNonCombatWithDefaultOdds(otherCardPools)
                .WithRngOverride(Rng);

            // 每组奖励包含三张候选卡牌。
            rewards.Add(
                new CardReward(
                    options,
                    3,
                    Owner
                )
            );
        }

        // 显示原版自定义奖励界面。
        await RewardsCmd.OfferCustom(
            Owner,
            rewards
        );

        // 奖励处理完成后，显示事件结局。
        await FinishOption("TT");
    }

    // 雪松：七张PC牌中选择两张
    // 雪松：七张PC牌中选择两张加入牌组。
    private async Task Xuesong()
    {
        var cards = new List<CardCreationResult>
        {
            CreatePCCard<TT>(),
            CreatePCCard<Nana>(),
            CreatePCCard<TrojanHorse>(),
            CreatePCCard<SnowCedar>(),
            CreatePCCard<Gratitude>(),
            CreatePCCard<ThreeW>(),
            CreatePCCard<Qiqi>()
        };

        var prefs = new CardSelectorPrefs(
            L10NLookup(
                "LEAKY_SPOON_CIRCLE.pages.XUESONG.selectionScreenPrompt"
            ),
            2
        );

        await SelectCardsToAddToDeckFromGrid(cards, prefs);

        await FinishOption("XUESONG");
    }

// 创建指定PC牌的可变实例，并包装为卡牌选择结果。
    private CardCreationResult CreatePCCard<T>()
        where T : CardModel
    {
        return new CardCreationResult(
            Owner.RunState.CreateCard<T>(Owner)
        );
    }

    // 娜娜：获得5点最大生命值
    private Task Nana()
    {
        return FinishOption("NANA");
    }

    // 鹅鹅：增加药水栏位并获得混沌药水
    private async Task Goose()
    {
        // 增加一个永久药水栏位。
        await PlayerCmd.GainMaxPotionCount(1, Owner);

        // 创建混沌药水。
        var potion = ModelDb.Potion<DistilledChaos>()
            .ToMutable();

        // 将药水放入玩家的药水栏。
        await PotionCmd.TryToProcure(potion, Owner);

        await FinishOption("GOOSE");
    }

    // =========================================================
    // 结束事件
    // =========================================================

    private Task FinishOption(string optionKey)
    {
        SetEventFinished(
            L10NLookup(
                $"LEAKY_SPOON_CIRCLE.pages.{optionKey}.description"
            )
        );

        return Task.CompletedTask;
    }
}
