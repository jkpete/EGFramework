
namespace EGFramework.Examples.Gateway{
	public class DataGatewayInfo 
	{
		public AddressTCPClient TCPClientAddress { set; get; }
	}

	public interface IAddress{
		void GetSender();
	}

	public struct AddressTCPClient{
		public string Host { set; get; }
		public int Port { set; get; }
	}
	public struct AddressTCPServer{
		public int Port { set; get; }
	}
	public struct AddressUDP{
		public string Host { set; get; }
		public int Port { set; get; }
	}
	public struct AddressHttpServer{
		public string[] prefix { set; get; }
	}
	public struct AddressHttpClient{
		public string[] TargetAddress { set; get; }
	}

}
