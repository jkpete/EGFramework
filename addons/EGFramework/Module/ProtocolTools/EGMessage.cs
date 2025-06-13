using System;
using System.Collections.Concurrent;
using System.Timers;

namespace EGFramework
{
    //before use this module,you should install NewtonSoft.Json Nuget Package
    //in vscode,you can install this package by install vscode extension NuGet Package Manager GUI
    //search 'Newton' and choose Newtonsoft.Json
    //EGMessage, Every Message tools will be based in this class
    public class EGMessage : EGModule
    {
        public EasyEvent<ResponseMsg> OnDataReceived { set; get; } = new EasyEvent<ResponseMsg>();
        public EasyEvent<ResponseMsgEvent> OnResponse { set; get; } = new EasyEvent<ResponseMsgEvent>();
        public EasyEvent<RequestMsgEvent> OnRequest { set; get; } = new EasyEvent<RequestMsgEvent>();

        /// <summary>
        /// Send delay in millisecond,if you don't need a Timer to delay send message,you can set it to 0. ( this delay options is prevent for sticky package )
        /// </summary>
        /// <value></value>
        public int SendDelay { set; get; } = 100;
        public ConcurrentDictionary<string,ConcurrentQueue<RequestMsgEvent>> RequestCache { set; get; } = new ConcurrentDictionary<string,ConcurrentQueue<RequestMsgEvent>>();
        private System.Timers.Timer RequestTimer { set; get; }

        public override void Init()
        {
            if(SendDelay>0){
                RequestTimer = new System.Timers.Timer(SendDelay);
                RequestTimer.Elapsed += ExecuteRequest;
                RequestTimer.AutoReset = true;
                RequestTimer.Enabled = true;
            }
        }
        #region ReceiveFunctions
        private void ReceiveResponse<T>(ResponseMsg msg) where T :IResponse, new()
        {
            this.ExecuteResponse(new T(),msg.stringData,msg.byteData,msg.sender,msg.protocolType);
        }
        /// <summary>
        /// Start to receive type of T data
        /// </summary>
        /// <typeparam name="T">Data class of message</typeparam>
        public void OnReceive<T>() where T : IResponse, new()
        {
            OnDataReceived.Register(ReceiveResponse<T>);
        }
        /// <summary>
        /// Stop to receive type of T data
        /// </summary>
        /// <typeparam name="T">Data class of message</typeparam>
        public void OffReceive<T>() where T : IResponse, new()
        {
            OnDataReceived.UnRegister(ReceiveResponse<T>);
        }
        #endregion
        #region request & response

        public void SendRequest<TRequest>(TRequest request,string sender,ProtocolType protocolType) where TRequest:IRequest
        {
            if(SendDelay>0){
                if(!RequestCache.ContainsKey(sender)){
                    RequestCache[sender] = new ConcurrentQueue<RequestMsgEvent>();
                }
                RequestCache[sender].Enqueue(new RequestMsgEvent(request,sender,protocolType));
            }else{
                OnRequest.Invoke(new RequestMsgEvent(request,sender,protocolType));
            }
            //ExecuteRequest();
            //OnRequest.Invoke(requestCache.Dequeue());
        }

        private void ExecuteRequest(object source, ElapsedEventArgs e){
            foreach(ConcurrentQueue<RequestMsgEvent> singleCache in RequestCache.Values){
                if(singleCache.Count>0){
                    singleCache.TryDequeue(out RequestMsgEvent msg);
                    OnRequest.Invoke(msg);
                }
            }
        }
        
        private void ExecuteResponse<TResponse>(TResponse response,string protocolString,byte[] protocolBytes,string sender,ProtocolType protocolType) where TResponse:IResponse
        {
            bool isSet = response.TrySetData(protocolString,protocolBytes);
            if (isSet)
            {
                //this.SendEvent(new ResponseMsgEvent(response, sender));
                OnResponse.Invoke(new ResponseMsgEvent(response,sender,protocolType));
            }
        }
        #endregion

