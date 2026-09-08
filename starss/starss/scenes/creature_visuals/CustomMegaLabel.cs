using Godot;
using System;
using MegaCrit.Sts2.addons.mega_text;

/// <summary>
/// starss 专用 MegaLabel。
///
/// MegaLabel 默认 AutoSizeEnabled=true、范围 Min=8~Max=100，若场景没写
/// MinFontSize/MaxFontSize（Godot 会序列化成 null），编辑器里因星区工程没有游戏主题
/// 字体项而 no-op、显示场景的 36；运行时游戏主题生效后 SetTextAutoSize 会把字号
/// 自动放大到最大 100，导致能量数字 Label 异常变大。
///
/// 这里参照游戏原版能量计数器场景（res://scenes/combat/energy_counters/defect_energy_counter.tscn
/// 设 MinFontSize=32 / MaxFontSize=36）在基类 _Ready 缩放前锁定范围，保证编辑器与运行时一致。
/// </summary>
public partial class CustomMegaLabel : MegaLabel
{
    public override void _Ready()
    {
        AutoSizeEnabled = true;
        MinFontSize = 32;
        MaxFontSize = 36;
        base._Ready();
    }
}
