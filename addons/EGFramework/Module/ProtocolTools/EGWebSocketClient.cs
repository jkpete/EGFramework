using System;
using System.Collections.Concurrent;
using System.Text;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace EGFramework{
    public class EGWebSocketClient : IEGFramework, IModule, IProtocolSend, IProtocolReceived
    {
        public Dictionary<string,ClientWebSocket> WebSocketClientDevices { set; get; } = new Dictionary<string, ClientWebSocket>();
        
        public string ErrorLogs { set; get; }

        public Encoding StringEncoding { set; get; } = Encoding.UTF8;

        public ConcurrentQueue<ResponseMsg> ResponseMsgs { set; get; } = new ConcurrentQueue<ResponseMsg>();
        
        public void Init()
        {
            this.EGRegisterSendAction(request=>{
                if(request.protocolType == ProtocolType.TCPClient){
                    if(request.req.ToProtocolData() != null && request.req.ToProtocolData() != ""){
                        this.SendStringData(request.sender,request.req.ToProtocolData());
                    }
                    if(request.req.ToProtocolByteData() != null && request.req.ToProtocolByteData().Length > 0){
                        this.SendByteData(request.sender,request.req.ToProtocolByteData());
                    }
                }
            });
        }

        /// <summary>
        /// Connect Websocket client to server with check if target server is listened.
        /// </summary>
        public async Task<bool> ConnectWebSocket(string address){
            try{
                Uri uri = new Uri(address);
                if(!WebSocketClientDevices.ContainsKey(address)){
                    ClientWebSocket ws  = new ClientWebSocket();
                    await ws.ConnectAsync(uri,default);
                    //Print("Connect Tcp success in "+tcpClient.Client.RemoteEndPoint.ToString());
                    WebSocketClientDevices.Add(address,ws);
                    _ = HandleClientAsync(ws,address);
                }else{
                    if(WebSocketClientDevices[address].State != WebSocketState.Open){
                        await WebSocketClientDevices[address].ConnectAsync(uri,default);
                        _ = HandleClientAsync(WebSocketClientDevices[address],address);
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
        /// Disconnect Websocket client to server.
        /// </summary>
        public void DisconnectWebsocket(string address){
            if(WebSocketClientDevices.ContainsKey(address)){
                if (WebSocketClientDevices[address].State == WebSocketState.Open)
                {
                    WebSocketClientDevices[address].CloseAsync(WebSocketCloseStatus.NormalClosure,"Client closed",default);
                    WebSocketClientDevices[address].Dispose();
                    WebSocketClientDevices.Remove(address);
                }
            }else{
                //Not found in Websocket client,need add?
            }
        }

        public async Task HandleClientAsync(ClientWebSocket client,string address)
        {
            try
            {
                string ClientName = address;
                while (true)
                {
                    byte[] buffer = new byte[1024];
                    WebSocketReceiveResult result = await client.ReceiveAsync(buffer, default);
                    if (result.Count == 0)
                    {
                        break;
                    }
                    string data = StringEncoding.GetString(buffer, 0, result.Count);
                    byte[] receivedByte = new byte[result.Count];
                    Array.Copy(buffer, 0, receivedByte, 0, result.Count);
                    ResponseMsg receivedMsgs = new ResponseMsg(data,receivedByte,ClientName, ProtocolType.WebSocketClient);
                    ResponseMsgs.Enqueue(receivedMsgs);
                    //this.EGOnReceivedData(receivedMsgs);
                }
                DisconnectWebsocket(address);
            }
            catch (Exception)
            {
            }
        }

        public ConcurrentQueue<ResponseMsg> GetReceivedMsg()
        {
            return this.ResponseMsgs;
        }        

        public async void SendByteDataAsync(string address,byte[] data){
            // if serial port not open,open first
            try{
                bool result = await ConnectWebSocket(address);
                if(result){
                    await WebSocketClientDevices[address].SendAsync(data,WebSocketMessageType.Binary,true,default);
                }
            }catch(Exception e){
                ErrorLogs = "[write error]" + e.ToString();
            }
        }

        public void SendByteData(string destination, byte[] data)
        {
            SendByteDataAsync(destination, data);
        }

        public void SendStringData(string destination, string data)
        {
            SendByteData(destination,StringEncoding.GetBytes(data));
        }

        public void SetEncoding(Encoding textEncoding)
        {
            this.StringEncoding = textEncoding;
        }
        public IArchitecture GetArchitecture()
        {
            return EGArchitectureImplement.Interface;
        }
    }
}
