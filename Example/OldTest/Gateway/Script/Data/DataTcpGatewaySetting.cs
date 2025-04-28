
using System.Collections.Generic;

namespace EGFramework.Examples.Gateway{
    public class DataTcpGatewaySetting{
        public List<DataTcpGatewayDevice> DataTcpGatewayDevices = new List<DataTcpGatewayDevice>();

    }
    public class DataTcpGatewayDevice{
        public string Host { set; get; }
        public int Port { set; get; }
        public string MqttHost { set; get; }
        public string RequestTheme { set; get; }
        public string ResponseTheme { set; get; }
    }
}
