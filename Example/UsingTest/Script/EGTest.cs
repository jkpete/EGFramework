using Godot;
using static Godot.GD;
using System.Collections.Generic;
using EGFramework;
using Newtonsoft.Json;
using System;
using System.Net;

namespace EGFramework.Examples{
    public partial class EGTest : Node,IEGFramework
    {
        public Label label { set; get; }

        public override void _Ready()
        {
            this.EGEnabledProtocolTools();
            
            this.EGMqtt().ConnectMQTTServer("192.168.1.220");
            
            //this.EGUDP().UDPDevices[5555].Connect(IPAddress.Parse("224.0.0.251"),5353);
            //byte[] sendData = { 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x09, 0x5F, 0x73, 0x65, 0x72, 0x76, 0x69, 0x63, 0x65, 0x73, 0x07, 0x5F, 0x64, 0x6E, 0x73, 0x2D, 0x73, 0x64, 0x04, 0x5F, 0x75, 0x64, 0x70, 0x05, 0x6C, 0x6F, 0x63, 0x61, 0x6C, 0x00, 0x00, 0x0C, 0x00, 0x01 };
            //this.EGUDP().UDPDevices[5555].Send(sendData);
            // this.EGRegisterMessageEvent<PrintResponse>((e,sender,protocol)=>{
            //     Print(sender);
            // });
            // this.EGOnMessage<PrintResponse>();
            // this.EGReadFromFile("SaveData/MySeg2.seg2");
            //TestTCPClient();
            //TestSerialPort();
            //TestTCPServer();
            //this.EGUDP();
            //this.EGUDPListen(11000);
            //this.EGSendMessage(new MessageStruct(1,"xxx"),"192.168.1.192:9002",ProtocolType.UDP);
            //this.EGHttpClient();
            //this.EGHttpServer().Listen("http://+:6555/index/");
        }
        public override void _Process(double delta)
        {
            //Print(this.EGMqtt().MqttDevices["192.168.1.220"].IsConnected);
        }
        public override void _ExitTree()
        {

        }

        public void OnButton1Click(){
            this.EGMqtt().SubScribeTheme("192.168.1.220","test");
            byte[] testData = {0x3A,0x55};
            this.EGMqtt().PublishTheme("192.168.1.220","test",testData);
        }
        
        public void RefreshMsg(bool coil){
            label.Text += coil+" ";
        }
        public void OnTimer(){
            //this.EGSendMessage(new MessageStruct(1,"山东博裕1"),"COM4",ProtocolType.SerialPort);
            //this.EGSendMessage(new MessageStruct(1,"山东博裕1"),"192.168.1.244:6060",ProtocolType.TCPClient);
            //this.EGSendMessage(new ModbusTCP_WriteMultiCoil(1,0,sendData),"192.168.1.200:3000",ProtocolType.TCPClient);
            //TestSingleCoil();
            //TestMultiCoil();
            //TestSingleHoldingRegister();
            //TestMultiHoldingRegister();
        }
        public void TestModbus(){
            label = this.GetNode<Label>("Label");
            label.Text = "[Modbus]";
            this.EGRegisterMessageEvent<MessageResponse>((e,sender,type)=>{
                Print("[Got Response]"+"[sender = "+sender+"]"+"["+type+"]"+ e.MessageId+"||"+e.Author);
            });
            this.EGOnMessage<MessageResponse>();
            this.EGRegisterMessageEvent<ModbusTCP_Response>((e,sender,type)=>{
                if(type == ProtocolType.TCPClient && e.FunctionType == ModbusFunctionType.ReadCoil){
                    int registerId = 0;
                    foreach(bool coil in e.Coil){
                        Print(registerId + "Status：" + coil);
                        registerId++;
                    }
                }
                if(type == ProtocolType.TCPClient && e.FunctionType == ModbusFunctionType.ReadDiscreteInput){
                    int registerId = 0;
                    foreach(bool discreteInput in e.DiscreteInput){
                        Print(registerId + "Status：" + discreteInput);
                        registerId++;
                    }
                }
                if(type == ProtocolType.TCPClient && e.FunctionType == ModbusFunctionType.ReadHoldingRegisters){
                    int registerId = 0;
                    foreach(ushort holdingRegister in e.HoldingRegister){
                        Print(registerId + "Status：" + holdingRegister);
                        registerId++;
                    }
                }
                if(type == ProtocolType.TCPClient && e.FunctionType == ModbusFunctionType.ReadInputRegisters){
                    int registerId = 0;
                    foreach(ushort inputRegister in e.InputRegister){
                        Print(registerId + "Status：" + inputRegister);
                        label.Text+=registerId + "Status：" + inputRegister;
                        registerId++;
                    }
                }
                //this.EGOffMessage<ModbusTCP_Response>();
            });

            this.EGRegisterMessageEvent<ModbusRTU_Response>((e,sender,type)=>{
                if(type == ProtocolType.SerialPort && e.FunctionType == ModbusFunctionType.ReadCoil){
                    int registerId = 0;
                    foreach(bool coil in e.Coil){
                        Print(registerId + "Status：" + coil);
                        registerId++;
                    }
                }
                if(type == ProtocolType.SerialPort && e.FunctionType == ModbusFunctionType.ReadDiscreteInput){
                    int registerId = 0;
                    foreach(bool discreteInput in e.DiscreteInput){
                        Print(registerId + "Status：" + discreteInput);
                        registerId++;
                    }
                }
                if(type == ProtocolType.SerialPort && e.FunctionType == ModbusFunctionType.ReadHoldingRegisters){
                    int registerId = 0;
                    foreach(ushort holdingRegister in e.HoldingRegister){
                        Print(registerId + "Status：" + holdingRegister);
                        registerId++;
                    }
                }
                if(type == ProtocolType.SerialPort && e.FunctionType == ModbusFunctionType.ReadInputRegisters){
                    int registerId = 0;
                    foreach(ushort inputRegister in e.InputRegister){
                        Print(registerId + "Status：" + inputRegister);
                        label.Text+=registerId + "Status：" + inputRegister;
                        registerId++;
                    }
                }
                //this.EGOffMessage<ModbusRTU_Response>();
            });
            
        }

