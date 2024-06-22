using System;
using System.Linq;
using log4net.Core;

namespace EGFramework{
    /// <summary>
    /// some extensions function about Modbus-TCP and Modbus-RTU (Physic layer used RS-485)
    /// not include Modbus-ASCII,because LRC verify not developed
    /// </summary>
    public static class EGModbusExtension 
    {
        
        //Send Protocol
        //---------Modbus-TCP's Prefix---------
        //[00 00 00 00 00 06] 01 03 00 00 00 08
        //00 00 ----- info head (check for reply  Any things can be defined)
        //xx xx 00 00 00 06 info length ( Max length 65535 )
        public static byte[] MakeModbusTCPPrefix(this object self,ushort messageId,uint length){
            return messageId.ToBytes().Concat(length.ToBytes()).ToArray();
        }
        public static byte[] MakeModbusTCPPrefix(this object self,ushort messageId,byte[] sendData){
            return messageId.ToBytes().Concat(((uint)sendData.Length).ToBytes()).ToArray();
        }
    }

    /// <summary>
    /// Modbus FunctionCode 
    /// 0x01 => Read Coils ---- OK
    /// 0x02 => Read Discrete input ---- OK
    /// 0x03 => Read Holding registers ---- OK
    /// 0x04 => Read Input registers ---- OK
    /// 0x05 => Write Single Coils ---- OK
    /// 0x06 => Write Single Holding registers ---- OK
    /// 0x0F => Write Multi Coils ---- OK
    /// 0x10 => Write Multi Holding registers ---- OK
    /// </summary>
    
    #region Modbus TCP Request and Response 
    public struct ModbusTCP_ReadCoils : IRequest
    {
        public const byte FunctionCode = 0x01;
        public const ushort MessageId = 0xFF01;
        public byte DeviceAddress { set; get; } 
        public ushort RegisterAddress { set; get; } 
        public ushort ReadCount { set; get; } 

        /// <summary>
        /// Construct the protocol
        /// </summary>
        /// <param name="deviceAddress"></param>
        /// <param name="registerAddress"></param>
        /// <param name="readCount">Read count should be less than 2000</param>
        public ModbusTCP_ReadCoils(byte deviceAddress,ushort registerAddress,ushort readCount){
            DeviceAddress = deviceAddress;
            RegisterAddress = registerAddress;
            ReadCount = readCount;
        }
        public byte[] ToProtocolByteData()
        {
            byte[] protocolRequest = this.MakeModbusTCPPrefix(MessageId,6);
            protocolRequest = protocolRequest.Append(DeviceAddress).ToArray();
            protocolRequest = protocolRequest.Append(FunctionCode).ToArray();
            protocolRequest = protocolRequest.Concat(RegisterAddress.ToBytes()).ToArray();
            if(ReadCount>2000){
                ReadCount = 2000;
            }
            byte[] registerValues = ReadCount.ToBytes();
            protocolRequest = protocolRequest.Concat(registerValues).ToArray();
            return protocolRequest;
        }

        public string ToProtocolData()
        {
            return "";
        }
    }

    public struct ModbusTCP_ReadDiscreteInput : IRequest
    {
        public const byte FunctionCode = 0x02;
        public const ushort MessageId = 0xFF02;
        public byte DeviceAddress { set; get; } 
        public ushort RegisterAddress { set; get; } 
        public ushort ReadCount { set; get; } 

        /// <summary>
        /// Construct the protocol
        /// </summary>
        /// <param name="deviceAddress"></param>
        /// <param name="registerAddress"></param>
        /// <param name="readCount">Read count should be less than 2000</param>
        public ModbusTCP_ReadDiscreteInput(byte deviceAddress,ushort registerAddress,ushort readCount){
            DeviceAddress = deviceAddress;
            RegisterAddress = registerAddress;
            ReadCount = readCount;
        }
        public byte[] ToProtocolByteData()
        {
            byte[] protocolRequest = this.MakeModbusTCPPrefix(MessageId,6);
            protocolRequest = protocolRequest.Append(DeviceAddress).ToArray();
            protocolRequest = protocolRequest.Append(FunctionCode).ToArray();
            protocolRequest = protocolRequest.Concat(RegisterAddress.ToBytes()).ToArray();
            if(ReadCount>2000){
                ReadCount = 2000;
            }
            byte[] registerValues = ReadCount.ToBytes();
            protocolRequest = protocolRequest.Concat(registerValues).ToArray();
            return protocolRequest;
        }

