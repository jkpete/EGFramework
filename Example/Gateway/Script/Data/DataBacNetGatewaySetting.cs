using System.Collections.Generic;
using System.IO.BACnet;

namespace EGFramework.Examples.Gateway
{
    public class DataBacnetGatewaySetting {
        public string MqttHost { set; get; }        
        public string HttpServerPrefix { set; get; }
        public string RequestTheme { set; get; }
        public string ResponseTheme { set; get; }
    }
    
}