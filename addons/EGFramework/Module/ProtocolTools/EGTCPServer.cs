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
        public TcpListener TcpServer { set; get; }

        public bool IsListening { set; get; }
        public Dictionary<string, TcpClient> LinkedClients { set; get; } = new Dictionary<string, TcpClient>();
        public Encoding StringEncoding { set; get; } = Encoding.UTF8;
        public List<string> ClientNames = new List<string>();

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
            TcpServer = new(ipEndPoint);
            try
            {
                TcpServer.Start();
                IsListening = true;
                while (IsListening)
                {
                    TcpClient client = await TcpServer.AcceptTcpClientAsync();
                    ClientNames.Add(client.Client.RemoteEndPoint.ToString());
                    LinkedClients.Add(client.Client.RemoteEndPoint.ToString(), client);
                    OnClientConnect.Invoke(client.Client.RemoteEndPoint.ToString());
                    _ = HandleClientAsync(client);
                }
                TcpServer.Stop();
            }
            catch (Exception)
            {
            }
        }

        public async Task HandleClientAsync(TcpClient client)
        {
            try
            {
                NetworkStream stream = client.GetStream();
                string ClientName = client.Client.RemoteEndPoint.ToString();
                while (true)
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
            if (ClientNames.Contains(clientName)) {
                ClientNames.Remove(clientName);
            }
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

    }
}

