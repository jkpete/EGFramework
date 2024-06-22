using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EGFramework{
    public class EGUDP : IEGFramework, IModule, IProtocolSend, IProtocolReceived
    {
        public Dictionary<int,UdpClient> UDPDevices { set; get; } = new Dictionary<int, UdpClient>();

        public Encoding StringEncoding { set; get; } = Encoding.UTF8;

        public Queue<ResponseMsg> ResponseMsgs { set; get; } = new Queue<ResponseMsg>();

        public void Init()
        {
            this.EGRegisterSendAction(request => {
                if(request.protocolType == ProtocolType.UDP){
                    if(request.req.ToProtocolData() != null && request.req.ToProtocolData() != ""){
                        this.SendStringData(request.sender.GetHostByIp(),request.sender.GetPortByIp(),request.req.ToProtocolData());
                    }
                    if(request.req.ToProtocolByteData() != null && request.req.ToProtocolByteData().Length > 0){
                        this.SendByteData(request.sender.GetHostByIp(),request.sender.GetPortByIp(),request.req.ToProtocolByteData());
                    }
                }
            });
        }

        public void ListenUDP(int localPort){
            if (!UDPDevices.ContainsKey(localPort)) {
                try
                {
                    UdpClient udpDevice = new UdpClient(localPort);
                    UDPDevices.Add(localPort,udpDevice);
                    HandleUDPListenAsync(udpDevice);
                    //StartListening(localPort);
                }
                catch (Exception e){
                    Godot.GD.Print("Error" + e);
                }
            }
        }

        public void EndListenUDP(int localPort){
            if (UDPDevices.ContainsKey(localPort)) {
                UDPDevices[localPort].Close();
                UDPDevices.Remove(localPort);
            }
        }

        public async void HandleUDPListenAsync(UdpClient client)
        {
            try
            {
                while (true)
                {
                    UdpReceiveResult data = await client.ReceiveAsync();
                    string dataStr = StringEncoding.GetString(data.Buffer);
                    ResponseMsg receivedMsgs = new ResponseMsg(dataStr,data.Buffer,data.RemoteEndPoint.ToString(), ProtocolType.UDP);
                    //this.EGOnReceivedData(receivedMsgs);
                    ResponseMsgs.Enqueue(receivedMsgs);
                }
            }
            catch (Exception)
            {
            }
        }

        public void SendByteData(string host,int port,byte[] data){
            UdpClient udpClient = new UdpClient();
            try{
                udpClient.Send(data, data.Length, host, port);
            }
            catch ( Exception e ){
                Godot.GD.Print(e.ToString());	
            }
            udpClient.Close();
            udpClient.Dispose();
        }
        public void SendByteData(string destination,byte[] data){
            SendByteData(destination.GetHostByIp(),destination.GetPortByIp(),data);
        }

        public void SendStringData(string host,int port,string data){
            byte[] buffer = StringEncoding.GetBytes(data);
            this.SendByteData(host,port,buffer);
        }
        public void SendStringData(string destination,string data){
            SendStringData(destination.GetHostByIp(),destination.GetPortByIp(),data);
        }

        public void SetEncoding(Encoding textEncoding){
            StringEncoding = textEncoding;
        }

        public void BroadCastUDPMessage(string host,int port,byte[] message){

        }

        public IArchitecture GetArchitecture()
        {
            return EGArchitectureImplement.Interface;
        }

        public Queue<ResponseMsg> GetReceivedMsg()
        {
            return ResponseMsgs;
        }
    }

    public static class CanGetEGUDPExtension{
        public static EGUDP EGUDP(this IEGFramework self){
            return self.GetModule<EGUDP>();
        }

        public static void EGUDPListen(this IEGFramework self ,int port){
            self.GetModule<EGUDP>().ListenUDP(port);
        }
    }
}

