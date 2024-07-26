using Godot;
using System;

namespace EGFramework.EGTween{
    public static class EGTweenExtension
    {
		#region Function
			public static Tween KillOnEnd(this Tween self){
				self.TweenCallback(Callable.From(self.Kill));
				return self;
			}
		#endregion
		#region Position
		public static Tween TweenPosition(this Control self,Vector2 position,float delay){
			Tween tween = self.CreateTween();
			tween.TweenProperty(self,"position",position,delay);
			return tween;
		}

		public static Tween TweenPosition(this Node2D self,Vector2 position,float delay){
			Tween tween = self.CreateTween();
			tween.TweenProperty(self,"position",position,delay);
			return tween;
		}

		public static Tween TweenPosition(this Node3D self,Vector3 position,float delay){
			Tween tween = self.CreateTween();
			tween.TweenProperty(self,"position",position,delay);
			return tween;
		}
		
		#endregion
		
		#region Rotation
		public static Tween TweenRotationByRad(this Control self,float rad,float delay){
			Tween tween = self.CreateTween();
			tween.TweenProperty(self,"rotation",rad,delay);
			return tween;
		}
		public static Tween TweenRotationByAngle(this Control self,float angle,float delay){
			Tween tween = self.CreateTween();
			tween.TweenProperty(self,"rotation",angle*Mathf.Pi/180,delay);
			return tween;
		}
		public static Tween TweenRotationByRad(this Node2D self,float rad,float delay){
			Tween tween = self.CreateTween();
			tween.TweenProperty(self,"rotation",rad,delay);
			return tween;
		}
		public static Tween TweenRotationByAngle(this Node2D self,float angle,float delay){
			Tween tween = self.CreateTween();
			tween.TweenProperty(self,"rotation",angle*Mathf.Pi/180,delay);
			return tween;
		}
		
		#endregion

		#region Scale
		public static Tween TweenScale(this Control self,Vector2 scale,float delay){
			Tween tween = self.CreateTween();
			tween.TweenProperty(self,"scale",scale,delay);
			return tween;
		}
		public static Tween TweenScale(this Control self,float scale,float delay){
			Tween tween = self.CreateTween();
			tween.TweenProperty(self,"scale",new Vector2(self.Scale.X*scale,self.Scale.Y*scale),delay);
			return tween;
		}

		public static Tween TweenScale(this Node2D self,Vector2 scale,float delay){
			Tween tween = self.CreateTween();
			tween.TweenProperty(self,"scale",scale,delay);
			return tween;
		}
		public static Tween TweenScale(this Node2D self,float scale,float delay){
			Tween tween = self.CreateTween();
			tween.TweenProperty(self,"scale",new Vector2(self.Scale.X*scale,self.Scale.Y*scale),delay);
			return tween;
		}

		public static Tween TweenScale(this Node3D self,Vector3 scale,float delay){
			Tween tween = self.CreateTween();
			tween.TweenProperty(self,"scale",scale,delay);
			return tween;
		}
		public static Tween TweenScale(this Node3D self,float scale,float delay){
			Tween tween = self.CreateTween();
			tween.TweenProperty(self,"scale",new Vector3(self.Scale.X*scale,self.Scale.Y*scale,self.Scale.Z*scale),delay);
			return tween;
		}
		#endregion
    }
}
