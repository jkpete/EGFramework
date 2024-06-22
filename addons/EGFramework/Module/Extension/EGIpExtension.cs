
namespace EGFramework {
    public static class EGIpExtension
    {
        /// <summary>
        /// Get host from IP. Such as 192.168.0.1:5555 => get 192.168.0.1
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static string GetHostByIp(this string ip)
        {
            int colonIndex = ip.IndexOf(":");
            string host = "";
            if (colonIndex != -1)
            {
                host = ip.Substring(0, colonIndex);
            }
            return host;
        }

        public static int GetPortByIp(this string ip)
        {
            int colonIndex = ip.IndexOf(":");
            string portString = ip.Substring(colonIndex + 1);
            int port;
            if (int.TryParse(portString, out port))
            {
                //nothing to do
            }
            else
            {
                port = 0;
            }
            return port;
        }

        public static string GetStrFrontSymbol(this string str,char symbol){
            int colonIndex = str.IndexOf(symbol);
            string frontStr = "";
            if (colonIndex != -1)
            {
                frontStr = str.Substring(0, colonIndex);
            }
            return frontStr;
        }

        public static string GetStrBehindSymbol(this string str,char symbol){
            int colonIndex = str.IndexOf(symbol);
            string behindStr = str.Substring(colonIndex + 1);
            return behindStr;
        }
    }
}

