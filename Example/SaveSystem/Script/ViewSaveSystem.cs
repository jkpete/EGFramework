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
            DataList = new string[3][];
            string[] a = { "Name", "Age" };
            DataList[0] = a;
            string[] b = { "Tom", "18" };
            DataList[1] = b;
            string[] c = { "Jerry", "20" };
            DataList[2] = c;
            this.GetNode<TabContainer>("TabContainer").CreateTable(DataList, "Student");
            DataStudent dataStudent = new DataStudent("S", 18);
            DataStudent dataStudent2 = new DataStudent(null, 20);
            List<DataStudent> dataStudents = new List<DataStudent>();
            dataStudents.Add(dataStudent);
            dataStudents.Add(dataStudent2);
            EGodotTable table = this.GetNode<TabContainer>("TabContainer").CreateTable<DataStudent>(dataStudents, "Teacher");
            // Button btn = this.CreateNode<Button>("Test");
            // btn.Text = "Test";
            // btn.Position = new Vector2(100,100);
            // btn.Connect("pressed",Callable.From (() => { 
            //     this.Alert("Test");
            // }));
            // EGodotEditDialog Edit = this.CreateNode<EGodotEditDialog>("Edit");
            // Edit.InitDialog(new Dictionary<string, object>() {{"Name","Tom"},{"Age",18}},(data) => {
            //     GD.Print(data["Name"]);
            //     GD.Print(data["Age"]);
            // });

            EGodotTableRowData rowData = table.CreateNode<EGodotTableRowData>("RowData");
            rowData.Init(new Dictionary<string, object>() { { "Name", "Tom" }, { "Age", 18 } });
            EGodotRowData rowData2 = table.CreateNode<EGodotRowData>("RowData2");
            rowData2.Init(new Dictionary<string, object>() { { "Name", "Z" }, { "Age", 1 } });
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