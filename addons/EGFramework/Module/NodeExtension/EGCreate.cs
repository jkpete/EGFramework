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
        
        public static void Alert(this Node self,string alertMsg){
            AcceptDialog acceptDialog;
            if(EGArchitectureImplement.Interface.GetModule<EGSingletonNode>().NodeContainer.Get<AcceptDialog>()!=null){
                acceptDialog = EGArchitectureImplement.Interface.GetModule<EGSingletonNode>().NodeContainer.Get<AcceptDialog>();
            }else{
                acceptDialog = self.CreateNode<AcceptDialog>();
            }
            EGArchitectureImplement.Interface.GetModule<EGSingletonNode>().NodeContainer.Register(acceptDialog);
            acceptDialog.Name = "AlertDialog";
            acceptDialog.Title = "Alert";
            acceptDialog.DialogText = alertMsg;
            acceptDialog.PopupCentered();
        }
        public static Tree CreateTree(this Node self,string treeName = "Tree"){
            Tree tree = new Tree();
            tree.Name = treeName;
            self.AddChild(tree);
            return tree;
        }
    }
}
