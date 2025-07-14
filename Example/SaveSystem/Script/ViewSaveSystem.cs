using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EGFramework.UI;
using Godot;
using LiteDB;
using Renci.SshNet;

namespace EGFramework.Examples.Test {
    public partial class ViewSaveSystem : Node, IEGFramework
    {
        public string[][] DataList { get; set; }
        public string[][] DataList2 { get; set; }
        public override void _Ready()
        {
            Container container = this.GetNode<TabContainer>("TabContainer");
            List<DataStudent> dataStudents = new List<DataStudent>();
            for (int stu = 0; stu < 10; stu++)
            {
                dataStudents.Add(new DataStudent("stu" + stu, 18));
            }
            for (int stu = 0; stu < 11; stu++)
            {
                dataStudents.Add(new DataStudent("A" + stu, 20 + stu));
            }
            EGodotTable table = container.CreateNode<EGodotTable>("Default");
            table.InitData<DataStudent>(dataStudents);

            EGSqliteSave SqliteTest = this.EGSave().Load<EGSqliteSave>("SaveData/test.db");
            // IEnumerable<string> dataBaseKey = SqliteTest.GetKeys();
            // GD.Print(dataBaseKey);
            // foreach (string data in dataBaseKey)
            // {
            //     GD.Print(data);
            // }
            EGodotSaveTable PersonTable = container.CreateNode<EGodotSaveTable>("SQLite");
            PersonTable.InitSaveData<EGSqliteSave>(SqliteTest);
            PersonTable.InitData<DataPerson>("person");

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
    public struct DataStudent
    {
        public string Name { get; set; }
        public int Age;
        public int ID;
        public EGPathSelect Path { set; get; }
        public DataStudent(string name, int age)
        {
            Name = name;
            Age = age;
            ID = 0;
            Path = new EGPathSelect();
        }
    }
    
    public struct DataPerson{
        public string id { get; set; }
        public string namee { set; get; }
        public string workPlace { set; get; }
        public string policeNum { set; get; }
    }
}