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
        public static HBoxContainer CreateRowData(this Node self,string[] titleList,string rowName = "RowData"){
            HBoxContainer RowData = new HBoxContainer();
            RowData.Name = rowName;
            foreach(string s in titleList){
                Label label = new Label();
                if(s != null){
                    label.Name = s;
                    label.Text = s;
                }else{
                    label.Name = "Null";
                    label.Text = "Null";
                }
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
            return Table;
        }
        public static VBoxContainer CreateTable<T>(this Node self,IEnumerable<T> tableData,string tableName = "ObjectTable",int limit = 0){
            VBoxContainer Table = new VBoxContainer();
            Table.Name = tableName;
            MemberInfo[] propertyNames = typeof(T).GetProperties();
            MemberInfo[] fieldNames = typeof(T).GetFields();
            MemberInfo[] memberInfos = propertyNames.Concat(fieldNames).ToArray();
            string[] propertyName = new string[memberInfos.Length];
            int dataPointer = 0;
            for (int i = 0; i < memberInfos.Length; i++)
            {
                propertyName[i] = memberInfos[i].Name;
            }
            Table.CreateRowData(propertyName,"Title");
            foreach (T t in tableData)
            {
                string[] s = t.GetType().GetProperties().Select(p => p.GetValue(t)?.ToString()).ToArray();
                string[] a = t.GetType().GetFields().Select(p => p.GetValue(t)?.ToString()).ToArray();
                string[] result = s.Concat(a).ToArray();
                Table.CreateRowData(result, "tableRowData"+dataPointer);
                dataPointer++;
            }
            self.AddChild(Table);
            return Table;
        }
        public static Tree CreateTree(this Node self,string treeName = "Tree"){
            Tree tree = new Tree();
            tree.Name = treeName;
            self.AddChild(tree);
            return tree;
        }
    }
}
