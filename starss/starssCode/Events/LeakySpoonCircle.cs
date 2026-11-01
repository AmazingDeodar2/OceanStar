
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.HoverTips;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Models;

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
        var options = new List<EventOption>(3);

        foreach (CharacterOption character in SelectedOptions)
        {
            options.Add(CreateOption(character));
        }

        return options;
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
    private Task WoodenHorse()
    {
        return FinishOption("WOODEN_HORSE");
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
    private Task TT()
    {
        return FinishOption("TT");
    }

    // 雪松：七张PC牌中选择两张
    private Task Xuesong()
    {
        return FinishOption("XUESONG");
    }

    // 娜娜：获得5点最大生命值
    private Task Nana()
    {
        return FinishOption("NANA");
    }

    // 鹅鹅：增加药水栏位并获得混沌药水
    private Task Goose()
    {
        return FinishOption("GOOSE");
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
