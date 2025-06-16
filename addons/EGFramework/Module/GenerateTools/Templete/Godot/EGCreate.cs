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
        
        public static TNode SingletoneNode<TNode>(this Node self) where TNode : Node, new()
        {
            TNode nodeData;
            if (EGArchitectureImplement.Interface.GetModule<EGSingletonNode>().NodeContainer.Get<TNode>() != null)
            {
                nodeData = EGArchitectureImplement.Interface.GetModule<EGSingletonNode>().NodeContainer.Get<TNode>();
            }
            else
            {
                nodeData = self.CreateNode<TNode>();
                EGArchitectureImplement.Interface.GetModule<EGSingletonNode>().NodeContainer.Register(nodeData);
            }
            nodeData.Name = typeof(TNode).Name;
            return nodeData;
        }
        public static TNode SingletoneNode<TNode>(this Node self,string name) where TNode : Node, new()
        {
            TNode nodeData;
            if (EGArchitectureImplement.Interface.GetModule<EGSingletonNode>().NodeContainer.Get<TNode>() != null)
            {
                nodeData = EGArchitectureImplement.Interface.GetModule<EGSingletonNode>().NodeContainer.Get<TNode>();
            }
            else
            {
                nodeData = self.CreateNode<TNode>();
                EGArchitectureImplement.Interface.GetModule<EGSingletonNode>().NodeContainer.Register(nodeData);
            }
            nodeData.Name = name;
            return nodeData;
        }

    }
}