        public string ToProtocolData()
        {
            return "";
        }
    }

    public struct ModbusTCP_ReadHoldingRegisters : IRequest
    {
        public const byte FunctionCode = 0x03;

        public const ushort MessageId = 0xFF03;
        public byte DeviceAddress { set; get; } 
        public ushort RegisterAddress { set; get; } 
        public ushort ReadCount { set; get; } 

        /// <summary>
        /// Construct the protocol
        /// </summary>
        /// <param name="deviceAddress"></param>
        /// <param name="registerAddress"></param>
        /// <param name="readCount">Read count should be less than 125</param>
        public ModbusTCP_ReadHoldingRegisters(byte deviceAddress,ushort registerAddress,ushort readCount){
            DeviceAddress = deviceAddress;
            RegisterAddress = registerAddress;
            ReadCount = readCount;
        }
        public byte[] ToProtocolByteData()
        {
            byte[] protocolRequest = this.MakeModbusTCPPrefix(MessageId,6);
            protocolRequest = protocolRequest.Append(DeviceAddress).ToArray();
            protocolRequest = protocolRequest.Append(FunctionCode).ToArray();
            protocolRequest = protocolRequest.Concat(RegisterAddress.ToBytes()).ToArray();
            if(ReadCount>125){
                ReadCount = 125;
            }
            byte[] registerValues = ReadCount.ToBytes();
            protocolRequest = protocolRequest.Concat(registerValues).ToArray();
            return protocolRequest;
        }

        public string ToProtocolData()
        {
            return "";
        }
    }

    public struct ModbusTCP_ReadInputRegisters : IRequest
    {
        public const byte FunctionCode = 0x04;

        public const ushort MessageId = 0xFF04;
        public byte DeviceAddress { set; get; } 
        public ushort RegisterAddress { set; get; } 
        public ushort ReadCount { set; get; } 

        /// <summary>
        /// Construct the protocol
        /// </summary>
        /// <param name="deviceAddress"></param>
        /// <param name="registerAddress"></param>
        /// <param name="readCount">Read count should be less than 125</param>
        public ModbusTCP_ReadInputRegisters(byte deviceAddress,ushort registerAddress,ushort readCount){
            DeviceAddress = deviceAddress;
            RegisterAddress = registerAddress;
            ReadCount = readCount;
        }
        public byte[] ToProtocolByteData()
        {
            byte[] protocolRequest = this.MakeModbusTCPPrefix(MessageId,6);
            protocolRequest = protocolRequest.Append(DeviceAddress).ToArray();
            protocolRequest = protocolRequest.Append(FunctionCode).ToArray();
            protocolRequest = protocolRequest.Concat(RegisterAddress.ToBytes()).ToArray();
            if(ReadCount>125){
                ReadCount = 125;
            }
            byte[] registerValues = ReadCount.ToBytes();
            protocolRequest = protocolRequest.Concat(registerValues).ToArray();
            return protocolRequest;
        }

        public string ToProtocolData()
        {
            return "";
        }
    }

    public struct ModbusTCP_WriteSingleCoil : IRequest
    {
        public const byte FunctionCode = 0x05;

        public const ushort MessageId = 0xFF05;
        public byte DeviceAddress { set; get; } 
        public ushort RegisterAddress { set; get; } 
        public bool Value { set; get; } 
        
        public ModbusTCP_WriteSingleCoil(byte deviceAddress,ushort registerAddress,bool value){
            DeviceAddress = deviceAddress;
            RegisterAddress = registerAddress;
            Value = value;
        }
        public byte[] ToProtocolByteData()
        {
            byte[] protocolRequest = this.MakeModbusTCPPrefix(MessageId,6);
            protocolRequest = protocolRequest.Append(DeviceAddress).ToArray();
            protocolRequest = protocolRequest.Append(FunctionCode).ToArray();
            protocolRequest = protocolRequest.Concat(RegisterAddress.ToBytes()).ToArray();
            byte[] registerValues = {0x00,0x00};
            if(Value){
                registerValues[0]=0xFF;
            }
            protocolRequest = protocolRequest.Concat(registerValues).ToArray();
            return protocolRequest;
        }