        public void SetDelay(int millisecond){
            this.SendDelay = millisecond;
            if(millisecond != 0){
                RequestTimer.Interval = millisecond;
            }
        }
    }
    
    #region interface
    public interface IResponse
    {
        /// <summary>
        /// Attempt to fill in the data. If it does not comply with the relevant protocol rules, it is recommended to return false. If false is returned here, the data response will be ignored.
        /// </summary>
        /// <param name="protocolData">original received</param>
        /// <returns></returns>
        bool TrySetData(string protocolData,byte[] protocolBytes);
    }
    public interface IRequest
    {
        /// <summary>
        /// define you message info in this function,and this return will be send to server&client by request.
        /// </summary>
        /// <returns>request info</returns>
        string ToProtocolData();
        
        byte[] ToProtocolByteData();
    }
    #endregion
    #region AbstractClass
    
    public class BaseJsonResponse : IResponse
    {
        private string ExceptionMsg;
        public virtual string ToProtocolData()
        {
            return "";
        }
        public virtual bool TrySetData(string json,byte[] bytes)
        {
            try
            {
                return true;
            }
            catch (Exception e)
            {
                ExceptionMsg = e.ToString();
                //PrintErr(ExceptionMsg);
                return false;
            }
        }
    }
    
    public class StringRequest : IRequest
    {
        private string RequestStr;
        public StringRequest() {
            RequestStr = "No message";
        }
        public StringRequest(string str) {
            RequestStr = str;
        }

        public byte[] ToProtocolByteData()
        {
            return null;
        }

        public string ToProtocolData()
        {
            return RequestStr;
        }
    }
    #endregion
    #region Extension
    public static class CanRegisterMessageExtension {
        /// <summary>
        /// To register event until message received,if you only need message,please use:
        /// this.RegisterMessageEvent<BaseJsonResponse>(e=>{ //To execute your message})
        /// if you want to get sender,you also can to:
        /// this.RegisterMessageEvent<BaseJsonResponse>((res,sender))=>{ //To execute your message
        ///     Print(res.toProtocolData());
        ///     Print(sender);
        /// })
        /// </summary>
        /// <param name="self"></param>
        /// <param name="onEvent"></param>
        /// <typeparam name="TResponse"></typeparam>
        /// <returns></returns>
        public static IUnRegister EGRegisterMessageEvent<TResponse>(this IEGFramework self, Action<TResponse> onEvent)where TResponse : IResponse
        {
            return EGArchitectureImplement.Interface.GetModule<EGMessage>().OnResponse.Register(e=> {
                if (e.res.GetType() == typeof(TResponse)) {
                    onEvent.Invoke((TResponse)e.res);
                }
            });
        }
        /// <summary>
        /// To register event until message received,if you only need message,please use:
        /// this.RegisterMessageEvent<BaseJsonResponse>(e=>{ //To execute your message})
        /// if you want to get sender,you also can to:
        /// this.RegisterMessageEvent<BaseJsonResponse>((res,sender))=>{ //To execute your message
        ///     Print(res.toProtocolData());
        ///     Print(sender);
        /// })
        /// </summary>
        /// <param name="self"></param>
        /// <param name="onEvent"></param>
        /// <typeparam name="TResponse"></typeparam>
        /// <returns></returns>
        public static IUnRegister EGRegisterMessageEvent<TResponse>(this IEGFramework self, Action<TResponse,string> onEvent)where TResponse : IResponse
        {
            return EGArchitectureImplement.Interface.GetModule<EGMessage>().OnResponse.Register(e=> {
                if (e.res.GetType() == typeof(TResponse)) {
                    onEvent.Invoke((TResponse)e.res,e.sender);
                }
            });
        }

