
using System.Collections.Generic;

namespace EGFramework.Examples.Gateway{
    public class DataModbusGatewaySetting{
        public float Delay { set; get; }
        public List<DataModbus485Device> Devices485 = new List<DataModbus485Device>();
        public List<DataModbusTCPDevice> DevicesTCP = new List<DataModbusTCPDevice>();
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
        public short Address { set; get; }
        public ModbusRegisterType RegisterType { set; get; } = ModbusRegisterType.HoldingRegister;
        public string Info { set; get; }
        public string Unit { set; get; }
    }
    

}