        public string ToProtocolData()
        {
            return "";
        }
    }

    public struct ModbusTCP_WriteSingleHoldingRegister : IRequest
    {
        public const byte FunctionCode = 0x06;

        public const ushort MessageId = 0xFF06;
        public byte DeviceAddress { set; get; } 
        public ushort RegisterAddress { set; get; } 
        public ushort Value { set; get; } 
        
        public ModbusTCP_WriteSingleHoldingRegister(byte deviceAddress,ushort registerAddress,ushort value){
            DeviceAddress = deviceAddress;
            RegisterAddress = registerAddress;
            Value = value;
        }
        public byte[] ToProtocolByteData()
        {
            byte[] protocolRequest = this.MakeModbusTCPPrefix(MessageId,6);
            protocolRequest = protocolRequest.Append(DeviceAddress).ToArray();
            protocolRequest = protocolRequest.Append(FunctionCode).ToArray();
            protocolRequest = protocolRequest.Concat(RegisterAddress.ToBytes()).ToArray();
            byte[] registerValues = Value.ToBytes();
            protocolRequest = protocolRequest.Concat(registerValues).ToArray();
            return protocolRequest;
        }

        public string ToProtocolData()
        {
            return "";
        }
    }

    public struct ModbusTCP_WriteMultiCoil : IRequest{
        public const byte FunctionCode = 0x0F;

        public const ushort MessageId = 0xFF0F;
        public byte DeviceAddress { set; get; } 
        public ushort RegisterStartAddress { set; get; } 
        public bool[] Values { set; get; } 
        
        /// <summary>
        /// Construct the protocol
        /// </summary>
        /// <param name="deviceAddress">usual use 0x01 for device address</param>
        /// <param name="registerStartAddress"></param>
        /// <param name="values">Values length should be less than 2000</param>
        public ModbusTCP_WriteMultiCoil(byte deviceAddress,ushort registerStartAddress,bool[] values){
            DeviceAddress = deviceAddress;
            RegisterStartAddress = registerStartAddress;
            Values = values;
        }
        public byte[] ToProtocolByteData()
        {
            
            byte[] protocolRequest = {DeviceAddress,FunctionCode};
            protocolRequest = protocolRequest.Concat(RegisterStartAddress.ToBytes()).ToArray();
            protocolRequest = protocolRequest.Concat(((ushort)Values.Length).ToBytes()).ToArray();
            
            //Length range should be 1-2000 0x0001-0x07D0,otherwise delete the data after 2000
            if(Values.Length>2000){
                bool[] SourceValues = Values;
                Values = new bool[2000];
                Array.Copy(Values,0,SourceValues,0,2000);
            }
            //bool array 2000 => byte array 250
            byte[] valueGroup = Values.ToByteArray();
            byte valueLength = (byte)valueGroup.Length;
            protocolRequest = protocolRequest.Append(valueLength).ToArray();
            protocolRequest = protocolRequest.Concat(valueGroup).ToArray();
            byte[] protocolPrefix = this.MakeModbusTCPPrefix(MessageId,protocolRequest);
            protocolRequest = protocolPrefix.Concat(protocolRequest).ToArray();
            return protocolRequest;
        }

        public string ToProtocolData()
        {
            return "";
        }
    }
    
    /// <summary>
    /// Start write at Register start address,such as 0x03,write order by order like 0x03,0x04,0x05...,write count is the value array length
    /// </summary>
    public struct ModbusTCP_WriteMultiHoldingRegister : IRequest
    {
        public const byte FunctionCode = 0x10;

        public const ushort MessageId = 0xFF10;
        public byte DeviceAddress { set; get; } 
        public ushort RegisterStartAddress { set; get; } 
        public ushort[] Values { set; get; } 
        
