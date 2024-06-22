using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

namespace EGFramework{
    /// <summary>
    /// This class is not a simple protocol tools,only support sample functions to easy use.
    /// </summary>
    public class EGModbus : IEGFramework, IModule
    {
        public Queue<ModbusRTU_Response> RTUCache = new Queue<ModbusRTU_Response>();
        public Queue<ModbusTCP_Response> TCPCache = new Queue<ModbusTCP_Response>();

        public int Delay = 2000;

        public Queue<int> WaitForSendRTU = new Queue<int>();
        public int NextSendRTU = 0;
        public int SendPointerRTU = 1;

        public EasyEvent OnReadTimeOut = new EasyEvent();

        public void Init()
        {
            this.EGRegisterMessageEvent<ModbusRTU_Response>((e,sender,ProtocolType)=>{
                if(ProtocolType == ProtocolType.SerialPort){
                    RTUCache.Enqueue(e);
                }
            });
            this.EGRegisterMessageEvent<ModbusTCP_Response>((e,sender,ProtocolType)=>{
                if(ProtocolType == ProtocolType.TCPClient){
                    TCPCache.Enqueue(e);
                }
            });
            this.EGOnMessage<ModbusRTU_Response>();
            this.EGOnMessage<ModbusTCP_Response>();
        }

        public bool IsReadingRTU { set; get; }
        public async Task<ModbusRTU_Response?> ReadRTUAsync(ModbusRegisterType registerType,string serialPort,byte deviceAddress,ushort start,ushort count){
            if(IsReadingRTU){
                SendPointerRTU++;
                int messageId = SendPointerRTU;
                WaitForSendRTU.Enqueue(messageId);
                await Task.Run(async () =>
                {
                    while (IsReadingRTU || NextSendRTU != messageId)
                    {
                        await Task.Delay(10);
                        //break;
                    }
                });
                GD.Print("-----Read"+messageId+" ----");
                //return null;
            }
            RTUCache.Clear();
            IsReadingRTU = true;
            IRequest ReadRequest;
            ModbusRTU_Response? res = null;
            switch(registerType){
                case ModbusRegisterType.HoldingRegister:
                    ReadRequest = new ModbusRTU_ReadHoldingRegisters(deviceAddress,start,count);
                    // this.AppendMessage("【发送-"+DataModbusItem.SerialPort+"】 "+ReadRequest.ToProtocolByteData().ToStringByHex());
                    this.EGSendMessage(ReadRequest,serialPort,ProtocolType.SerialPort);
                    // this.EGSerialPort().SetExpectReceivedDataLength(5+count*2);
                    this.EGSerialPort().SetExpectReceivedDataLength(5);
                    break;
            }
            await Task.Run(async ()=>{
                int timeout = 0;
                while(RTUCache.Count==0 && timeout < Delay){
                    await Task.Delay(10);
                    timeout+=10;
                }
                if(RTUCache.Count>0){
                    res = RTUCache.Dequeue();
                }else{
                    //Print Error Timeout
                    OnReadTimeOut.Invoke();
                    this.EGSerialPort().ClearBuffer(serialPort);
                }
            });
            IsReadingRTU = false;
            if(this.WaitForSendRTU.Count>0){
                NextSendRTU = this.WaitForSendRTU.Dequeue();
            }
            return res;
        }

        public bool IsReadingTCP { set; get; }
        public async Task<ModbusTCP_Response?> ReadTCPAsync(ModbusRegisterType registerType,string ipPort,byte deviceAddress,ushort start,ushort count){
            if(IsReadingTCP){
                await Task.Run(async () =>
                {
                    while (IsReadingTCP)
                    {
                        await Task.Delay(10);
                        //break;
                    }
                });
                //return null;
            }
            TCPCache.Clear();
            IsReadingTCP = true;
            IRequest ReadRequest;
            ModbusTCP_Response? res = null;
            switch(registerType){
                case ModbusRegisterType.HoldingRegister:
                    ReadRequest = new ModbusTCP_ReadHoldingRegisters(deviceAddress,start,count);
                    // this.AppendMessage("【发送-"+DataModbusItem.SerialPort+"】 "+ReadRequest.ToProtocolByteData().ToStringByHex());
                    this.EGSendMessage(ReadRequest,ipPort,ProtocolType.TCPClient);
                    break;
            }
            await Task.Run(async ()=>{
                int timeout = 0;
                while(TCPCache.Count == 0 && timeout < Delay){
                    await Task.Delay(10);
                    timeout += 10;
                }
                if(TCPCache.Count>0){
                    res = TCPCache.Dequeue();
                }else{
                    //Print Error Timeout
                    OnReadTimeOut.Invoke();
                }
            });
            IsReadingTCP = false;
            return res;
        }

        public IArchitecture GetArchitecture()
        {
            return EGArchitectureImplement.Interface;
        }
    }
    public static class CanGetEGModbusExtension{
        public static EGModbus EGModbus(this IEGFramework self){
            return self.GetModule<EGModbus>();
        }
    }
}