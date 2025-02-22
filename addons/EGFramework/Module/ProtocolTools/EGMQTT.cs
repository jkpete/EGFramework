using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;

//Important:
//This EGModule implement by the nuget package MQTTnet:
//the project url is: https://github.com/dotnet/MQTTnet
//license is : https://github.com/dotnet/MQTTnet/blob/master/LICENSE by MIT license
namespace EGFramework{
    public class EGMqtt : IEGFramework, IModule, IProtocolSend, IProtocolReceived
    {
        public MqttFactory MqttFactory = new MqttFactory();
        public Dictionary<string,IMqttClient> MqttDevices { set; get; } = new Dictionary<string, IMqttClient>();

        public Encoding StringEncoding { set; get; } = Encoding.UTF8;

        public Queue<ResponseMsg> ResponseMsgs { set; get; } = new Queue<ResponseMsg>();

        public EasyEvent<string> OnMqttConnect { set; get; } = new EasyEvent<string>();


        public void Init()
        {
            this.EGRegisterSendAction(request=>{
                if(request.protocolType == ProtocolType.MQTTClient){
                    if(request.req.ToProtocolData() != null && request.req.ToProtocolData() != ""){
                        this.SendStringData(request.sender.GetStrFrontSymbol('|'),request.sender.GetStrBehindSymbol('|'),request.req.ToProtocolData());
                    }
                    if(request.req.ToProtocolByteData() != null && request.req.ToProtocolByteData().Length > 0){
                        this.SendByteData(request.sender.GetStrFrontSymbol('|'),request.sender.GetStrBehindSymbol('|'),request.req.ToProtocolByteData());
                    }
                }
            });
        }

        public async void ConnectMQTTServer(string serverURL){
            if(!MqttDevices.ContainsKey(serverURL)){
                IMqttClient mqttClient = MqttFactory.CreateMqttClient();
                var mqttClientOptions = new MqttClientOptionsBuilder().WithTcpServer(serverURL).Build();
                mqttClient.ApplicationMessageReceivedAsync += e =>
                {
                    byte[] receivedBytes = e.ApplicationMessage.PayloadSegment.ToArray();
                    ResponseMsgs.Enqueue(new ResponseMsg(StringEncoding.GetString(receivedBytes),receivedBytes,serverURL + "|" + e.ApplicationMessage.Topic,ProtocolType.MQTTClient));
                    //GD.Print(e.ApplicationMessage.Topic+":"+e.ApplicationMessage.PayloadSegment.ToArray().ToStringByHex());
                    return Task.CompletedTask;
                };
                await mqttClient.ConnectAsync(mqttClientOptions,CancellationToken.None);
                MqttDevices.Add(serverURL,mqttClient);
                EG.Print("Success Connect!"+MqttDevices[serverURL].IsConnected);
                OnMqttConnect.Invoke(serverURL);
            }else{
                if(!MqttDevices[serverURL].IsConnected){
                    var mqttClientOptions = new MqttClientOptionsBuilder().WithTcpServer(serverURL).Build();
                    await MqttDevices[serverURL].ConnectAsync(mqttClientOptions,CancellationToken.None);
                    EG.Print("Success Connect!"+MqttDevices[serverURL].IsConnected);
                    OnMqttConnect.Invoke(serverURL);
                }else{
                    EG.Print("Server has been Connected"+MqttDevices[serverURL].IsConnected);
                    OnMqttConnect.Invoke(serverURL);
                }
            }
        }

        public async void DisconnectMQTTServer(string serverURL){
            if(MqttDevices.ContainsKey(serverURL) && MqttDevices[serverURL].IsConnected){
                await MqttDevices[serverURL].DisconnectAsync(new MqttClientDisconnectOptionsBuilder().WithReason(MqttClientDisconnectOptionsReason.NormalDisconnection).Build());
            }else{
                EG.Print("Not connect");
            }
        }

        public async void SubScribeTheme(string serverURL,string Theme){
            MqttClientSubscribeOptions mqttSubscribeOptions = MqttFactory.CreateSubscribeOptionsBuilder()
                .WithTopicFilter(
                    f =>
                    {
                        f.WithTopic(Theme);
                    })
                .Build();
            if(MqttDevices.ContainsKey(serverURL) && MqttDevices[serverURL].IsConnected){
                await MqttDevices[serverURL].SubscribeAsync(mqttSubscribeOptions,CancellationToken.None);
                EG.Print("Subscribe "+Theme+" success!");
            }else{
                EG.Print("Not connect");
            }
        }

        public async void UnSubScribeTheme(string serverURL,string Theme){
            MqttClientUnsubscribeOptions mqttUnSubscribeOptions = MqttFactory.CreateUnsubscribeOptionsBuilder()
                .WithTopicFilter(Theme)
                .Build();
            if(MqttDevices.ContainsKey(serverURL) && MqttDevices[serverURL].IsConnected){
                await MqttDevices[serverURL].UnsubscribeAsync(mqttUnSubscribeOptions,CancellationToken.None);
            }
        }

        public async void PublishTheme(string serverURL,string Theme,string Data){
            var applicationMessage = new MqttApplicationMessageBuilder()
                .WithTopic(Theme)
                .WithPayload(Data)
                .Build();
            if(MqttDevices.ContainsKey(serverURL) && MqttDevices[serverURL].IsConnected){
                await MqttDevices[serverURL].PublishAsync(applicationMessage, CancellationToken.None);
            }
        }

        public async void PublishTheme(string serverURL,string Theme,byte[] Data){
            var applicationMessage = new MqttApplicationMessageBuilder()
                .WithTopic(Theme)
                .WithPayload(Data)
                .Build();
            if(MqttDevices.ContainsKey(serverURL) && MqttDevices[serverURL].IsConnected){
                await MqttDevices[serverURL].PublishAsync(applicationMessage, CancellationToken.None);
                EG.Print("publish success!");
            }
        }

        public void SendByteData(string serverURL,string theme, byte[] data)
        {
            this.PublishTheme(serverURL,theme,data);
        }
        public void SendByteData(string destination, byte[] data)
        {
            this.SendByteData(destination.GetStrFrontSymbol('|'),destination.GetStrBehindSymbol('|'),data);
        }

        public void SendStringData(string serverURL,string theme, string data)
        {
            this.PublishTheme(serverURL,theme,data);
        }
        public void SendStringData(string destination, string data)
        {
            this.SendStringData(destination.GetStrFrontSymbol('|'),destination.GetStrBehindSymbol('|'),data);
        }

        public void SetEncoding(Encoding textEncoding)
        {
            StringEncoding = textEncoding;
        }
        public Queue<ResponseMsg> GetReceivedMsg()
        {
            return ResponseMsgs;
        }
        
        public IArchitecture GetArchitecture()
        {
            return EGArchitectureImplement.Interface;
        }
    }

    public static class CanGetEGMqttExtension{
        public static EGMqtt EGMqtt(this IEGFramework self){
            return self.GetModule<EGMqtt>();
        }
    }
}