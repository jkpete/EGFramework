using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EGFramework{
    public class EGTCPServer : IModule, IEGFramework,IProtocolReceived
    {
        public Dictionary<int,TcpListener> TcpServerDevices { set; get; } = new Dictionary<int, TcpListener>();
        public Dictionary<int,bool> IsListening { set; get; } = new Dictionary<int, bool>();
        public Dictionary<string, TcpClient> LinkedClients { set; get; } = new Dictionary<string, TcpClient>();
        public Encoding StringEncoding { set; get; } = Encoding.UTF8;

        public EasyEvent<string> OnClientConnect { set; get; } = new EasyEvent<string>();
        public EasyEvent<string> OnClientDisconnect { set; get; } = new EasyEvent<string>();

        public ConcurrentQueue<ResponseMsg> ResponseMsgs { set; get; } = new ConcurrentQueue<ResponseMsg>();

        public string ErrorLogs { set; get; }
        public void Init()
        {
            this.EGRegisterSendAction(request => {
                if(request.protocolType == ProtocolType.TCPServer){
                    if(request.req.ToProtocolData() != null && request.req.ToProtocolData() != ""){
                        ResponseStringData(request.sender,request.req.ToProtocolData());
                    }
                    if(request.req.ToProtocolByteData() != null && request.req.ToProtocolByteData().Length > 0){
                        ResponseByteData(request.sender,request.req.ToProtocolByteData());
                    }
                }
            });
        }

        public ConcurrentQueue<ResponseMsg> GetReceivedMsg()
        {
            return ResponseMsgs;
        }

        public async void StartServer(int port)
        {
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, port);
            TcpListener tcpServer = new TcpListener(ipEndPoint);
            if(TcpServerDevices.ContainsKey(port)){
                return;
            }else{
                TcpServerDevices.Add(port,tcpServer);
                IsListening.Add(port,true);
            }
            TcpServerDevices[port].Start();
            try
            {
                while (IsListening[port])
                {
                    TcpClient client = await TcpServerDevices[port].AcceptTcpClientAsync();
                    LinkedClients.Add(client.Client.RemoteEndPoint.ToString(), client);
                    OnClientConnect.Invoke(client.Client.RemoteEndPoint.ToString());
                    _ = HandleClientAsync(client);
                    EG.Print("[EGTCPServer]"+port+" Client connected: " + client.Client.RemoteEndPoint.ToString());
                }
                TcpServerDevices[port].Stop();
            }
            catch (Exception e)
            {
                EG.Print("[EGTCPServer]"+port+" Error: " + e.ToString());
            }
        }
        public void EndServer(int port){
            IsListening[port] = false;
        }

        public async Task HandleClientAsync(TcpClient client)
        {
            try
            {
                NetworkStream stream = client.GetStream();
                string ClientName = client.Client.RemoteEndPoint.ToString();
                while (client.Connected)
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {
                        break;
                    }
                    string data = StringEncoding.GetString(buffer, 0, bytesRead);
                    ResponseMsg receivedMsgs = new ResponseMsg(data,buffer,ClientName, ProtocolType.TCPServer);
                    //await Task.Run(() => OnDataReceived(receivedMsgs)).ConfigureAwait(false);
                    //this.EGOnReceivedData(receivedMsgs);
                    ResponseMsgs.Enqueue(receivedMsgs);
                }
                //await Task.Run(() => DeleteClient(client)).ConfigureAwait(false);
                EG.Print("[EGTCPServer] Client Disconnected: " + client.Client.LocalEndPoint.ToString() +"--"+ client.Client.RemoteEndPoint.ToString());
                DeleteClient(client);
                client.Close();
            }
            catch (Exception)
            {
            }
        }

        public async void ResponseByteData(string clientName,byte[] data){
            // if serial port not open,open first
            try{
                await this.LinkedClients[clientName]?.GetStream().WriteAsync(data, 0, data.Length);
            }catch(Exception e){
                ErrorLogs = "[write error]" + e.ToString();
            }
        }
        public void ResponseStringData(string clientName,string str){
            byte[] buffer = StringEncoding.GetBytes(str);
            ResponseByteData(clientName,buffer);
        }

        public void DeleteClient(TcpClient client)
        {
            string clientName = client.Client.RemoteEndPoint.ToString();
            if (LinkedClients.ContainsKey(clientName)) {
                LinkedClients.Remove(clientName);
            }
            OnClientDisconnect.Invoke(clientName);
        }

        public IArchitecture GetArchitecture()
        {
            return EGArchitectureImplement.Interface;
        }

    }

    public static class CanGetEGTCPServerExtension{
        public static EGTCPServer EGTCPServer(this IEGFramework self){
            return self.GetModule<EGTCPServer>();
        }

        public static void EGTCPServerListen(this IEGFramework self ,int port){
            self.GetModule<EGTCPServer>().StartServer(port);
        }
        public static void EGTCPServerEndListen(this IEGFramework self ,int port){
            self.GetModule<EGTCPServer>().EndServer(port);
        }

    }
}

