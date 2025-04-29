using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using LiteDB;
using Renci.SshNet;

namespace EGFramework.Examples.Test{
    public partial class ViewSaveSystem : Node,IEGFramework
    {
        public string[][] DataList { get; set; }
        public string[][] DataList2 { get; set; }
        public override void _Ready()
        {
            DataList = new string[3][];
            string[] a = {"Name","Age"};
            DataList[0] = a;
            string[] b = {"Tom","18"};
            DataList[1] = b;
            string[] c = {"Jerry","20"};
            DataList[2] = c;
            this.GetNode<TabContainer>("TabContainer").CreateTable(DataList,"Student");
            DataList2 = new string[3][];
            string[] d = {"Name","Age"};
            DataList2[0] = d; 
            string[] e = {"Jim","60"};
            DataList2[1] = e;
            string[] f = {"Bob","50"};
            DataList2[2] = f;
            this.GetNode<TabContainer>("TabContainer").CreateTable(DataList2,"Teacher");
        }

        public override void _ExitTree()
        {
            
        }
        
    }
}