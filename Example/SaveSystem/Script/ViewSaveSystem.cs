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
            // TestTree();
            // TestTable();
            // TestJson();
            TestDialog();
        }

        public override void _ExitTree()
        {

        }

        public void TestDialog()
        {
            DataStudent dataStudent = new DataStudent();
            dataStudent.EGenerateDictiontaryByObject();
            this.ExecuteAfterSecond(() =>
            {
                this.EGEditDialog(new DataStudent().EGenerateDictiontaryByObject(), e =>
                {
                    GD.Print("Name:" + e["Name"] + "Age:" + e["Age"]);
                }, "Edit");
            },0.2f);
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
            EGJsonSave jsonManage = this.EGSave().Read<EGJsonSave>("Example", json);
            GD.Print(jsonManage.GetObject<string>("CPU"));

            // JsonTextReader reader = new JsonTextReader(new StringReader(json));
            // while (reader.Read())
            // {
            //     if (reader.Value != null)
            //     {
            //         GD.Print("Token: {"+reader.TokenType+"}, Value: {"+ reader.Value+"}");
            //     }
            //     else
            //     {
            //         GD.Print("Token: {"+ reader.TokenType+"}");
            //     }
            // }
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

            // EGSqliteSave SqliteTest = this.EGSave().Load<EGSqliteSave>("SaveData/test.db");
            // EGodotSaveTable PersonTable = container.CreateNode<EGodotSaveTable>("SQLite");
            // PersonTable.InitSaveData<EGSqliteSave>(SqliteTest);
            // PersonTable.InitData<DataPerson>("person");
        }

    }
    public struct DataStudent
    {
        public int ID;
        public string Name { get; set; }
        public int Age;
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