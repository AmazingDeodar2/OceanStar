using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Vfx.Forms;


namespace starss.starssCode.Mechanics;


public abstract class StateModel
{

    public Player Owner { get; internal set; } = null!;


    protected NFormVfx? Vfx { get; private set; }


    public abstract string Id { get; }


    public virtual int Duration { get; protected set; }



    /// <summary>
    /// 创建该状态对应的视觉效果
    /// 子类需要特效时重写即可
    /// </summary>
    protected virtual NFormVfx? CreateVfx()
    {
        return null;
    }



    /// <summary>
    /// 由 StateSpace 调用
    /// 进入状态时创建视觉效果
    /// </summary>
    public void CreateVfxInstance()
    {
        if (Vfx != null)
            return;


        Vfx = CreateVfx();
    }




    /// <summary>
    /// 由 StateSpace 调用
    /// 离开状态时关闭视觉效果
    /// </summary>
    public void RemoveVfxInstance()
    {
        if (Vfx == null)
            return;


        Vfx.SetActive(false);

        Vfx = null;
    }



    public virtual Task OnEnter(
        PlayerChoiceContext choiceContext)
    {
        return Task.CompletedTask;
    }

    

    public virtual Task OnActive(
        PlayerChoiceContext choiceContext)
        => Task.CompletedTask;



    public virtual Task OnExit(
        PlayerChoiceContext choiceContext)
    {
        return Task.CompletedTask;
    }



    public virtual int ModifyDiceRoll(
        Creature creature,
        CardModel? sourceCard,
        int roll)
    {
        return roll;
    }



    public virtual Task AfterCardGeneratedForCombat(
        CardModel card,
        Player? creator)
    {
        return Task.CompletedTask;
    }



    public virtual bool ShouldClearBlock(
        Creature creature)
    {
        return true;
    }



    public virtual bool ShouldFlush(
        Player player)
    {
        return true;
    }



    public virtual bool TryModifyKeywordsInCombat(
        CardModel card,
        ISet<CardKeyword> keywords)
    {
        return false;
    }



    public abstract string DisplayName { get; }



    public bool IsExpired =>
        Duration <= 0;

}