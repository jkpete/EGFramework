using Godot;
using static Godot.GD;
using System;
using System.Collections.Generic;

namespace EGFramework{
    public partial class EGThread : Node,IModule,IEGFramework{

        public Queue<Action> ActionQueue = new Queue<Action>();
        public IOCContainer ActionPool = new IOCContainer();

        public void Init()
        {
            
        }
        public override void _Ready()
        {

        }

        public override void _Process(double delta)
        {
            //base._Process(delta);
            if(ActionQueue.Count>0){
                Action execute = ActionQueue.Dequeue();
                execute.Invoke();
                execute = null;
            }
        }

        public void ExecuteInMainThread(Action action){
            ActionQueue.Enqueue(action);
        }

        public void ExecuteInMainThread<T>(Action<T> action){

        }


        public IArchitecture GetArchitecture()
        {
            return EGArchitectureImplement.Interface;
        }
    }
    public static class EGThreadExtension
    {
        public static void ExecuteInMainThread(this Node self, Action action){
            //action.Invoke();
            self.NodeModule<EGThread>().ExecuteInMainThread(action);
        }

        public static void ExecuteInMainThread<T>(this Node self, Action<T> action){
            //action.Invoke();
        }

        public static void ExecuteAfterSecond(this Node self, Action action,float delay){

        }

        public static void ExecuteAfterSecond<T>(this Node self, Action<T> action,float delay){

        }

        public static void EGEnabledThread(this Node self){
            self.NodeModule<EGThread>();
        }
    }
}