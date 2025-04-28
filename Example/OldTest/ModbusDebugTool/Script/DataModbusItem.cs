using System;
using System.Collections.Generic;

namespace EGFramework.Examples.ModbusDebugTool{
    public class DataModbus{
        /// <summary>
        /// Key is the ModbusRTU Type + "-" + DeviceAddress + "-" + RegisterAddress + "-" + Port
        /// </summary>
        /// <typeparam name="string">Key is the ModbusRTU DeviceAddress + "-" + Type + "-" + RegisterAddress + "-" + Port</typeparam>
        /// <typeparam name="DataModbusItem"></typeparam>
        /// <returns></returns>
        public Dictionary<string,DataModbusItem> Items { set; get; } = new Dictionary<string,DataModbusItem>();
    }
    public class DataModbusItem 
    {
        public ModbusRegisterType Type { set; get; } = ModbusRegisterType.HoldingRegister;

        public byte DeviceAddress { set; get; }

        public ushort RegisterAddress { set; get; }

        public string SerialPort { set; get; }

        public string GetKey(){
            return Type + "-" + DeviceAddress + "-" + RegisterAddress + "-" + SerialPort;
        }
    }

    public class DataModbusSettings{
        public int BaudRate{ set; get; } = 115200;
    }
}
