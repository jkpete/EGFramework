
using System.Collections.Generic;

namespace EGFramework.Examples.Gateway{
    public class DataModbusGatewaySetting{
        public float Delay { set; get; }
        public Dictionary<string,DataModbus485Device> Devices485 = new Dictionary<string,DataModbus485Device>();
        public Dictionary<string,DataModbusTCPDevice> DevicesTCP = new Dictionary<string,DataModbusTCPDevice>();
    }
    public class DataModbus485Device{
        public string SerialPort { set; get; }
        public byte Address { set; get; }
        public int BaudRate { set; get; }
        public Dictionary<string,DataModbusValue> ValueRegisters = new Dictionary<string, DataModbusValue>();
    }

    public class DataModbusTCPDevice{
        public string Host { set; get; }
        public int Port { set; get; }
        public byte Address { set; get; }
        public Dictionary<string,DataModbusValue> ValueRegisters = new Dictionary<string, DataModbusValue>();
    }
    public class DataModbusValue{
        public ushort Address { set; get; }
        public ushort Length { set; get; }
        public DataModbusValueType ValueType = DataModbusValueType.Float_;
        public ModbusRegisterType RegisterType { set; get; } = ModbusRegisterType.HoldingRegister;
        public string Name { set; get; }
        // public string Unit { set; get; }
    }

    public enum DataModbusValueType{
        UShort_ = 0,
        Float_ = 1
    }
    

}
