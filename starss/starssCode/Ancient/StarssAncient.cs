using System;
using System.Collections.Generic;
using BaseLib.Abstracts;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

namespace starss.starssCode.Ancients;

public class StarssAncient : CustomAncientModel
{
    public StarssAncient()
        : base(
            autoAdd: true,
            logDialogueLoad: true
        )
    {
        GD.Print("[starss Ancient] Constructor called");
    }

    // 显式指定测试场景
    public override string? CustomScenePath =>
        "res://starss/scenes/events/starss_ancient.tscn";

    // 三个固定奖励选项
    protected override OptionPools MakeOptionPools
    {
        get
        {
            GD.Print("[starss Ancient] Building option pools");

            return new OptionPools(
                MakePool(AncientOption<Nunchaku>()),
                MakePool(AncientOption<Lantern>()),
                MakePool(AncientOption<ArtOfWar>())
            );
        }
    }

    protected override IReadOnlyList<EventOption>
        GenerateInitialOptions()
    {
        GD.Print(
            "[starss Ancient] GenerateInitialOptions START"
        );

        try
        {
            var options = base.GenerateInitialOptions();

            GD.Print(
                $"[starss Ancient] Generated {options.Count} options"
            );

            return options;
        }
        catch (Exception ex)
        {
            GD.PushError(
                "[starss Ancient] Option generation failed: " + ex
            );

            throw;
        }
    }
    public override string? CustomRunHistoryIconPath =>
        "res://images/ui/top_panel/character_icon_ironclad_outline.png";

    public override string? CustomRunHistoryIconOutlinePath =>
        "res://images/ui/top_panel/character_icon_ironclad_outline.png";
    public override bool IsValidForAct(ActModel act)
    {
        return false;
    }
}