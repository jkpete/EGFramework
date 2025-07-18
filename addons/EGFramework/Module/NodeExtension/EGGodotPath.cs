using Godot;

namespace EGFramework{

    public partial class EGGodotPath : IModule, IEGFramework
    {
        public void Init()
        {
            
        }

        public void OpenResPath(){
            OS.ShellOpen("".GetGodotResPath());   
        }

        public void OpenUserPath(){
            OS.ShellOpen("".GetGodotUserPath());   
        }
        public IArchitecture GetArchitecture()
        {
            return EGArchitectureImplement.Interface;
        }

    }

     public static class GodotPathExtension{

        public static string GetGodotResPath(this string path){
            return ProjectSettings.GlobalizePath("res://"+path);
        }

        public static string GetGodotUserPath(this string path){
            return ProjectSettings.GlobalizePath("user://"+path);
        }

    }
}