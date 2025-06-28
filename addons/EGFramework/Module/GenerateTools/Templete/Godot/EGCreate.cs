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
        public Queue<Window> WindowCache = new Queue<Window>();
        public Window CurrentWindow { set; get; }
        public void Init()
        {

        }

        public void ReturnToLastWindow()
        {
            if (CurrentWindow != null && !CurrentWindow.Visible)
            {
                GD.Print("-----");
                if (this.WindowCache.Count > 0)
                {
                    this.CurrentWindow.VisibilityChanged -= ReturnToLastWindow;
                    this.CurrentWindow.Hide();
                    this.CurrentWindow = this.WindowCache.Dequeue();
                    this.CurrentWindow?.PopupCentered();
                }
            }
        }

        public IArchitecture GetArchitecture()
        {
            return EGArchitectureImplement.Interface;
        }
        
    }

    public static class EGCanCreateNodeExtension
    {
        public static TNode CreateNode<TNode>(this Node self) where TNode : Node, new()
        {
            TNode nodeData = new TNode();
            nodeData.Name = typeof(TNode).Name;
            self.AddChild(nodeData);
            return nodeData;
        }

        public static TNode CreateNode<TNode>(this Node self, string name) where TNode : Node, new()
        {
            TNode nodeData = new TNode();
            nodeData.Name = name;
            self.AddChild(nodeData);
            return nodeData;
        }

        public static TNode SingletoneNode<TNode>(this Node self, string name) where TNode : Node, new()
        {
            TNode nodeData;
            if (EGArchitectureImplement.Interface.GetModule<EGSingletonNode>().NodeContainer.Get<TNode>() != null)
            {
                nodeData = EGArchitectureImplement.Interface.GetModule<EGSingletonNode>().NodeContainer.Get<TNode>();
            }
            else
            {
                nodeData = self.GetTree().Root.CreateNode<TNode>();
                EGArchitectureImplement.Interface.GetModule<EGSingletonNode>().NodeContainer.Register(nodeData);
            }
            nodeData.Name = name;
            return nodeData;
        }

        public static TWindowNode PopupNode<TWindowNode>(this Node self, string name) where TWindowNode : Window, new()
        {
            TWindowNode nodeData;
            EGSingletonNode singletonNode = EGArchitectureImplement.Interface.GetModule<EGSingletonNode>();
            if (singletonNode.NodeContainer.Get<TWindowNode>() != null)
            {
                nodeData = singletonNode.NodeContainer.Get<TWindowNode>();
            }
            else
            {
                nodeData = self.GetTree().Root.CreateNode<TWindowNode>();
                singletonNode.NodeContainer.Register(nodeData);
            }
            singletonNode.WindowCache.Enqueue(nodeData);
            nodeData.Name = name;
            // nodeData.CloseRequested += () =>
            // {
            //     if (!nodeData.Visible)
            //     {
            //         GD.Print("-----");
            //         if (singletonNode.WindowCache.Count > 0)
            //         {
            //             singletonNode.CurrentWindow = singletonNode.WindowCache.Dequeue();
            //             singletonNode.CurrentWindow?.PopupCentered();
            //         }
            //     }
            // };
            singletonNode.CurrentWindow?.Hide();
            nodeData.PopupCentered();
            singletonNode.CurrentWindow = nodeData;
            // nodeData.VisibilityChanged += singletonNode.ReturnToLastWindow;
            return nodeData;
        }

    }
}
