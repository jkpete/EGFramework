using System.Collections.Generic;
using Godot;

namespace EGFramework.UI
{
    public interface IEGodotTree
    {
        void RenderTree();
    }
    public partial class EGodotTree : Tree, IEGFramework,IEGodotTree
    {
        public void RenderTree()
        {

        }
    }

    public struct EGodotTreeItem
    {
        public string Name { set; get; }
        /// <summary>
        /// SVG Code.
        /// </summary>
        /// <value></value>
        public string IconCode { set; get; }

        public Stack<EGodotTreeItem> Parents { set; get; }
        public List<EGodotTreeItem> Childs { set; get; }

        public void AddChild()
        {
            
        }
    }

}