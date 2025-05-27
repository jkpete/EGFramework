namespace EGFramework.ProtocolHelper
{
    public struct DataDevice
    {
        public string Sender { set; get; }
        public ProtocolType ProtocolType { set; get; }
        private bool IsConnected { set; get; }
    }
}