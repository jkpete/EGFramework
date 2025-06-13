using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Net;

namespace EGFramework{
    public class EGTCPClient : IModule, IEGFramework, IProtocolSend, IProtocolReceived
    {
        public Dictionary<string,TcpClient> TCPClientDevices { set; get; } = new Dictionary<string, TcpClient>();
        
        public string ErrorLogs { set; get; }

        public Encoding StringEncoding { set; get; } = Encoding.UTF8;

        public ConcurrentQueue<ResponseMsg> ResponseMsgs { set; get; } = new ConcurrentQueue<ResponseMsg>();

        public void Init()
        {
            this.EGRegisterSendAction(request=>{
                if(request.protocolType == ProtocolType.TCPClient){
                    if(request.req.ToProtocolData() != null && request.req.ToProtocolData() != ""){
                        this.SendStringData(request.sender.GetHostByIp(),request.sender.GetPortByIp(),request.req.ToProtocolData());
                    }
                    if(request.req.ToProtocolByteData() != null && request.req.ToProtocolByteData().Length > 0){
                        this.SendByteData(request.sender.GetHostByIp(),request.sender.GetPortByIp(),request.req.ToProtocolByteData());
                    }
                }
            });
        }

        /// <summary>
        /// Connect Tcp client to server with check if target server is listened.
        /// </summary>
        public async Task<bool> ConnectTCP(string host,int port){
            try{
                if(!TCPClientDevices.ContainsKey(host + ":" + port)){
                    TcpClient tcpClient = new TcpClient();
                    await tcpClient.ConnectAsync(host,port);
                    //Print("Connect Tcp success in "+tcpClient.Client.RemoteEndPoint.ToString());
                    TCPClientDevices.Add(host + ":" + port,tcpClient);
                    _ = HandleClientAsync(tcpClient,host,port);
                }else{
                    if(!TCPClientDevices[host + ":" + port].Connected){
                        await TCPClientDevices[host + ":" + port].ConnectAsync(host,port);
                        _ = HandleClientAsync(TCPClientDevices[host + ":" + port],host,port);
                    }
                }
                return true;
            }
            catch(Exception e){
                ErrorLogs = "[open port error]" + e.ToString();
                return false;
            }
        }

        /// <summary>
        /// Disconnect Tcp client to server.
        /// </summary>
        public void DisconnectTCP(string host,int port){
            if(TCPClientDevices.ContainsKey(host + ":" + port)){
                if (TCPClientDevices[host + ":" + port].Connected)
                {
                    TCPClientDevices[host + ":" + port].Close();
                    TCPClientDevices.Remove(host + ":" + port);
                }
            }else{
                //Not found in TCPClientDevices,need add?
            }
        }

        
        public void SetEncoding(Encoding textEncoding)
        {
            this.StringEncoding = textEncoding;
        }
        public async void SendByteData(string host,int port,byte[] data){
            // if serial port not open,open first
            try{
                bool result = await ConnectTCP(host,port);
                if(result){
                    await TCPClientDevices[host + ":" + port].GetStream().WriteAsync(data,0,data.Length);
                }
            }catch(Exception e){
                ErrorLogs = "[write error]" + e.ToString();
            }
        }
        public void SendByteData(string destination,byte[] data){
            SendByteData(destination.GetHostByIp(),destination.GetPortByIp(),data);
        }

        public void SendByteDataOnce(string host,int port,byte[] data){
            SendByteData(host,port,data);
            DisconnectTCP(host,port);
        }

        public void SendStringData(string host,int port,string str){
            SendByteData(host,port,StringEncoding.GetBytes(str));
        }

        public void SendStringData(string destination,string data){
            SendStringData(destination.GetHostByIp(),destination.GetPortByIp(),data);
        }
        public void SendStringDataOnce(string host,int port,string str){
            SendStringData(host,port,str);
            DisconnectTCP(host,port);
        }

        public ConcurrentQueue<ResponseMsg> GetReceivedMsg()
        {
            return ResponseMsgs;
        }

        /// <summary>
        /// UpdateStatus
        /// </summary>
        public async void CheckAndRelink(){
            foreach(TcpClient tcpClient in TCPClientDevices.Values){
                if(!tcpClient.Connected){
                   await tcpClient.ConnectAsync((IPEndPoint)tcpClient.Client.RemoteEndPoint);
                }
            }
        }

        public async Task HandleClientAsync(TcpClient client,string host,int port)
        {
            try
            {
                NetworkStream stream = client.GetStream();
                string ClientName = host+":"+port;
                while (true)
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {
                        break;
                    }
                    string data = StringEncoding.GetString(buffer, 0, bytesRead);
                    byte[] receivedByte = new byte[bytesRead];
                    Array.Copy(buffer, 0, receivedByte, 0, bytesRead);
                    ResponseMsg receivedMsgs = new ResponseMsg(data,receivedByte,ClientName, ProtocolType.TCPClient);
                    ResponseMsgs.Enqueue(receivedMsgs);
                    //this.EGOnReceivedData(receivedMsgs);
                }
                DeleteClient(client,host,port);
            }
            catch (Exception)
            {
            }
        }

        public void DeleteClient(TcpClient client,string host,int port)
        {
            client.Close();
            string clientName = host+":"+port;
            if (TCPClientDevices.ContainsKey(clientName)) {
                TCPClientDevices.Remove(clientName);
            }
        }
        
        public IArchitecture GetArchitecture()
        {
            return EGArchitectureImplement.Interface;
        }


    }

    public static class CanGetEGTCPClientExtension{
        public static EGTCPClient EGTCPClient(this IEGFramework self){
            return self.GetModule<EGTCPClient>();
        }

        public static TcpClient EGGetTCPClient(this IEGFramework self,string host,int port){
            return self.GetModule<EGTCPClient>().TCPClientDevices[host + ":" + port];
        }
    }

}
