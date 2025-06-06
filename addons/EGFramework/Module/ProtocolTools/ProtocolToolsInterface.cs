using System.Collections.Concurrent;
using System.Text;

namespace EGFramework{
    public interface IProtocolSend{
        public void SendByteData(string destination,byte[] data);
        public void SendStringData(string destination,string data);
        public void SetEncoding(Encoding textEncoding);
    }
    public interface IProtocolReceived{
        public ConcurrentQueue<ResponseMsg> GetReceivedMsg();
    }

    public interface IProtocolListener{
        public bool IsEnabled(string ServiceName);
    }


}