        /// <summary>
        /// Construct the protocol
        /// </summary>
        /// <param name="deviceAddress">usual use 0x01 for device address</param>
        /// <param name="registerStartAddress"></param>
        /// <param name="values">Values length should be less than 125</param>
        public ModbusTCP_WriteMultiHoldingRegister(byte deviceAddress,ushort registerStartAddress,ushort[] values){
            DeviceAddress = deviceAddress;
            RegisterStartAddress = registerStartAddress;
            Values = values;
        }
        public byte[] ToProtocolByteData()
        {
            
            byte[] protocolRequest = {DeviceAddress,FunctionCode};
            protocolRequest = protocolRequest.Concat(RegisterStartAddress.ToBytes()).ToArray();
            protocolRequest = protocolRequest.Concat(((ushort)Values.Length).ToBytes()).ToArray();

            //Length range should be 1-125 0x0001-0x07D,otherwise delete the data after 125
            if(Values.Length>125){
                ushort[] SourceValues = Values;
                Values = new ushort[125];
                Array.Copy(Values,0,SourceValues,0,125);
            }
            //ushort array 125 => byte array 250
            byte[] valueGroup = {};
            foreach(ushort value in Values){
                byte[] registerValues = value.ToBytes();
                valueGroup = valueGroup.Concat(registerValues).ToArray();
            }
            byte valueLength = (byte)valueGroup.Length;
            protocolRequest = protocolRequest.Append(valueLength).ToArray();
            protocolRequest = protocolRequest.Concat(valueGroup).ToArray();
            byte[] protocolPrefix = this.MakeModbusTCPPrefix(MessageId,protocolRequest);
            protocolRequest = protocolPrefix.Concat(protocolRequest).ToArray();
            return protocolRequest;
        }

        public string ToProtocolData()
        {
            return "";
        }
    }

    public struct ModbusTCP_Response : IResponse
    {
        public bool[] Coil { set; get; }
        public bool[] DiscreteInput { set; get; }
        public ushort[] HoldingRegister { set; get; }
        public ushort[] InputRegister { set; get; }
        public byte FunctionCode { set; get; }
        public byte DeviceAddress { set; get; }
        public ushort RegisterStartAddress { set; get; }
        public uint DataLength { set; get; }
        