        public static IUnRegister EGRegisterMessageEvent<TResponse>(this IEGFramework self, Action<TResponse,string,ProtocolType> onEvent)where TResponse : IResponse
        {
            return EGArchitectureImplement.Interface.GetModule<EGMessage>().OnResponse.Register(e=> {
                if (e.res.GetType() == typeof(TResponse)) {
                    onEvent.Invoke((TResponse)e.res,e.sender,e.protocolType);
                }
            });
        }
        /// <summary>
        /// Start to receive type of TResponse data
        /// </summary>
        /// <param name="self"></param>
        /// <typeparam name="TResponse"></typeparam>
        public static void EGOnMessage<TResponse>(this IEGFramework self) where TResponse : IResponse,new()
        {
            EGArchitectureImplement.Interface.GetModule<EGMessage>().OnReceive<TResponse>();
        }
        /// <summary>
        /// Stop to receive type of TResponse data
        /// </summary>
        /// <param name="self"></param>
        /// <typeparam name="TResponse"></typeparam>
        public static void EGOffMessage<TResponse>(this IEGFramework self) where TResponse : IResponse,new()
        {
            EGArchitectureImplement.Interface.GetModule<EGMessage>().OffReceive<TResponse>();
        }
    }
    public static class CanSendMessageExtension {
        /// <summary>
        /// to send message by request and define sender
        /// </summary>
        /// <param name="self"></param>
        /// <param name="request"></param>
        /// <param name="sender"></param>
        /// <typeparam name="TRequest"></typeparam>
        public static void EGSendMessage<TRequest>(this IEGFramework self, TRequest request,string sender,ProtocolType protocolType)where TRequest : IRequest
        {
            EGArchitectureImplement.Interface.GetModule<EGMessage>().SendRequest(request,sender,protocolType);
        }
    }
    
    /// <summary>
    /// this extension to link with protocol tools,such as tcp,udp,serial port,etc...
    /// </summary>
    public static class EGMessageEventExtension{
        public static void EGOnReceivedData(this IModule self, ResponseMsg receivedData)
        {
            EGArchitectureImplement.Interface.GetModule<EGMessage>().OnDataReceived.Invoke(receivedData);
        }
        public static void EGRegisterSendAction(this IModule self, Action<RequestMsgEvent> sendAction){
            EGArchitectureImplement.Interface.GetModule<EGMessage>().OnRequest.Register(sendAction);
        }
    }
    #endregion
    #region event

    public struct ResponseMsg
    {
        public string sender;
        public string stringData;
        public byte[] byteData;
        
        public ProtocolType protocolType;
        public ResponseMsg(string stringData_,byte[] byteData_,string sender_,ProtocolType protocolType_)
        {
            stringData = stringData_;
            byteData = byteData_;
            sender = sender_;
            protocolType = protocolType_;
        }
    }

    public struct ResponseMsgEvent
    {
        public IResponse res;
        public string sender;
        public ProtocolType protocolType;
        public ResponseMsgEvent(IResponse res_,string sender_,ProtocolType protocolType_)
        {
            res = res_;
            sender = sender_;
            protocolType = protocolType_;
        }
    }

    public struct RequestMsgEvent
    {
        public IRequest req;
        public string sender;
        public ProtocolType protocolType;
        public RequestMsgEvent(IRequest req_ ,string sender_,ProtocolType protocolType_)
        {
            req = req_;
            sender = sender_;
            protocolType = protocolType_;
        }
    }
    #endregion
    
    public enum ProtocolType{
        TCPClient = 0x00,
        TCPServer = 0x01,
        UDP = 0x02,
        SerialPort = 0x03,
        WebSocketClient = 0x10,
        WebSocketServer = 0x11,
        HttpServer = 0x20,
        HttpGet = 0x21,
        HttpPost = 0x22,
        HttpPut = 0x23,
        HttpPatch = 0x24,
        HttpDelete = 0x25,
        DTLSClient = 0x30,
        DTLSServer = 0x31,
        SSLClient = 0x40,
        SSLServer = 0x41,
        FileStream = 0x50,
        MemoryStream = 0x60,
        MQTTClient = 0x70,
        Bacnet = 0x80,
        SSHClient = 0x90,
        Process = 0xA0,
        //MQTT,SSH,etc...
    }
}