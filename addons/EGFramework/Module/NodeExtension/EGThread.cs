using Godot;
using static Godot.GD;
using System;
using System.Collections.Generic;

namespace EGFramework{
    public partial class EGThread : Node,IModule,IEGFramework{

        public EasyEventOnce EventPool = new EasyEventOnce();
        public Dictionary<Action,SceneTreeTimer> EventDelayPool = new Dictionary<Action, SceneTreeTimer>();

        public void Init()
        {
            
        }

        public override void _Ready()
        {

        }

        public override void _Process(double delta)
        {
            //base._Process(delta);
            EventPool.Invoke();
        }

        public void ExecuteInMainThread(Action action){
            //ActionQueue.Enqueue(action);
            EventPool.Register(action);
        }

        public void ExecuteAfterSecond(Action action,double delay){
            SceneTreeTimer timer = this.GetTree().CreateTimer(delay);
            timer.Timeout += action;
            timer.Timeout += () => EventDelayPool.Remove(action);
            EventDelayPool.Add(action,timer);
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

        public static void ExecuteAfterSecond(this Node self, Action action,double delay){
            self.NodeModule<EGThread>().ExecuteAfterSecond(action,delay);
        }

        public static void EGEnabledThread(this Node self){
            self.NodeModule<EGThread>();
        }
    }
}