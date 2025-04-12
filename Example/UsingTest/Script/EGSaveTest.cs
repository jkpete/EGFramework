using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using LiteDB;
using Renci.SshNet;

namespace EGFramework.Examples.Test{
    public partial class EGSaveTest : Node,IEGFramework
    {
        public Label Label { set; get; }
        public override void _Ready()
        {
            this.Label = this.GetNode<Label>("Label");
            // this.EGEnabledThread();
            // this.ExecuteAfterSecond(TestMainThreadFunc,2.0f);new PrivateKeyFile("../../../.ssh/id_ed25519")
            //base._Ready();
            //TestCode();

            this.EGEnabledProtocolTool<EGSsh>();
            this.EGEnabledProtocolTool<EGTCPClient>();
            this.EGOnMessage<EasyMessage>();
            // this.EGRegisterMessageEvent<EasyMessage>((e,sender)=>{

            // });
            //TestSsh();
            TestTCPSend();
        }

        public async void TestSsh(){
            await this.EGSsh().ConnectSsh("127.0.0.1","jkpete",new PrivateKeyFile("../../../.ssh/id_ed25519"));
            // await this.EGSsh().ConnectSsh("byserver","bytech","bytech");
            this.EGSendMessage(new EasyMessage(){sendString = "ls -la"},"127.0.0.1",ProtocolType.SSHClient);
        }
        public async void TestTCPSend(){
            await this.EGTCPClient().ConnectTCP("127.0.0.1",5555);
            await this.EGTCPClient().ConnectTCP("127.0.0.1",6666);
            this.EGSendMessage(new EasyMessage(){sendString = "ls -la"},"127.0.0.1:5555",ProtocolType.TCPClient);
            this.EGSendMessage(new EasyMessage(){sendString = "ls -la"},"127.0.0.1:6666",ProtocolType.TCPClient);
            this.EGSendMessage(new EasyMessage(){sendString = "ls -la"},"127.0.0.1:6666",ProtocolType.TCPClient);
            this.EGSendMessage(new EasyMessage(){sendString = "ls -la"},"127.0.0.1:6666",ProtocolType.TCPClient);
            this.EGSendMessage(new EasyMessage(){sendString = "ls -la"},"127.0.0.1:6666",ProtocolType.TCPClient);
            this.EGSendMessage(new EasyMessage(){sendString = "ls -la"},"127.0.0.1:5555",ProtocolType.TCPClient);
        }

        public async void TestThread(){
            await Task.Run(()=>{
                //this.ExecuteInMainThread(TestMainThreadFunc);
                this.ExecuteAfterSecond(TestMainThreadFunc,2.0f);
            });
        }

        public void TestMainThreadFunc(){
            this.Label.Text = "Thread Test";
            GD.Print("Invoked!");
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
        public void TestCode(){
            // this.EGSave().OpenUserPath();
            // GD.Print(ProjectSettings.GlobalizePath("res://SaveData/Default.json"));
            // GD.Print(ProjectSettings.GlobalizePath("user://SaveData/Default.json"));
            // GD.Print(Path.GetDirectoryName(ProjectSettings.GlobalizePath("res://SaveData/Default.json")));
            
            // string Path2 = "Data1.json".GetGodotResPath();
            // this.EGSave().LoadObjectFile<EGJsonSave>(Path2);
            
            // this.EGSave().SetObject(Path2,"Customer1",new Customer() { Name = "Andy" });
            // this.EGSave().SetObject(Path2,"Customer3",new Customer() { Name = "Terry" });

            // string CardPath1 = "Card1";
            // FileAccess fileAccess = FileAccess.Open("res://SaveData/TestCsv.csv", FileAccess.ModeFlags.Read);
            // GD.Print(fileAccess.GetAsText());
            // FileAccess testJson = FileAccess.Open("res://TestJson.json", FileAccess.ModeFlags.Read);
            // this.EGSave().ReadObject<EGJsonSave>("TestJson",testJson.GetAsText());
            // Customer customer = this.EGSave().GetObject<Customer>("TestJson","Customer3");
            // GD.Print("ReadName is "+customer.Name);

            // FileAccess testCSV = FileAccess.Open("res://SaveData/TestCSV.json", FileAccess.ModeFlags.Read);

            
            // EGCsvSave csvSave = new EGCsvSave();
            // csvSave.InitSaveFile("SaveData/TestCsv.csv");
            // Customer testData = csvSave.GetData<Customer>("",1);
            // GD.Print("Name = "+testData.Name +" || ID = "+testData.Id);
            // CustomerByte testData = new CustomerByte(){
            //     Id = 1008,
            //     Name = "AddDataDefault",
            //     IsActive = true
            // };
            // csvSave.SetData("",testData,2)

            string Path1 = "SaveData/TestCsv.csv".GetGodotResPath();
            this.EGSave().LoadDataFile<EGCsvSave>(Path1);
            IEnumerable<Customer> allResult = this.EGSave().FindData<Customer>(Path1,"",cus=>cus.Id==0);
            foreach(Customer customer in allResult){
                GD.Print(customer.Id +"|" + customer.Name);
            }
            // this.EGSave().SetData(Path1,"Customer1",new Customer() { Name = "Andy" },9);
            // IEnumerable<Customer> allResult = this.EGSave().GetAllData<Customer>(Path1,"");
            // foreach(Customer customer in allResult){
            //     GD.Print(customer.Id +"|" + customer.Name);
            // }
            // Customer customer1 = this.EGSave().GetData<Customer>(Path1,"",0);
            // GD.Print(customer1.Id +"|" + customer1.Name);

            
            // FileAccess testCsv = FileAccess.Open("res://SaveData/TestCsv.csv", FileAccess.ModeFlags.Read);
            // this.EGSave().ReadData<EGCsvSave>("TestCsv",testCsv.GetAsText());
            

            // this.EGSave().LoadObjectFile<EGByteSave>("SaveData/testDat.dat");
            // // this.EGSave().SetObject("SaveData/testDat.dat","",testData);
            // CustomerByte testDat = this.EGSave().GetObject<CustomerByte>("SaveData/testDat.dat","");
            // GD.Print(testDat.Id);

            // System.Linq.Expressions.Expression<Func<Customer, bool>> expr = i => i.Name == "Creature";
            // IEnumerable<Customer> linqResult = csvSave.FindData<Customer>("",expr);
            // GD.Print("Find result " + linqResult.Count());
            // foreach(Customer customer in linqResult){
            //     GD.Print(customer.Id);
            // }

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

    public class CustomerByte : Customer, IResponse, IRequest
    {
        public byte[] ToProtocolByteData()
        {
            byte[] data = new byte[0];
            data = data.Concat(((uint)Id).ToBytesLittleEndian()).ToArray();
            return data;
        }

        public string ToProtocolData()
        {
            return "";
        }


        public bool TrySetData(string protocolData, byte[] protocolBytes)
        {
            if(protocolBytes != null && protocolBytes.Length >= 4){
                Id = (int)protocolBytes.ToUINTLittleEndian();
                return true;
            }else{
                return false;
            }
        }
    }

    public class EasyMessage : IResponse, IRequest
    {
        public byte[] sendByte = null;
        public string sendString = null;
        public byte[] ToProtocolByteData()
        {
            return sendByte;
        }

        public string ToProtocolData()
        {
            return sendString;
        }

        public bool TrySetData(string protocolData, byte[] protocolBytes)
        {
            try
            {
                GD.Print("[String]"+protocolData);
                GD.Print("[Bytes]"+protocolBytes);
                return true;
            }
            catch (System.Exception)
            {
                
                throw;
            }
        }
    }
}