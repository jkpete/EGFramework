using Godot;
using static Godot.GD;
using System;
namespace EGFramework{
    public partial class EGThread : Node{
        public override void _Ready()
        {
            base._Ready();
        }

        public override void _Process(double delta)
        {
            base._Process(delta);
        }
    }
    public static class EGThreadExtension
    {
        public static void ExecuteInMainThread(this Node self, Action action){
            //action.Invoke();
        }

        public static void ExecuteAfterSecond(this Node self, Action action,float delay){

        }
    }
}