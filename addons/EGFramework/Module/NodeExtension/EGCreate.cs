using System.Runtime.CompilerServices;
using Godot;
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
        public static HBoxContainer CreateRowData(this Node self,string[] titleList,string rowName = "RowData"){
            HBoxContainer RowData = new HBoxContainer();
            RowData.Name = rowName;
            foreach(string s in titleList){
                Label label = new Label();
                label.Name = s;
                label.Text = s;
                label.HorizontalAlignment = HorizontalAlignment.Center;
                label.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
                RowData.AddChild(label);
            }
            self.AddChild(RowData);
            return RowData;
        }
        public static VBoxContainer CreateTable(this Node self,string[][] tableStr,string tableName = "Table"){
            VBoxContainer Table = new VBoxContainer();
            Table.Name = tableName;
            int dataPointer = 0;
            foreach(string[] s in tableStr){
                Table.CreateRowData(s,"tableRowData"+dataPointer);
                dataPointer++;
            }
            self.AddChild(Table);
            EG.Print("CreateTable",tableStr.Length);
            return Table;
        }
    }
}
