using System.Collections.Generic;
using Godot;

namespace EGFramework.UI
{
    public interface IEGodotTree
    {
        void RenderTree();
    }
    public partial class EGodotTree : Tree, IEGFramework, IEGodotTree
    {
        public EGTree Tree { set; get; }

        public void Init(EGTree tree)
        {
            this.Tree = tree;
            RenderTree();
        }

        public void InitByJson(string json)
        {
            EGTree eGTree = EGTreeFactory.CreateTreeByJson(json);
            this.Tree = eGTree;
            RenderTree();
        }

        public void RenderTree()
        {
            this.HideRoot = true;
            CreateTreeItem(Tree, this.GetRoot());
        }
        

        public void CreateTreeItem(EGTree tree,TreeItem parent)
        {
            TreeItem current = this.CreateItem(parent);
            current.SetText(0,tree.Name);
            current.SetTooltipText(0, tree.StrValue);
            foreach (EGTree child in tree.GetChilds())
            {
                CreateTreeItem(child, current);
            }
        }
    }

}