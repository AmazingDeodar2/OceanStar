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
        new Vector2(0,-300);


    protected virtual Vector2 SmallScale =>
        new Vector2(0.39f, 0.39f);


    protected virtual Vector2 BigScale =>
        new Vector2(0.41f, 0.41f);



    protected virtual float Alpha =>
        0.60f;



    protected void InitSprite()
    {
        Sprite = new Sprite2D();


        Sprite.Texture =
            GD.Load<Texture2D>(
                TexturePath
            );


        Sprite.Position =
            BasePosition;


        Sprite.Scale =
            SmallScale;


        Sprite.Modulate =
            new Color(
                1,
                1,
                1,
                0
            );


        AddChild(Sprite);
    }




    protected void FadeIn()
    {
        if(Sprite == null)
            return;


        FadeTween?.Kill();


        FadeTween =
            CreateTween();


        FadeTween.TweenProperty(
            Sprite,
            "modulate:a",
            Alpha,
            1.5f
        );


        FadeTween.TweenCallback(
            Callable.From(
                StartIdle
            )
        );
    }





    private void StartIdle()
    {
        if(Sprite == null)
            return;


        IdleTween?.Kill();


        IdleTween =
            CreateTween()
            .SetLoops();



        IdleTween.TweenProperty(
            Sprite,
            "scale",
            BigScale,
            1f
        );


        IdleTween.TweenProperty(
            Sprite,
            "scale",
            SmallScale,
            1f
        );
    }




    private void StartFloat()
    {
        if(Sprite == null)
            return;


        FloatTween =
            CreateTween()
            .SetLoops();


        FloatTween.TweenProperty(
            Sprite,
            "position:y",
            BasePosition.Y-10,
            1.5f
        );


        FloatTween.TweenProperty(
            Sprite,
            "position:y",
            BasePosition.Y+10,
            1.5f
        );
    }




    public override void SetActive(
        bool active)
    {
        base.SetActive(active);


        if(Sprite==null)
            return;


        if(active)
        {
            Visible=true;

            FadeIn();

            StartFloat();
        }
        else
        {
            IdleTween?.Kill();

            FloatTween?.Kill();


            FadeTween =
                CreateTween();


            FadeTween.TweenProperty(
                Sprite,
                "modulate:a",
                0,
                1.5f
            );


            FadeTween.TweenCallback(
                Callable.From(
                    ()=>Visible=false
                )
            );
        }
    }
}