        public void TestTCPServer(){
            this.EGTCPServer().OnClientConnect.Register(e=>{
                Print(e +" is connected");
                this.EGSendMessage(new MessageStruct(1,"Hello"),e,ProtocolType.TCPServer);
            });
            this.EGTCPServer().OnClientDisconnect.Register(e=>{
                Print(e +" is disconnect");
            });
            this.EGTCPServerListen(9999);
        }

        public void TestSqlite(){
            this.EGSqlite().SaveData(new TestBoxMessage());
            List<TestBoxMessage> result = this.EGSqlite().GetDataSet<TestBoxMessage>();
            if(result == null){
                PrintErr(this.EGSqlite().ExceptionMsg);
            }
            Print("Result = " + result[0].TestDouble + result[0].TestFloat);
        }
        public void TestSerialPort(){
            this.EGSerialPort().SetBaudRate(9600);
            this.EGSendMessage(new ModbusRTU_ReadInputRegisters(1,0,2),"COM4",ProtocolType.SerialPort);
            this.EGSendMessage(new ModbusRTU_ReadCoils(1,0,8),"COM4",ProtocolType.SerialPort);
            this.EGOnMessage<ModbusRTU_Response>();
        }

        public void TestTCPClient(){
            this.EGTCPClient();
            this.EGSendMessage(new ModbusTCP_ReadDiscreteInput(1,0,8),"192.168.1.196:6000",ProtocolType.TCPClient);
            this.EGSendMessage(new ModbusTCP_ReadInputRegisters(1,0,2),"192.168.1.196:6000",ProtocolType.TCPClient);
            this.EGOnMessage<ModbusTCP_Response>();
        }

