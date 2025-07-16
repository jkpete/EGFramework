using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EGFramework.UI;
using Godot;
using LiteDB;
using Newtonsoft.Json;
using Renci.SshNet;

namespace EGFramework.Examples.Test {
    public partial class ViewSaveSystem : Node, IEGFramework
    {
        public string[][] DataList { get; set; }
        public string[][] DataList2 { get; set; }
        Container container{ set; get; }
        public override void _Ready()
        {
            TestTree();
        }

        public override void _ExitTree()
        {

        }

        public void TestJson()
        {
            string json = @"{
                'CPU': 'Intel',
                'PSU': '500W',
                'Drives': [
                    'DVD read/writer'
                    /*(broken)*/,
                    '500 gigabyte hard drive',
                    '200 gigabyte hard drive'
                ],
                'My' : {
                    'AA':'BB',
                    'Date': new Date(123456789)
                }
            }";

            JsonTextReader reader = new JsonTextReader(new StringReader(json));
            while (reader.Read())
            {
                if (reader.Value != null)
                {
                    GD.Print("Token: {"+reader.TokenType+"}, Value: {"+ reader.Value+"}");
                }
                else
                {
                    GD.Print("Token: {"+ reader.TokenType+"}");
                }
            }
        }

        public void TestTree()
        {
            string json = @"{
                'CPU': 'Intel',
                'PSU': '500W',
                'My' : {
                    'AA':'BB',
                    'Date': 111
                }
            }";
            container = this.GetNode<TabContainer>("TabContainer");
            EGodotTree eGodotTree = container.CreateNode<EGodotTree>("TestTree");
            eGodotTree.InitByJson(json);
        }

        public void TestTable()
        {
            container = this.GetNode<TabContainer>("TabContainer");
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