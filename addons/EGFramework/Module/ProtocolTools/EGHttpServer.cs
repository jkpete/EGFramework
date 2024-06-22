using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EGFramework{
    public class EGHttpServer : IEGFramework, IModule
    {
        public HttpListener HttpServer { set; get; }
        public Encoding StringEncoding { set; get; } = Encoding.UTF8;

        public Dictionary<string,HttpListenerResponse> ResponsePools { set; get; } = new Dictionary<string, HttpListenerResponse>();
        
        public Dictionary<string,Func<ResponseMsg,IRequest>> ExecuteDelegates { set; get; } = new Dictionary<string, Func<ResponseMsg, IRequest>>();
        public void Init()
        {

        }

        /// <summary>
        /// if you are in Win7 or newest system, you should add prefix in urlacl by cmd for example: netsh http add urlacl url=http://+:6555/index/ user=Everyone
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public async void Listen(string prefix,Func<ResponseMsg,IRequest> responseFunc){
            ExecuteDelegates.Add(prefix,responseFunc);
            if(!HttpListener.IsSupported){
                return;
            }
            if(HttpServer == null){
                HttpServer = new HttpListener();
            }
            HttpServer.Prefixes.Add(prefix);
            if(!HttpServer.IsListening){
                HttpServer.Start();
                //GD.Print("Http listened in :" + prefix);
                while(true){
                    HttpListenerContext context = await HttpServer.GetContextAsync();
                    HttpListenerRequest request = context.Request;
                    string responseKey = "";
                    ResponseMsg receivedMsgs = new ResponseMsg();
                    try
                    {
                        switch (request.HttpMethod)
                        {
                            case "POST":
                                {
                                    Stream stream = context.Request.InputStream;
                                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                                    // byte[] postBuffer = new byte[stream.Length];
                                    // stream.Read(postBuffer,0,postBuffer.Length);
                                    string postString = reader.ReadToEnd();
                                    receivedMsgs = new ResponseMsg(postString,new byte[1],"POST", ProtocolType.HttpServer);
                                }
                                break;
                            case "GET":
                                {
                                    NameValueCollection data = request.QueryString;
                                    Dictionary<string,string> getDic = data.AllKeys.ToDictionary(k=>k,k=>data[k]);
                                    string getData = JsonConvert.SerializeObject(getDic);
                                    byte[] getBuffer = StringEncoding.GetBytes(getData);
                                    responseKey = "GET:"+this.GetTimeStamp().ToString();
                                    receivedMsgs = new ResponseMsg(getData,getBuffer,responseKey, ProtocolType.HttpServer);
                                    //GD.Print("Received from "+receivedMsgs.sender+": "+receivedMsgs.stringData);
                                }
                                break;
                            case "PUT":
                                {
                                    Stream stream = context.Request.InputStream;
                                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                                    byte[] postBuffer = new byte[stream.Length];
                                    stream.Read(postBuffer,0,postBuffer.Length);
                                    string postData = reader.ReadToEnd();
                                    receivedMsgs = new ResponseMsg(postData,postBuffer,"PUT", ProtocolType.HttpServer);
                                }
                                break;
                            case "PATCH":
                                {
                                    Stream stream = context.Request.InputStream;
                                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                                    byte[] postBuffer = new byte[stream.Length];
                                    stream.Read(postBuffer,0,postBuffer.Length);
                                    string postData = reader.ReadToEnd();
                                    receivedMsgs = new ResponseMsg(postData,postBuffer,"PATCH", ProtocolType.HttpServer);
                                }
                                break;
                            case "DELETE":
                                {
                                    NameValueCollection data = request.QueryString;
                                    Dictionary<string,string> getDic = data.AllKeys.ToDictionary(k=>k,k=>data[k]);
                                    string getData = JsonConvert.SerializeObject(getDic);
                                    byte[] getBuffer = StringEncoding.GetBytes(getData);
                                    receivedMsgs = new ResponseMsg(getData,getBuffer,"DELETE", ProtocolType.HttpServer);
                                }
                                break;
                        }
                    }
                    catch (System.Exception e)
                    {
                        receivedMsgs.stringData = "Exception:"+e;
                        throw;
                    }
                    HttpListenerResponse response = context.Response;
                    response.AppendHeader("Access-Control-Allow-Origin", "*");
                    response.AppendHeader("Access-Control-Allow-Credentials", "true");
                    response.AppendHeader("Server", "MyIIS");
                    response.StatusCode = 200;
                    byte[] buffer = StringEncoding.GetBytes("API not found!");
                    if(ExecuteDelegates.ContainsKey(request.Url.ToString()+"/")){
                        IRequest result = ExecuteDelegates[request.Url.ToString()+"/"].Invoke(receivedMsgs);
                        buffer = StringEncoding.GetBytes(result.ToProtocolData());
                    }
                    System.IO.Stream output = response.OutputStream;
                    await output.WriteAsync(buffer, 0, buffer.Length);
                    output.Close();
                    // if(!ResponsePools.ContainsKey("")){
                    //     ResponsePools.Add("",response);
                    // }
                    // Response("","Hello world");
                }
                
            }
            Godot.GD.Print("Server Overed");
        }

        public IArchitecture GetArchitecture()
        {
            return EGArchitectureImplement.Interface;
        }

    }

    public static class CanGetEGHttpServerExtension{
        public static EGHttpServer EGHttpServer(this IEGFramework self){
            return self.GetModule<EGHttpServer>();
        }

        public static void EGHttpServerListen(this IEGFramework self ,string prefix,Func<ResponseMsg,IRequest> responseFunc){
            self.GetModule<EGHttpServer>().Listen(prefix,responseFunc);
        }
    }

}
