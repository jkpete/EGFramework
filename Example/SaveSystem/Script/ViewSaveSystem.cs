using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EGFramework.UI;
using Godot;
using LiteDB;
using Newtonsoft.Json;
using Renci.SshNet;

namespace EGFramework.Examples.Test
{
    public partial class ViewSaveSystem : Node, IEGFramework
    {
        public string[][] DataList { get; set; }
        public string[][] DataList2 { get; set; }
        Container container { set; get; }
        public override void _Ready()
        {
            // TestTree();
            // TranslationServer.SetLocale("jp");
            // GD.Print(Tr("Data")+"+___+");
            // TestTable();
            // TestJson();
            this.CallDeferred("TestDialog");
            // SchoolType school = SchoolType.London;
            // school.EGenerateMappingByEnum();
            // foreach (KeyValuePair<int, string> selectOptions in school.EGenerateMappingByEnum())
            // {
            //     GD.Print(selectOptions.Key+"---"+selectOptions.Value);
            // }
            // TestDialog();
            // TestMySQL();
            // EG.Print(OS.GetLocaleLanguage());
        }

        public override void _ExitTree()
        {

        }

        public void TestMySQL()
        {
            EGDapper mysqlSave = this.EGSave().Load<EGMysqlSave>("server=" + "localhost" + ";port=" + "3306" + ";uid=" + "root" + ";pwd=" + "root" + ";database=" + "Test3" + ";");
            bool isExist = mysqlSave.ContainsData("DataStudent", 3);
            GD.Print(isExist);
            // mysqlSave.CreateTable<DataStudent>("DataStudent");
            // DataStudent stuData = new DataStudent("Bob", 12);
            // stuData.Path = new EGPathSelect(){Path = "AA"};
            // mysqlSave.AddData("DataStudent",stuData);
            // DataStu stu1 = new DataStu("Anti", 20,"London");
            // mysqlSave.AddData("DataStudent",stu1);
            // DataStu stu2 = new DataStu("CC", 23,"NewYork"){Age = 19};
            // DataStu stu3 = new DataStu("Rocket", 24,"Paris"){Age = 26};
            // List<DataStu> stuList = new List<DataStu>();
            // stuList.Add(stu2);
            // stuList.Add(stu3);
            // mysqlSave.AddData<DataStu>("DataStudent",stuList);
            // mysqlSave.RemoveData("DataStudent",2);
            // IEnumerable<DataStu> findStudent = mysqlSave.FindData<DataStu>("DataStudent", e => e.Name == "CC");
            // IEnumerable<DataStu> findStudent = mysqlSave.FindData<DataStu>("DataStudent","Name","CC");
            // GD.Print(findStudent.Count() +" data has been find!");
            // int count = 0;
            // foreach (DataStu stu in findStudent)
            // {
            //     DataStu NewData = new DataStu("CC_" + count, stu.Age, stu.Path);
            //     mysqlSave.UpdateData("DataStudent", NewData, stu.ID);
            //     count++;
            // }
        }

        public void TestDialog()
        {
            DataStudent dataStudent = new DataStudent("ZG",10);
            this.EGEditDialog(dataStudent.EGenerateDictiontaryByObject(), e =>
            {
                GD.Print("Name:" + e["Name"] + "Age:" + e["Age"]+"School:" + e["School"] + "Path:" + e["Path"]);
            }, "Edit");
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

            EGSqliteSave SqliteTest = this.EGSave().Load<EGSqliteSave>("SaveData/test.db");
            EGodotSaveTable PersonTable = container.CreateNode<EGodotSaveTable>("SQLite");
            PersonTable.InitSaveData<EGSqliteSave>(SqliteTest);
            PersonTable.InitData<DataPerson>("person");
        }

    }
    public struct DataStudent
    {
        public int ID;
        public string Name { get; set; }
        public int Age;
        public EGPathSelect Path { set; get; }
        public SchoolType School { set; get; }
        public DataStudent(string name, int age)
        {
            Name = name;
            Age = age;
            ID = 0;
            School = SchoolType.MIT;
            Path = new EGPathSelect();
        }
    }

    public struct DataStu
    {
        public int ID;
        public string Name { get; set; }
        public int Age { set; get; }
        public string Path { set; get; }
        public DataStu(string name, int age, string path)
        {
            Name = name;
            Age = age;
            ID = 0;
            Path = path;
        }
    }

    public struct DataPerson
    {
        public string id { get; set; }
        public string namee { set; get; }
        public string workPlace { set; get; }
        public string policeNum { set; get; }
    }

    public enum SchoolType
    {
        Tsinghua = 0,
        MIT = 1,
        London = 2,
        Data = 3
    }
}