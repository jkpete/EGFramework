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
            this.EGSave().OpenUserPath();
            // GD.Print(ProjectSettings.GlobalizePath("res://SaveData/Default.json"));
            // GD.Print(ProjectSettings.GlobalizePath("user://SaveData/Default.json"));
            // GD.Print(Path.GetDirectoryName(ProjectSettings.GlobalizePath("res://SaveData/Default.json")));
            // TestLiteDB();
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

        public void TestLiteDB(){
            // 打开数据库 (如果不存在自动创建)
            using(var db = new LiteDatabase("SaveData/DefaultLiteDBData.db"))
            {
                // 获取一个集合 (如果不存在创建)
                LiteCollection<Customer> col = (LiteCollection<Customer>)db.GetCollection<Customer>("customers");
                GD.Print(col);

                // // 创建新顾客实例
                // var customer = new Customer
                // { 
                //     Id = 200,
                //     Name = "Alexander King", 
                //     Phones = new string[] { "8000-0000", "9000-0000" }, 
                //     IsActive = true
                // };
                // // 插入新顾客文档 (Id 自增)
                // for (int i = 0; i < 10000; i++)
                // {
                //     customer.Id ++;
                //     col.Insert(customer);
                // }
                // // 更新集合中的一个文档
                // customer.Name = "Joana Doe";
                // col.Update(customer);
                // // 使用文档的 Name 属性为文档建立索引
                // col.EnsureIndex(x => x.Name);
                // 使用 LINQ 查询文档
                // var results = col.Find(x => x.Name.StartsWith("Al"));
                // GD.Print("Find:"+results.Count());
                // string ids = "";
                // foreach(var item in results){
                //     ids += "["+item.Id.ToString()+"]";
                    
                // }
                // GD.Print(ids);
                // // 让我们创建在电话号码字段上创建一个索引 (使用表达式). 它是一个多键值索引
                // //col.EnsureIndex(x => x.Phones, "$.Phones[*]"); 
                // col.EnsureIndex(x => x.Phones);
                // // 现在我们可以查询电话号码
                // var r = col.FindOne(x => x.Phones.Contains("8888-5555"));\

                // Test Other
                // ILiteCollection<SqliteBackpackItem> col = db.GetCollection<SqliteBackpackItem>("SqliteBackpackItem");
                // var item = new SqliteBackpackItem{
                //     ItemID = 10,
                //     ItemCount = 1,
                //     BackpackID = 1,
                // };
                // for (int i = 0; i < 100; i++)
                // {
                //     col.Insert(item);
                // }
            }
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
        public int Id { get; set; }
        public string Name { get; set; }
        public string[] Phones { get; set; }
        public bool IsActive { get; set; }
    }
}