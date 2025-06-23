using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EGFramework.UI;
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

            Container container = this.GetNode<TabContainer>("TabContainer");
            DataStudent dataStudent = new DataStudent(null, 18);
            DataStudent dataStudent2 = new DataStudent("F", 20);
            List<DataStudent> dataStudents = new List<DataStudent>();
            for (int stu = 0; stu < 10; stu++)
            {
                dataStudents.Add(new DataStudent("stu"+stu, 18));
            }
            for (int stu = 0; stu < 10; stu++)
            {
                dataStudents.Add(dataStudent2);
            }
            EGodotTable table = container.CreateNode<EGodotTable>("Teacher");
            table.InitData<DataStudent>(dataStudents);

            // EGodotTableRowData rowData = container.CreateNode<EGodotTableRowData>("RowData");
            // rowData.Init(new Dictionary<string, object>() { { "Name", "Tom" }, { "Age", 18 } });
            // EGodotRowData rowData2 = container.CreateNode<EGodotRowData>("RowData2");
            // rowData2.Init(new Dictionary<string, object>() { { "Name", "Z" }, { "Age", 1 } });
            // EGodotEditParam editParam = container.CreateNode<EGodotEditParam>("editParam");
            // editParam.Init(new KeyValuePair<string, object>("数量",1));
        }

        public override void _ExitTree()
        {

        }

        
    }
    public struct DataStudent{
        public string Name { get; set; }
        public int Age;
        public int ID;
        public DataStudent(string name,int age){
            Name = name;
            Age = age;
            ID = 0;
        }
    }
}