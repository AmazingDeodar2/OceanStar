
using Godot;
using MegaCrit.Sts2.Core.Nodes.Vfx.Forms;

namespace starss.starssCode.Vfx.States;

public abstract partial class NStateFormVfx : NFormVfx
{
    protected Sprite2D? Sprite;

    protected Tween? FadeTween;
    protected Tween? IdleTween;
    protected Tween? FloatTween;

    protected abstract string TexturePath { get; }

    protected virtual Vector2 BasePosition =>
        new Vector2(0, -300);

    protected virtual Vector2 SmallScale =>
        new Vector2(0.39f, 0.39f);

    protected virtual Vector2 BigScale =>
        new Vector2(0.41f, 0.41f);

    protected virtual float Alpha => 0.60f;

    // 不依赖游戏原生 FormHolder 的挂载方式
    public void AttachTo(Node parent)
    {
        parent.AddChild(this);
        ZIndex = 0;

        // 将状态动画移动到子节点列表的最前面
        parent.MoveChild(this, 0);
    }

    protected void InitSprite()
    {
        Sprite = new Sprite2D
        {
            Texture = GD.Load<Texture2D>(TexturePath),
            Position = BasePosition,
            Scale = SmallScale,
            Modulate = new Color(1, 1, 1, 0)
        };

        AddChild(Sprite);
    }

    protected void FadeIn()
    {
        if (Sprite == null || !IsInsideTree())
            return;

        FadeTween?.Kill();

        Visible = true;

        FadeTween = CreateTween();

        FadeTween.TweenProperty(
            Sprite,
            "modulate:a",
            Alpha,
            1.5f
        );

        FadeTween.TweenCallback(
            Callable.From(StartIdle)
        );
    }

    private void StartIdle()
    {
        if (Sprite == null || !IsInsideTree())
            return;

        IdleTween?.Kill();

        IdleTween = CreateTween().SetLoops();

        IdleTween.TweenProperty(
            Sprite, "scale", BigScale, 1f
        );

        IdleTween.TweenProperty(
            Sprite, "scale", SmallScale, 1f
        );
    }

    private void StartFloat()
    {
        if (Sprite == null || !IsInsideTree())
            return;

        FloatTween?.Kill();

        FloatTween = CreateTween().SetLoops();

        FloatTween.TweenProperty(
            Sprite,
            "position:y",
            BasePosition.Y - 10,
            1.5f
        );

        FloatTween.TweenProperty(
            Sprite,
            "position:y",
            BasePosition.Y + 10,
            1.5f
        );
    }

    public override void SetActive(bool active)
    {
        // 独立挂载时不调用 base.SetActive，
        // 避免依赖原生 Form VFX 生命周期。
        if (Sprite == null || !IsInsideTree())
            return;

        FadeTween?.Kill();
        IdleTween?.Kill();
        FloatTween?.Kill();

        if (active)
        {
            Visible = true;

            FadeIn();
            StartFloat();
        }
        else
        {
            FadeTween = CreateTween();

            FadeTween.TweenProperty(
                Sprite,
                "modulate:a",
                0f,
                1.5f
            );

            FadeTween.TweenCallback(
                Callable.From(QueueFree)
            );
        }
    }
}
