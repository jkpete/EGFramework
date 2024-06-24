
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
        public Dictionary<string,DataModbusRegister> Registers = new Dictionary<string, DataModbusRegister>();
    }

    public class DataModbusTCPDevice{
        public string Host { set; get; }
        public int Port { set; get; }
        public byte Address { set; get; }
        public Dictionary<string,DataModbusRegister> Registers = new Dictionary<string, DataModbusRegister>();
    }
    public class DataModbusRegister{
        public ushort Address { set; get; }
        public ModbusRegisterType RegisterType { set; get; } = ModbusRegisterType.HoldingRegister;
        public string Name { set; get; }
        // public string Unit { set; get; }
    }
    

}
