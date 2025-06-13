using System.IO.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace EGFramework{
    public class EGSerialPort : IModule,IEGFramework,IProtocolSend,IProtocolReceived
    {
        public Dictionary<string,SerialPort> SerialPortDevices { set; get; } = new Dictionary<string, SerialPort>();
        public Dictionary<string,int> SerialPortMapping { set; get; } = new Dictionary<string, int>();

        public int DefaultBaudRate { set; get; } = 115200;

        // If data pack break,you can set this param to control the data pack completed at least.
        private int MinDataPackLength { set; get; } = 0;

        public string ErrorLogs { set; get; }

        public string ReceivedStr { set; get; } = "";

        public Encoding StringEncoding { set; get; } = Encoding.UTF8;

        public ConcurrentQueue<ResponseMsg> ResponseMsgs { set; get; } = new ConcurrentQueue<ResponseMsg>();

        public Dictionary<string,byte[]> ReceivedCache { set; get; } = new Dictionary<string,byte[]>();

        public void Init()
        {
            this.EGRegisterSendAction(request=>{
                if(request.protocolType == ProtocolType.SerialPort){
                    if(request.req.ToProtocolData() != "" && request.req.ToProtocolData() != null){
                        this.SendSerialStringData(request.sender,request.req.ToProtocolData());
                    }
                    if(request.req.ToProtocolByteData().Length > 0 && request.req.ToProtocolByteData() != null){
                        this.SendSerialByteData(request.sender,request.req.ToProtocolByteData());
                    }
                }
            });
        }
        
        public ConcurrentQueue<ResponseMsg> GetReceivedMsg()
        {
            return this.ResponseMsgs;
        }

        public void SetEncoding(Encoding encoding){
            this.StringEncoding = encoding;
        }

        public Encoding GetEncoding(){
            return StringEncoding;
        }

        public List<string> RefreshSerialPort(){
            string[] portNames = SerialPort.GetPortNames();
            SerialPortMapping.Clear();
            int index = 0;
            foreach (string portName in portNames)
            {
                SerialPortMapping.Add(portName,index);
                index++;
            }
            return SerialPortMapping.Keys.ToList();
        }

        public void SetBaudRate(int baudRate){
            this.DefaultBaudRate = baudRate;
        }

        /// <summary>
        /// Open serial port with check if target serial port isOpen.
        /// </summary>
        /// <param name="serialPort"></param>
        public bool Open(string serialPort){
            try{
                if(!SerialPortDevices.ContainsKey(serialPort)){
                    SerialPort newPort = new SerialPort(serialPort, DefaultBaudRate, Parity.None, 8, StopBits.One);
                    SerialPortDevices.Add(serialPort,newPort);
                    ReceivedCache.Add(serialPort,new byte[0]);
                    if (!SerialPortDevices[serialPort].IsOpen)
                    {
                        SerialPortDevices[serialPort].Open();
                        SerialPortDevices[serialPort].DataReceived += SerialPort_DataReceived;
                    }
                }else{
                    if(!SerialPortDevices[serialPort].IsOpen){
                        SerialPortDevices[serialPort].Open();
                    }
                }
                return true;
            }
            catch(Exception e){
                ErrorLogs = "[open port error]" + e.ToString();
                return false;
            }
        }

        /// <summary>
        /// Close serial port with check if target serial port isOpen.
        /// </summary>
        /// <param name="serialPort"></param>
        public void Close(string serialPort){
            if(SerialPortDevices.ContainsKey(serialPort)){
                if (SerialPortDevices[serialPort].IsOpen)
                {
                    SerialPortDevices[serialPort].Close();
                    SerialPortDevices.Remove(serialPort);
                }
            }else{
                //Not found in SerialPortDevices,need add?
            }
        }

        public bool SendSerialByteData(string serialPort,byte[] data){
            // if serial port not open,open first
            try{
                bool isSuccessPort = Open(serialPort);
                if(isSuccessPort){
                    SerialPortDevices[serialPort].Write(data, 0, data.Length);
                    return true;
                }else{
                    return false;
                }
            }catch(Exception e){
                ErrorLogs = "[write error]" + e.ToString();
                return false;
            }
        }
        public void SendByteData(string destination,byte[] data){
            SendSerialByteData(destination,data);
        }

        public bool SendByteDataOnce(string serialPort,byte[] data){
            bool isSuccessSend = SendSerialByteData(serialPort,data);
            if(isSuccessSend){
                SerialPortDevices[serialPort].Close();
                SerialPortDevices.Remove(serialPort);
            }
            return isSuccessSend;
        }

        public bool SendSerialStringData(string serialPort,string data){
            // if serial port not open,open first
            try{
                bool isSuccessPort = Open(serialPort);
                if(isSuccessPort){
                    byte[] encodingData = StringEncoding.GetBytes(data);
                    SerialPortDevices[serialPort].Write(encodingData, 0, encodingData.Length);
                    return true;
                }else{
                    return false;
                }
            }catch(Exception e){
                ErrorLogs = "[write error]" + e.ToString();
                return false;
            }
        }
        public void SendStringData(string destination,string data){
            SendSerialStringData(destination,data);
        }

        public bool SendStringDataOnce(string serialPort,string data){
            bool isSuccessSend = SendSerialStringData(serialPort,data);
            if(isSuccessSend){
                SerialPortDevices[serialPort].Close();
                SerialPortDevices.Remove(serialPort);
            }
            return isSuccessSend;
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //await Task.Run(() => {}).ConfigureAwait(false);
            SerialPort serialPort = (SerialPort)sender;
            if(serialPort.BytesToRead >= 0){
                int bufferSize = serialPort.BytesToRead;
                byte[] buffer = new byte[bufferSize];
                serialPort.Read(buffer,0,serialPort.BytesToRead);
                ReceivedCache[serialPort.PortName] = ReceivedCache[serialPort.PortName].Concat(buffer).ToArray();
            }
            if(ReceivedCache[serialPort.PortName].Length >= MinDataPackLength){
                string str = StringEncoding.GetString(ReceivedCache[serialPort.PortName]);
                EG.Print("[Receive]"+ReceivedCache[serialPort.PortName].ToStringByHex());
                ResponseMsgs.Enqueue(new ResponseMsg(str,ReceivedCache[serialPort.PortName],serialPort.PortName,ProtocolType.SerialPort));
                ReceivedCache[serialPort.PortName] = new byte[0];
                MinDataPackLength = 0;
                //this.EGOnReceivedData(new ResponseMsg(str,buffer,serialPort.PortName,ProtocolType.SerialPort));
            }else{
                EG.Print("[Data Get]" + ReceivedCache[serialPort.PortName].Length);
                string str = StringEncoding.GetString(ReceivedCache[serialPort.PortName]);
                ResponseMsgs.Enqueue(new ResponseMsg(str,ReceivedCache[serialPort.PortName],serialPort.PortName,ProtocolType.SerialPort));
            }
        }

        public void SetExpectReceivedDataLength(int leastLength){
            this.MinDataPackLength = leastLength;
        }

        public void ClearReceivedCache(string serialPort){
            ReceivedCache[serialPort] = new byte[0];
        }

        public IArchitecture GetArchitecture()
        {
            return EGArchitectureImplement.Interface;
        }
    }
    public static class CanGetEGSerialPortExtension{
        public static EGSerialPort EGSerialPort(this IEGFramework self){
            return self.GetModule<EGSerialPort>();
        }

        public static SerialPort EGGetSerialPort(this IEGFramework self,string serial){
            return self.GetModule<EGSerialPort>().SerialPortDevices[serial];
        }
    }
}
