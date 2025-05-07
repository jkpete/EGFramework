using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;

namespace EGFramework{
    public static class EGGodotTableGenerator {
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
    }
}