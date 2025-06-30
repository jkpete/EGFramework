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
        public Stack<Window> WindowCache = new Stack<Window>();
        public Window CurrentWindow { set; get; }

        private bool PopUpFlag { set; get; }
        public void Init()
        {

        }

        public void PopupNode(Window window)
        {
            PopUpFlag = true;
            if (CurrentWindow != null)
            {
                this.CurrentWindow.Hide();
                CurrentWindow.VisibilityChanged -= OnPopUpUnitVisibleChanged;
                WindowCache.Push(CurrentWindow);
            }

            window.PopupCentered();
            this.CurrentWindow = window;
            CurrentWindow.VisibilityChanged += OnPopUpUnitVisibleChanged;
            GD.Print(WindowCache.Count);
            PopUpFlag = false;
        }

        public void OnPopUpUnitVisibleChanged()
        {
            if (CurrentWindow != null && !CurrentWindow.Visible && !PopUpFlag)
            {
                GD.Print("-----");
                CurrentWindow.VisibilityChanged -= OnPopUpUnitVisibleChanged;
                if (this.WindowCache.Count > 0)
                {
                    CurrentWindow.Hide();
                    Window lastWindow = WindowCache.Pop();
                    CurrentWindow = lastWindow;
                    CurrentWindow.PopupCentered();
                    CurrentWindow.VisibilityChanged += OnPopUpUnitVisibleChanged;
                }
                else
                {
                    CurrentWindow.Hide();
                    CurrentWindow = null;
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
            singletonNode.PopupNode(nodeData);
            nodeData.Name = name;
            return nodeData;
        }

    }
}
