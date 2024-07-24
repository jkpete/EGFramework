using Godot;
using System;
using EGFramework.EGTween;

namespace EGFramework.Examples.TweenAnime{
	public partial class TweenTest : Control
	{
		[Export] Control Target {set;get;}

		[Export] Node2D Target2D {set;get;}
		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			// Tween tween = GetTree().CreateTween();
			// tween.TweenProperty(Target,"color",Colors.Red,1.0f);
			// tween.TweenProperty(Target,"color",Colors.Green,1.0f);
			// tween.TweenProperty(Target,"color",Colors.Blue,1.0f);
			// tween.SetLoops();

			// Tween tween2 = GetTree().CreateTween();
			// tween2.TweenProperty(Target,"position",new Vector2(100,0),1.0f);
			// tween2.TweenProperty(Target,"position",new Vector2(100,100),1.0f);
			// tween2.TweenProperty(Target,"position",new Vector2(0,100),1.0f);
			// tween2.TweenProperty(Target,"position",new Vector2(0,0),1.0f);
			// tween2.TweenCallback(Callable.From(TweenOver));
			// tween2.SetLoops();

			// Tween tween3 = this.CreateTween();
			// tween3.SetEase(Tween.EaseType.InOut);
			// tween3.SetTrans(Tween.TransitionType.Cubic);
			// tween3.TweenInterval(0.5f);
			// tween3.TweenProperty(Target,"size",new Vector2(100,40),0.5f);
			// tween3.TweenProperty(Target,"size",new Vector2(40,100),0.5f);
			// tween3.TweenProperty(Target,"size",new Vector2(40,40),0.5f);
			// tween3.SetLoops();
			Target2D.TweenRotationByAngle(360f,1.0f);
		}

		public void TweenOver(){
			GD.Print("----end----");
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
		}
	}
}

