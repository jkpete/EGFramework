using Godot;
using LiteDB;
using System;
using System.IO;

namespace EGFramework.Examples.Test{
    public partial class EGSaveTest : Node,IEGFramework
    {
        public override void _Ready()
        {
            base._Ready();
            // this.EGSave().OpenUserPath();
            // GD.Print(ProjectSettings.GlobalizePath("res://SaveData/Default.json"));
            // GD.Print(ProjectSettings.GlobalizePath("user://SaveData/Default.json"));
            // GD.Print(Path.GetDirectoryName(ProjectSettings.GlobalizePath("res://SaveData/Default.json")));
            // TestLiteDB();
            // string CardPath1 = "SaveData/CardData1.json".GetGodotResPath();
            // this.EGSave().LoadObjectFile<EGJsonSave>(CardPath1);
            // // this.EGSave().SetObject(CardPath1,"Customer1",new Customer() { Name = "Andy" });
            // // this.EGSave().SetObject(CardPath1,"Customer3",new Customer() { Name = "Terry" });
            // Customer customer = this.EGSave().GetObject<Customer>(CardPath1,"Customer3");
            // GD.Print("ReadName is "+customer.Name);

            EGCsvSave csvSave = new EGCsvSave();
            csvSave.InitSaveFile("SaveData/TestCsv.csv");
            // Customer testData = csvSave.GetData<Customer>("",1);
            // GD.Print("Name = "+testData.Name +" || ID = "+testData.Id);
            Customer testData = new Customer(){
                Id = 1008,
                Name = "AddDataDefault",
                IsActive = true
            };
            csvSave.SetData("",testData,2);

            // GD.Print(typeof(Customer));
            // Type type = typeof(Customer);
            // foreach(PropertyInfo property in type.GetProperties()){
            //     GD.Print(property.Name);
            //     CsvParamAttribute csvParam = property.GetCustomAttribute<CsvParamAttribute>();
            //     if(csvParam != null){
            //         GD.Print("["+csvParam._name+"]");
            //     }
            // }
            // foreach(FieldInfo property in type.GetFields()){
            //     GD.Print(property.Name);
            // }
        }

        public void TestSqlite(){
            // string result = this.EGSqlite().CreateTable<SqliteBackpackItem>();
            this.EGSqlite().SaveData(new SqliteBackpackItem{
                ItemID = 10,
                ItemCount = 1,
                BackpackID = 1,
            });
            GD.Print(this.EGSqlite().ExceptionMsg);
            // var properties = typeof(SqliteBackpackItem).GetFields();
            // Godot.GD.Print(properties.Count() + " Readed ");
        }
        
    }
    public struct SqliteBackpackItem{
        public int Id { get; set; }
        public int ItemID { get; set; }
        public int ItemCount { get; set; }
        public int BackpackID { get; set; }
    }

    public class Customer
    {
        [CsvParam("ID")]
        public int Id { get; set; }
        [CsvParam("Name")]
        public string Name { get; set; }
        public string[] Phones { get; set; }
        [CsvParam("是否启用")]
        public bool IsActive { get; set; }
    }
}