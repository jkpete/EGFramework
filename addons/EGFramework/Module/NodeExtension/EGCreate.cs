using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Godot;
using Mysqlx.Crud;
using static Godot.GD;

namespace EGFramework{

    public class EGSingletonNode : IEGFramework, IModule
    {
        public IOCContainer NodeContainer = new IOCContainer();
        public void Init()
        {
            
        }

        public IArchitecture GetArchitecture()
        {
            return EGArchitectureImplement.Interface;
        }
        
    }

    public static class EGCanCreateNodeExtension
    {
        public static TNode CreateNode<TNode>(this Node self) where TNode : Node,new(){
            TNode nodeData = new TNode();
            nodeData.Name = typeof(TNode).Name;
            self.AddChild(nodeData);
            return nodeData;
        }

        public static TNode CreateNode<TNode>(this Node self,string name) where TNode : Node,new(){
            TNode nodeData = new TNode();
            nodeData.Name = name;
            self.AddChild(nodeData);
            return nodeData;
        }
        
        public static void Alert(this Node self,string alertMsg,string title = "Alert"){
            AcceptDialog acceptDialog;
            if(EGArchitectureImplement.Interface.GetModule<EGSingletonNode>().NodeContainer.Get<AcceptDialog>()!=null){
                acceptDialog = EGArchitectureImplement.Interface.GetModule<EGSingletonNode>().NodeContainer.Get<AcceptDialog>();
            }else{
                acceptDialog = self.CreateNode<AcceptDialog>();
            }
            EGArchitectureImplement.Interface.GetModule<EGSingletonNode>().NodeContainer.Register(acceptDialog);
            acceptDialog.Name = "AlertDialog";
            acceptDialog.Title = title;
            acceptDialog.DialogText = alertMsg;
            acceptDialog.PopupCentered();
        }

        public static void Confirm(this Node self,string alertMsg,Action<bool> callback,string title = "Confirm"){
            ConfirmationDialog confirmDialog;
            if(EGArchitectureImplement.Interface.GetModule<EGSingletonNode>().NodeContainer.Get<ConfirmationDialog>()!=null){
                confirmDialog = EGArchitectureImplement.Interface.GetModule<EGSingletonNode>().NodeContainer.Get<ConfirmationDialog>();
            }else{
                confirmDialog = self.CreateNode<ConfirmationDialog>();
            }
            EGArchitectureImplement.Interface.GetModule<EGSingletonNode>().NodeContainer.Register(confirmDialog);
            confirmDialog.Name = "ConfirmDialog";
            confirmDialog.Title = title;
            confirmDialog.DialogText = alertMsg;
            confirmDialog.PopupCentered();
            confirmDialog.Connect("confirmed",Callable.From(() => {
                callback(true);
            }));
            confirmDialog.Connect("canceled",Callable.From(() => {
                callback(false);
            }));
        }
        public static Tree CreateTree(this Node self,string treeName = "Tree"){
            Tree tree = new Tree();
            tree.Name = treeName;
            self.AddChild(tree);
            return tree;
        }
    }
}