        public byte[] SourceData { set; get; }
        public ModbusFunctionType FunctionType { set; get; }
        public ModbusErrorCode ErrorCode { set; get; }
        public bool IsError { set; get; }
        public bool TrySetData(string protocolData, byte[] protocolBytes)
        {
            try
            {
                if(protocolBytes==null && protocolBytes.Length < 8){
                    return false;
                }
                SourceData = protocolBytes;
                DeviceAddress = protocolBytes[6];
                FunctionType = (ModbusFunctionType)protocolBytes[7];

                if(FunctionCode == 0x83){
                    ErrorCode = (ModbusErrorCode)protocolBytes[2];
                    IsError = true;
                    return true;
                }
                byte[] dataLength = new byte[4];
                Array.Copy(protocolBytes,2,dataLength,0,4);
                DataLength = dataLength.ToUINT();
                if(protocolBytes.Length != DataLength+6){
                    return false;
                }
                //every response's start should be 0xff,because the request's start is 0xff
                if(protocolBytes[0]==0xff){
                    switch(FunctionType){
                        case ModbusFunctionType.ReadCoil:
                            byte readCoilLength = protocolBytes[8];
                            byte[] CoilBytes = new byte[readCoilLength];
                            Array.Copy(protocolBytes,9,CoilBytes,0,readCoilLength);
                            Coil = CoilBytes.ToBoolArray();
                            return true;
                        case ModbusFunctionType.ReadDiscreteInput:
                            byte readDiscreteInputLength = protocolBytes[8];
                            byte[] DiscreteInputBytes = new byte[readDiscreteInputLength];
                            Array.Copy(protocolBytes,9,DiscreteInputBytes,0,readDiscreteInputLength);
                            DiscreteInput = DiscreteInputBytes.ToBoolArray();
                            return true;
                        case ModbusFunctionType.ReadHoldingRegisters:
                            byte readHoldingRegistersLength = protocolBytes[8];
                            byte[] HoldingRegistersBytes = new byte[readHoldingRegistersLength];
                            Array.Copy(protocolBytes,9,HoldingRegistersBytes,0,readHoldingRegistersLength);
                            HoldingRegister = HoldingRegistersBytes.ToUShortArray();
                            return true;
                        case ModbusFunctionType.ReadInputRegisters:
                            byte readInputRegistersLength = protocolBytes[8];
                            byte[] InputRegistersBytes = new byte[readInputRegistersLength];
                            Array.Copy(protocolBytes,9,InputRegistersBytes,0,readInputRegistersLength);
                            InputRegister = InputRegistersBytes.ToUShortArray();
                            return true;
                        case ModbusFunctionType.WriteSingleCoil:
                            return true;
                        case ModbusFunctionType.WriteSingleHoldingRegister:
                            return true;
                        case ModbusFunctionType.WriteMultiCoil:
                            return true;
                        case ModbusFunctionType.WriteMultiHoldingRegister:
                            return true;
                        default:
                            return false;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return false;
        }
    }
    #endregion

    #region Modbus RTU Request and Response,Used RS-485 for Physic layer
    public struct ModbusRTU_ReadCoils : IRequest{
        public const byte FunctionCode = 0x01;
        public byte DeviceAddress { set; get; } 
        public ushort RegisterAddress { set; get; } 
        public ushort ReadCount { set; get; } 

        /// <summary>
        /// Construct the protocol
        /// </summary>
        /// <param name="deviceAddress"></param>
        /// <param name="registerAddress"></param>
        /// <param name="readCount">Read count should be less than 2000</param>
        public ModbusRTU_ReadCoils(byte deviceAddress,ushort registerAddress,ushort readCount){
            DeviceAddress = deviceAddress;
            RegisterAddress = registerAddress;
            ReadCount = readCount;
        }
        public byte[] ToProtocolByteData()
        {
            byte[] protocolRequest = { DeviceAddress , FunctionCode };
            protocolRequest = protocolRequest.Concat(RegisterAddress.ToBytes()).ToArray();
            if(ReadCount>2000){
                ReadCount = 2000;
            }
            byte[] registerValues = ReadCount.ToBytes();
            protocolRequest = protocolRequest.Concat(registerValues).ToArray();
            protocolRequest = protocolRequest.Concat(protocolRequest.CalculateCRC16Modbus().ToBytes()).ToArray();
            return protocolRequest;
        }

        public string ToProtocolData()
        {
            return "";
        }
    }

    public struct ModbusRTU_ReadDiscreteInput : IRequest{
        public const byte FunctionCode = 0x02;
        public byte DeviceAddress { set; get; } 
        public ushort RegisterAddress { set; get; } 
        public ushort ReadCount { set; get; } 

        /// <summary>
        /// Construct the protocol
        /// </summary>
        /// <param name="deviceAddress"></param>
        /// <param name="registerAddress"></param>
        /// <param name="readCount">Read count should be less than 2000</param>
        public ModbusRTU_ReadDiscreteInput(byte deviceAddress,ushort registerAddress,ushort readCount){
            DeviceAddress = deviceAddress;
            RegisterAddress = registerAddress;
            ReadCount = readCount;
        }
        public byte[] ToProtocolByteData()
        {
            byte[] protocolRequest = { DeviceAddress , FunctionCode };
            protocolRequest = protocolRequest.Concat(RegisterAddress.ToBytes()).ToArray();
            if(ReadCount>2000){
                ReadCount = 2000;
            }
            byte[] registerValues = ReadCount.ToBytes();
            protocolRequest = protocolRequest.Concat(registerValues).ToArray();
            protocolRequest = protocolRequest.Concat(protocolRequest.CalculateCRC16Modbus().ToBytes()).ToArray();
            return protocolRequest;
        }

        public string ToProtocolData()
        {
            return "";
        }
    }

    public struct ModbusRTU_ReadHoldingRegisters : IRequest{
        public const byte FunctionCode = 0x03;
        public byte DeviceAddress { set; get; } 
        public ushort RegisterAddress { set; get; } 
        public ushort ReadCount { set; get; } 

        /// <summary>
        /// Construct the protocol
        /// </summary>
        /// <param name="deviceAddress"></param>
        /// <param name="registerAddress"></param>
        /// <param name="readCount">Read count should be less than 2000</param>
        public ModbusRTU_ReadHoldingRegisters(byte deviceAddress,ushort registerAddress,ushort readCount){
            DeviceAddress = deviceAddress;
            RegisterAddress = registerAddress;
            ReadCount = readCount;
        }
        public byte[] ToProtocolByteData()
        {
            byte[] protocolRequest = { DeviceAddress , FunctionCode };
            protocolRequest = protocolRequest.Concat(RegisterAddress.ToBytes()).ToArray();
            if(ReadCount>125){
                ReadCount = 125;
            }
            byte[] registerValues = ReadCount.ToBytes();
            protocolRequest = protocolRequest.Concat(registerValues).ToArray();
            protocolRequest = protocolRequest.Concat(protocolRequest.CalculateCRC16Modbus().ToBytes()).ToArray();
            return protocolRequest;
        }

        public string ToProtocolData()
        {
            return "";
        }
    }

    public struct ModbusRTU_ReadInputRegisters : IRequest{
        public const byte FunctionCode = 0x04;
        public byte DeviceAddress { set; get; } 
        public ushort RegisterAddress { set; get; } 
        public ushort ReadCount { set; get; } 

        /// <summary>
        /// Construct the protocol
        /// </summary>
        /// <param name="deviceAddress"></param>
        /// <param name="registerAddress"></param>
        /// <param name="readCount">Read count should be less than 2000</param>
        public ModbusRTU_ReadInputRegisters(byte deviceAddress,ushort registerAddress,ushort readCount){
            DeviceAddress = deviceAddress;
            RegisterAddress = registerAddress;
            ReadCount = readCount;
        }
        public byte[] ToProtocolByteData()
        {
            byte[] protocolRequest = { DeviceAddress , FunctionCode };
            protocolRequest = protocolRequest.Concat(RegisterAddress.ToBytes()).ToArray();
            if(ReadCount>125){
                ReadCount = 125;
            }
            byte[] registerValues = ReadCount.ToBytes();
            protocolRequest = protocolRequest.Concat(registerValues).ToArray();
            protocolRequest = protocolRequest.Concat(protocolRequest.CalculateCRC16Modbus().ToBytes()).ToArray();
            return protocolRequest;
        }

        public string ToProtocolData()
        {
            return "";
        }
    }

    public struct ModbusRTU_WriteSingleCoil : IRequest{
        public const byte FunctionCode = 0x05;
        public byte DeviceAddress { set; get; } 
        public ushort RegisterAddress { set; get; } 
        public bool Value { set; get; } 
        
        public ModbusRTU_WriteSingleCoil(byte deviceAddress,ushort registerAddress,bool value){
            DeviceAddress = deviceAddress;
            RegisterAddress = registerAddress;
            Value = value;
        }
        public byte[] ToProtocolByteData()
        {
            byte[] protocolRequest = { DeviceAddress , FunctionCode };
            protocolRequest = protocolRequest.Concat(RegisterAddress.ToBytes()).ToArray();
            byte[] registerValues = {0x00,0x00};
            if(Value){
                registerValues[0]=0xFF;
            }
            protocolRequest = protocolRequest.Concat(registerValues).ToArray();
            protocolRequest = protocolRequest.Concat(protocolRequest.CalculateCRC16Modbus().ToBytes()).ToArray();
            return protocolRequest;
        }

        public string ToProtocolData()
        {
            return "";
        }
    }
    
    public struct ModbusRTU_WriteSingleHoldingRegister : IRequest{
        public const byte FunctionCode = 0x06;
        public byte DeviceAddress { set; get; } 
        public ushort RegisterAddress { set; get; } 
        public ushort Value { set; get; } 
        
        public ModbusRTU_WriteSingleHoldingRegister(byte deviceAddress,ushort registerAddress,ushort value){
            DeviceAddress = deviceAddress;
            RegisterAddress = registerAddress;
            Value = value;
        }
        public byte[] ToProtocolByteData()
        {
            byte[] protocolRequest = { DeviceAddress , FunctionCode };
            protocolRequest = protocolRequest.Concat(RegisterAddress.ToBytes()).ToArray();
            byte[] registerValues = Value.ToBytes();
            protocolRequest = protocolRequest.Concat(registerValues).ToArray();
            protocolRequest = protocolRequest.Concat(protocolRequest.CalculateCRC16Modbus().ToBytes()).ToArray();
            return protocolRequest;
        }

        public string ToProtocolData()
        {
            return "";
        }
    }
    
    public struct ModbusRTU_WriteMultiCoil : IRequest{
        public const byte FunctionCode = 0x0F;
        public byte DeviceAddress { set; get; } 
        public ushort RegisterStartAddress { set; get; } 
        public bool[] Values { set; get; } 
        
        /// <summary>
        /// Construct the protocol
        /// </summary>
        /// <param name="deviceAddress">usual use 0x01 for device address</param>
        /// <param name="registerStartAddress"></param>
        /// <param name="values">Values length should be less than 2000</param>
        public ModbusRTU_WriteMultiCoil(byte deviceAddress,ushort registerStartAddress,bool[] values){
            DeviceAddress = deviceAddress;
            RegisterStartAddress = registerStartAddress;
            Values = values;
        }
        public byte[] ToProtocolByteData()
        {
            
            byte[] protocolRequest = {DeviceAddress,FunctionCode};
            protocolRequest = protocolRequest.Concat(RegisterStartAddress.ToBytes()).ToArray();
            protocolRequest = protocolRequest.Concat(((ushort)Values.Length).ToBytes()).ToArray();
            
            //Length range should be 1-2000 0x0001-0x07D0,otherwise delete the data after 2000
            if(Values.Length>2000){
                bool[] SourceValues = Values;
                Values = new bool[2000];
                Array.Copy(Values,0,SourceValues,0,2000);
            }
            //bool array 2000 => byte array 250
            byte[] valueGroup = Values.ToByteArray();
            byte valueLength = (byte)valueGroup.Length;
            protocolRequest = protocolRequest.Append(valueLength).ToArray();
            protocolRequest = protocolRequest.Concat(valueGroup).ToArray();
            protocolRequest = protocolRequest.Concat(protocolRequest.CalculateCRC16Modbus().ToBytes()).ToArray();
            return protocolRequest;
        }

        public string ToProtocolData()
        {
            return "";
        }
    }
    
    /// <summary>
    /// Start write at Register start address,such as 0x03,write order by order like 0x03,0x04,0x05...,write count is the value array length
    /// </summary>
    public struct ModbusRTU_WriteMultiHoldingRegister : IRequest
    {
        public const byte FunctionCode = 0x10;

        public const ushort MessageId = 0xFF10;
        public byte DeviceAddress { set; get; } 
        public ushort RegisterStartAddress { set; get; } 
        public ushort[] Values { set; get; } 
        
        /// <summary>
        /// Construct the protocol
        /// </summary>
        /// <param name="deviceAddress">usual use 0x01 for device address</param>
        /// <param name="registerStartAddress"></param>
        /// <param name="values">Values length should be less than 125</param>
        public ModbusRTU_WriteMultiHoldingRegister(byte deviceAddress,ushort registerStartAddress,ushort[] values){
            DeviceAddress = deviceAddress;
            RegisterStartAddress = registerStartAddress;
            Values = values;
        }
        public byte[] ToProtocolByteData()
        {
            
            byte[] protocolRequest = {DeviceAddress,FunctionCode};
            protocolRequest = protocolRequest.Concat(RegisterStartAddress.ToBytes()).ToArray();
            protocolRequest = protocolRequest.Concat(((ushort)Values.Length).ToBytes()).ToArray();

            //Length range should be 1-125 0x0001-0x07D,otherwise delete the data after 125
            if(Values.Length>125){
                ushort[] SourceValues = Values;
                Values = new ushort[125];
                Array.Copy(Values,0,SourceValues,0,125);
            }
            //ushort array 125 => byte array 250
            byte[] valueGroup = {};
            foreach(ushort value in Values){
                byte[] registerValues = value.ToBytes();
                valueGroup = valueGroup.Concat(registerValues).ToArray();
            }
            byte valueLength = (byte)valueGroup.Length;
            protocolRequest = protocolRequest.Append(valueLength).ToArray();
            protocolRequest = protocolRequest.Concat(valueGroup).ToArray();
            protocolRequest = protocolRequest.Concat(protocolRequest.CalculateCRC16Modbus().ToBytes()).ToArray();
            return protocolRequest;
        }

        public string ToProtocolData()
        {
            return "";
        }
    }
    
    public struct ModbusRTU_Response : IResponse
    {
        public bool[] Coil { set; get; }
        public bool[] DiscreteInput { set; get; }
        public ushort[] HoldingRegister { set; get; }
        public ushort[] InputRegister { set; get; }
        public byte FunctionCode { set; get; }
        public byte DeviceAddress { set; get; }
        public ushort RegisterStartAddress { set; get; }
        
        public byte[] SourceData { set; get; }
        public ModbusFunctionType FunctionType { set; get; }
        public ModbusErrorCode ErrorCode { set; get; }
        public bool IsError { set; get; }

        public bool TrySetData(string protocolData, byte[] protocolBytes)
        {
            try
            {
                if(protocolBytes==null && protocolBytes.Length < 4){
                    return false;
                }
                SourceData = protocolBytes;
                DeviceAddress = protocolBytes[0];
                FunctionCode = protocolBytes[1];
                FunctionType = (ModbusFunctionType)protocolBytes[1];
                if(FunctionCode == 0x83){
                    ErrorCode = (ModbusErrorCode)protocolBytes[2];
                    IsError = true;
                    return true;
                }
                
                //check crc verify is success
                byte[] resultArray = new byte[protocolBytes.Length - 2];
                Array.Copy(protocolBytes, 0, resultArray, 0, protocolBytes.Length - 2);
                byte[] crcArray = new byte[2];
                Array.Copy(protocolBytes, protocolBytes.Length - 2, crcArray, 0, 2);
                if(resultArray.CalculateCRC16Modbus()!=crcArray.ToUShort()){
                    return false;
                }

                //every response's start should be 0xff,because the request's start is 0xff
                switch(FunctionType){
                    case ModbusFunctionType.ReadCoil:
                        byte readCoilLength = protocolBytes[2];
                        byte[] CoilBytes = new byte[readCoilLength];
                        Array.Copy(protocolBytes,3,CoilBytes,0,readCoilLength);
                        Coil = CoilBytes.ToBoolArray();
                        return true;
                    case ModbusFunctionType.ReadDiscreteInput:
                        byte readDiscreteInputLength = protocolBytes[2];
                        byte[] DiscreteInputBytes = new byte[readDiscreteInputLength];
                        Array.Copy(protocolBytes,3,DiscreteInputBytes,0,readDiscreteInputLength);
                        DiscreteInput = DiscreteInputBytes.ToBoolArray();
                        return true;
                    case ModbusFunctionType.ReadHoldingRegisters:
                        byte readHoldingRegistersLength = protocolBytes[2];
                        byte[] HoldingRegistersBytes = new byte[readHoldingRegistersLength];
                        Array.Copy(protocolBytes,3,HoldingRegistersBytes,0,readHoldingRegistersLength);
                        HoldingRegister = HoldingRegistersBytes.ToUShortArray();
                        return true;
                    case ModbusFunctionType.ReadInputRegisters:
                        byte readInputRegistersLength = protocolBytes[2];
                        byte[] InputRegistersBytes = new byte[readInputRegistersLength];
                        Array.Copy(protocolBytes,3,InputRegistersBytes,0,readInputRegistersLength);
                        InputRegister = InputRegistersBytes.ToUShortArray();
                        return true;
                    case ModbusFunctionType.WriteSingleCoil:
                        return true;
                    case ModbusFunctionType.WriteSingleHoldingRegister:
                        return true;
                    case ModbusFunctionType.WriteMultiCoil:
                        return true;
                    case ModbusFunctionType.WriteMultiHoldingRegister:
                        return true;
                    default:
                        return false;
                }
                
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
    #endregion

    public enum ModbusFunctionType{
        None = 0x00,
        ReadCoil = 0x01,
        ReadDiscreteInput = 0x02,
        ReadHoldingRegisters = 0x03,
        ReadInputRegisters = 0x04,
        WriteSingleCoil = 0x05,
        WriteSingleHoldingRegister = 0x06,
        WriteMultiCoil = 0x0f,
        WriteMultiHoldingRegister = 0x10
    }

    public enum ModbusRegisterType{
        None = 0x00,
        Coil = 0x01,
        DiscreteInput = 0x02,
        HoldingRegister = 0x03,
        InputRegisters = 0x04
    }

    public enum ModbusErrorCode{
        NoError = 0x00,
        IllegalFunction = 0x01,
        IllegalDataAddress = 0x02,
        IllegalDataValue = 0x03,
        SlaveDeviceFailure = 0x04,
        Acknowledge = 0x05,
        SlaveDeviceBusy = 0x06,
        MemoryParityError = 0x08,
        GatewayPathUnavailable = 0x0A,
        GatewayTargetDeviceFailedToRespond = 0x0B
    }
}
