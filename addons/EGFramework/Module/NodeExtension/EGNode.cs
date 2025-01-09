using Godot;
using static Godot.GD;
using System;
namespace EGFramework{
    public static class EGNodeExtension
    {
        public static TModule NodeModule<TModule>(this Node self) where TModule : Node,IModule,new(){
            if(EGArchitectureImplement.Interface.IsInitModule<TModule>()){
                TModule module = new TModule();
                module.Name = typeof(TModule).ToString();
                Print(module.Name);
                self.AddChild(module);
                EGArchitectureImplement.Interface.RegisterModule(module);
                return module;
            }else{
                return EGArchitectureImplement.Interface.GetModule<TModule>();
            }
        }

        public static void ClearChildren(this Node itemContainer)
        {
            foreach (Node child in itemContainer.GetChildren())
            {
                child.QueueFree();
            }
        }

        public static void ClearChildren<T>(this Node itemContainer) where T : Node
        {
            foreach (Node child in itemContainer.GetChildren())
            {
                if(child.GetType()==typeof(T)){
                    child.QueueFree();
                }
            }
        }

        public static void ClearChildrenLabel(this Node itemContainer)
        {
            foreach (Node child in itemContainer.GetChildren())
            {
                if(child.GetType()==typeof(Label)){
                    child.QueueFree();
                }
            }
        }

    }
}