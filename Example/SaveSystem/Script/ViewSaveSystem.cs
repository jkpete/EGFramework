using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            string[] a = {"Name","Age"};
            DataList[0] = a;
            string[] b = {"Tom","18"};
            DataList[1] = b;
            string[] c = {"Jerry","20"};
            DataList[2] = c;
            this.GetNode<TabContainer>("TabContainer").CreateTable(DataList,"Student");
            DataStudent dataStudent = new DataStudent("S",18);
            DataStudent dataStudent2 = new DataStudent(null,20);
            List<DataStudent> dataStudents = new List<DataStudent>();
            dataStudents.Add(dataStudent);
            dataStudents.Add(dataStudent2);
            this.GetNode<TabContainer>("TabContainer").CreateTable<DataStudent>(dataStudents,"Teacher");
            this.Alert("Hello World");
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