using System;
using System.Diagnostics;
namespace EGFramework{

	public interface IPlatform{
		void Log(string message);
		void Log(params object[] what);
	}
	public class EGPlatformGodot : IPlatform{
		public void Log(string message){
			Godot.GD.Print(message);
			// Console.WriteLine(message);
		}
		public void Log(params object[] what){
			Godot.GD.Print(what);
			// Console.WriteLine(what);
		}
	}
	// if not use please explain this
	public class EGPlatformDotnet : IPlatform{
		public void Log(string message){
			Console.WriteLine(message);
		}
		public void Log(params object[] what){
			Console.WriteLine(what);
		}
	}
	public static class EG
	{
		public static EGPlatformGodot Platform = new EGPlatformGodot();
		public static void Print(string message){
			Platform.Log(message);
		}
		public static void Print(params object[] what){
			Platform.Log(what);
		}

	}

	// public enum SupportPlatform{
	// 	Godot = 0x01,
	// 	Unity = 0x02,
	// 	WebApi = 0x03,
	// 	WPF = 0x04,
	// 	Form = 0x05,
	// }

}
