using System;
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

        public Queue<int> WaitForSendTCP = new Queue<int>();
        public int NextSendTCP = 0;
        public int SendPointerTCP = 1;

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

        #region  Modbus RTU Operations
        private bool IsRequestRTU { set; get; }
        public async Task<ModbusRTU_Response?> ReadRTUAsync(ModbusRegisterType registerType,string serialPort,byte deviceAddress,ushort start,ushort count){
            if(IsRequestRTU){
                SendPointerRTU++;
                int messageId = SendPointerRTU;
                WaitForSendRTU.Enqueue(messageId);
                await Task.Run(async () =>
                {
                    while (IsRequestRTU || NextSendRTU != messageId)
                    {
                        await Task.Delay(10);
                        //break;
                    }
                });
                GD.Print("-----Read"+messageId+" ----");
                //return null;
            }
            RTUCache.Clear();
            IsRequestRTU = true;
            IRequest ReadRequest;
            ModbusRTU_Response? res = null;
            switch(registerType){
                case ModbusRegisterType.Coil:
                    ReadRequest = new ModbusRTU_ReadCoils(deviceAddress,start,count);
                    this.EGSendMessage(ReadRequest,serialPort,ProtocolType.SerialPort);
                    this.EGSerialPort().SetExpectReceivedDataLength(6+count/8);
                    break;
                case ModbusRegisterType.DiscreteInput:
                    ReadRequest = new ModbusRTU_ReadDiscreteInput(deviceAddress,start,count);
                    this.EGSendMessage(ReadRequest,serialPort,ProtocolType.SerialPort);
                    this.EGSerialPort().SetExpectReceivedDataLength(6+count/8);
                    break;
                case ModbusRegisterType.HoldingRegister:
                    ReadRequest = new ModbusRTU_ReadHoldingRegisters(deviceAddress,start,count);
                    this.EGSendMessage(ReadRequest,serialPort,ProtocolType.SerialPort);
                    this.EGSerialPort().SetExpectReceivedDataLength(5+count*2);
                    break;
                case ModbusRegisterType.InputRegisters:
                    ReadRequest = new ModbusRTU_ReadInputRegisters(deviceAddress,start,count);
                    this.EGSendMessage(ReadRequest,serialPort,ProtocolType.SerialPort);
                    this.EGSerialPort().SetExpectReceivedDataLength(5+count*2);
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
                }
            });
            this.EGSerialPort().ClearReceivedCache(serialPort);
            IsRequestRTU = false;
            if(this.WaitForSendRTU.Count>0){
                NextSendRTU = this.WaitForSendRTU.Dequeue();
            }
            return res;
        }

        public async Task<ModbusRTU_Response?> WriteOnceRTUAsync(ModbusRegisterType registerType,string serialPort,byte deviceAddress,ushort registerAddress,object value){
            if(IsRequestRTU){
                SendPointerRTU++;
                int messageId = SendPointerRTU;
                WaitForSendRTU.Enqueue(messageId);
                await Task.Run(async () =>
                {
                    while (IsRequestRTU || NextSendRTU != messageId)
                    {
                        await Task.Delay(10);
                        //break;
                    }
                });
                GD.Print("-----Write"+messageId+" ----");
                //return null;
            }
            RTUCache.Clear();
            IsRequestRTU = true;
            IRequest WriteRequest;
            ModbusRTU_Response? res = null;
            switch(registerType){
                case ModbusRegisterType.Coil:
                    WriteRequest = new ModbusRTU_WriteSingleCoil(deviceAddress,registerAddress,(bool)value);
                    this.EGSendMessage(WriteRequest,serialPort,ProtocolType.SerialPort);
                    this.EGSerialPort().SetExpectReceivedDataLength(WriteRequest.ToProtocolByteData().Length);
                    break;
                case ModbusRegisterType.HoldingRegister:
                    WriteRequest = new ModbusRTU_WriteSingleHoldingRegister(deviceAddress,registerAddress,(ushort)value);
                    this.EGSendMessage(WriteRequest,serialPort,ProtocolType.SerialPort);
                    this.EGSerialPort().SetExpectReceivedDataLength(WriteRequest.ToProtocolByteData().Length);
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
                }
            });
            this.EGSerialPort().ClearReceivedCache(serialPort);
            IsRequestRTU = false;
            if(this.WaitForSendRTU.Count>0){
                NextSendRTU = this.WaitForSendRTU.Dequeue();
            }
            return res;
        }
        
        #endregion

        #region Modbus TCP Operations
        private bool IsRequestTCP { set; get; }
        public async Task<ModbusTCP_Response?> ReadTCPAsync(ModbusRegisterType registerType,string ipPort,byte deviceAddress,ushort start,ushort count){
            if(IsRequestTCP){
                SendPointerTCP++;
                int messageId = SendPointerTCP;
                WaitForSendTCP.Enqueue(messageId);
                await Task.Run(async () =>
                {
                    while (IsRequestTCP || NextSendTCP != messageId)
                    {
                        await Task.Delay(10);
                        //break;
                    }
                });
                GD.Print("-----Read"+messageId+" ----");
                //return null;
            }
            TCPCache.Clear();
            IsRequestTCP = true;
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
            IsRequestTCP = false;
            if(this.WaitForSendTCP.Count>0){
                NextSendTCP = this.WaitForSendTCP.Dequeue();
            }
            return res;
        }
        
        public async Task<ModbusTCP_Response?> WriteOnceTCPAsync(ModbusRegisterType registerType,string serialPort,byte deviceAddress,ushort registerAddress,object value){
            if(IsRequestTCP){
                SendPointerTCP++;
                int messageId = SendPointerTCP;
                WaitForSendTCP.Enqueue(messageId);
                await Task.Run(async () =>
                {
                    while (IsRequestTCP || NextSendTCP != messageId)
                    {
                        await Task.Delay(10);
                        //break;
                    }
                });
                GD.Print("-----Write"+messageId+" ----");
                //return null;
            }
            TCPCache.Clear();
            IsRequestTCP = true;
            IRequest WriteRequest;
            ModbusTCP_Response? res = null;
            switch(registerType){
                case ModbusRegisterType.Coil:
                    WriteRequest = new ModbusTCP_WriteSingleCoil(deviceAddress,registerAddress,(bool)value);
                    this.EGSendMessage(WriteRequest,serialPort,ProtocolType.TCPClient);
                    break;
                case ModbusRegisterType.HoldingRegister:
                    if(value.GetType() == typeof(float)){
                        ushort[] writeData = ((float)value).ToByteArrayBigEndian().ToUShortArray();
                        WriteRequest = new ModbusTCP_WriteMultiHoldingRegister(deviceAddress,registerAddress,writeData);
                    }else{
                        WriteRequest = new ModbusTCP_WriteSingleHoldingRegister(deviceAddress,registerAddress,(ushort)value);
                    }
                    this.EGSendMessage(WriteRequest,serialPort,ProtocolType.TCPClient);
                    break;
            }
            await Task.Run(async ()=>{
                int timeout = 0;
                while(TCPCache.Count==0 && timeout < Delay){
                    await Task.Delay(10);
                    timeout+=10;
                }
                if(TCPCache.Count>0){
                    res = TCPCache.Dequeue();
                }else{
                    //Print Error Timeout
                    OnReadTimeOut.Invoke();
                }
            });
            IsRequestTCP = false;
            if(this.WaitForSendTCP.Count>0){
                NextSendTCP = this.WaitForSendTCP.Dequeue();
            }
            if(this.WaitForSendTCP.Count>0){
                NextSendTCP = this.WaitForSendTCP.Dequeue();
            }
            return res;
        }
        #endregion
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