using System;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;
using starss.starssCode.Powers;

namespace starss.starssCode.Mechanics;

/// <summary>
/// 命运效果的统一执行入口。
///
/// 背景：以前每张命运牌自己写「困难成功」（<c>HardSuccess</c> = 投点 ≤ 命运值/2，
/// 见 <c>DiceHelper.cs:61-63</c>）的额外奖励，逻辑分散且强度不可控。
/// 现在困难成功的额外收益统一由能力牌「坦途」（<see cref="SmoothPathPower"/>）提供：
///
/// * 未触发命运 → 执行 0 次
/// * 普通成功 → 执行 1 次
/// * 困难成功 → 执行 1 + 坦途层数 次
///
/// 所有命运牌用 <see cref="Run"/> 包住自己的命运效果即可，不要再写 <c>if (check.HardSuccess)</c>。
/// </summary>
public static class FateEffect
{
    /// <summary>命运效果应执行的次数。</summary>
    public static int GetRepeatCount(in DiceCheckResult check, Creature owner)
    {
        if (!check.FateSuccess)
            return 0;

        if (!check.HardSuccess)
            return 1;

        // 困难成功：基础 1 次 + 坦途层数（没有该能力时 GetPowerAmount 返回 0）
        return 1 + owner.GetPowerAmount<SmoothPathPower>();
    }

    /// <summary>
    /// 按次数执行命运效果。<paramref name="fateEffect"/> 里不要再判 FateSuccess / HardSuccess。
    /// </summary>
    public static async Task Run(DiceCheckResult check, Creature owner, Func<Task> fateEffect)
    {
        int count = GetRepeatCount(check, owner);
        for (int i = 0; i < count; i++)
        {
            await fateEffect();
        }
    }

    /// <summary>
    /// 便捷重载：只有一个「同步」动作时（如一次性施加能力、移动牌），
    /// 内部包成 Task，省去每张卡写 async lambda。
    /// </summary>
    public static Task Run(DiceCheckResult check, Creature owner, Action fateEffect)
    {
        return Run(check, owner, () =>
        {
            fateEffect();
            return Task.CompletedTask;
        });
    }
}