        private bool IsOpen = false;
        public void TestSingleCoil(){
            if(IsOpen){
                //this.EGSendMessage(new ModbusRTU_WriteSingleCoil(1,0,false),"COM4",ProtocolType.SerialPort);
                this.EGSendMessage(new ModbusTCP_WriteSingleCoil(1,0,false),"192.168.1.196:6000",ProtocolType.TCPClient);
                IsOpen = false;
            }else{
                //this.EGSendMessage(new ModbusRTU_WriteSingleCoil(1,0,true),"COM4",ProtocolType.SerialPort);
                this.EGSendMessage(new ModbusTCP_WriteSingleCoil(1,0,true),"192.168.1.196:6000",ProtocolType.TCPClient);
                IsOpen = true;
            }
        }
        public void TestMultiCoil(){
            byte[] OpenCode = {0xFF};
            byte[] CloseCode = {0x00};
            if(IsOpen){
                this.EGSendMessage(new ModbusRTU_WriteMultiCoil(1,0,CloseCode.ToBoolArray()),"COM4",ProtocolType.SerialPort);
                IsOpen = false;
            }else{
                this.EGSendMessage(new ModbusRTU_WriteMultiCoil(1,0,OpenCode.ToBoolArray()),"COM4",ProtocolType.SerialPort);
                IsOpen = true;
            }
        }
        public void TestSingleHoldingRegister(){
            if(IsOpen){
                this.EGSendMessage(new ModbusRTU_WriteSingleHoldingRegister(1,0,0x00),"COM4",ProtocolType.SerialPort);
                IsOpen = false;
            }else{
                this.EGSendMessage(new ModbusRTU_WriteSingleHoldingRegister(1,0,0x01),"COM4",ProtocolType.SerialPort);
                IsOpen = true;
            }
        }
        public void TestMultiHoldingRegister(){
            ushort[] OpenCode = {1,1,1,1,1,1,1,1};
            ushort[] CloseCode = {0,0,0,0,0,0,0,0};
            if(IsOpen){
                this.EGSendMessage(new ModbusRTU_WriteMultiHoldingRegister(1,0,CloseCode),"COM4",ProtocolType.SerialPort);
                IsOpen = false;
            }else{
                this.EGSendMessage(new ModbusRTU_WriteMultiHoldingRegister(1,0,OpenCode),"COM4",ProtocolType.SerialPort);
                IsOpen = true;
            }
        }

    }
    public class TestBoxMessage {
        public int Code = 0;
        public float TestFloat = 0.1f;
        public double TestDouble;
        public bool IsHaveMessage;
        public string MessageInfo;
        public MessageType MsgType;
        public MessageStruct MsgStruct;
        public MessagePair MsgPair;
        public TestBoxMessage(){
            this.Code = 1;
            this.TestFloat = 1.2f;
            this.TestDouble = 2.52;
            this.IsHaveMessage = true;
            this.MessageInfo = "DefaultInfo";
            MsgType = MessageType.TypeInt;
            MsgPair = new MessagePair();
            MsgStruct = new MessageStruct(5,"Ad");
        }
        public string ToProtocolData()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
    public class MessagePair{
        public int MessageId;
        public string Author;
        public string MessageInfo;
        public int TimeStamp;
        public MessagePair(){
            this.MessageId = 10001;
            this.Author = "Admin";
            this.MessageInfo = "Hello every one!";
            this.TimeStamp = 1690188342;
        }
    }
    public struct MessageStruct : IRequest,IEGFramework{
        public string FunctionCode;
        public int MessageId;
        public string Author;
        public MessageStruct(int messageId,string author){
            FunctionCode = "Message";
            MessageId = messageId;
            Author = author;
        }
        public byte[] ToProtocolByteData()
        {
            return JsonConvert.SerializeObject(this).ToBytesByEncoding("GBK");
        }
        public string ToProtocolData()
        {
            return "";
            //return JsonConvert.SerializeObject(this);
        }
    }
    public struct MessageResponse : IResponse
    {
        public string FunctionCode { set; get; }
        public int MessageId;
        public string Author;
        public bool TrySetData(string protocolData, byte[] protocolBytes)
        {
            try
            {
                //GD.Print(protocolData);
                MessageResponse res = JsonConvert.DeserializeObject<MessageResponse>(protocolData);
                if(res.FunctionCode == "Message"){
                    this.FunctionCode = res.FunctionCode;
                    this.MessageId = res.MessageId;
                    this.Author = res.Author;
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    public struct PrintResponse : IResponse
    {
        public bool TrySetData(string protocolData, byte[] protocolBytes)
        {
            //Print("Received String is " + protocolData);
            Print("Received bytes is " + protocolBytes.ToStringByHex());
            return true;
        }

    }

    public enum MessageType{
        TypeString = 1,
        TypeInt = 2,
        TypeObject = 3,
        TypeArray = 4
    }
}