using Godot;
using System;
using System.Collections.Generic;
using System.IO.BACnet;
using System.Threading;
using System.Threading.Tasks;

namespace EGFramework.Examples.Gateway{
	public partial class ViewBacNetGateway : Control,IGateway,IEGFramework
	{
		public DataBacnetGatewaySetting DataBacnetGatewaySetting { set; get; }

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			if(this.Visible){
				InitGateway();
			}
		}

		public void InitGateway()
		{
			if(this.EGSave().GetObjectFromJson<DataBacnetGatewaySetting>() == null){
				DataBacnetGatewaySetting = new DataBacnetGatewaySetting(){
					MqttHost = "192.168.1.220",
					HttpServerPrefix = "http://127.0.0.1:5000/",
					ResponseTheme = "/LocalBacnetResponse",
					RequestTheme = "/LocalBacnetRequest"
				};
				this.EGSave().SetObjectToJson(DataBacnetGatewaySetting);
			}else{
				DataBacnetGatewaySetting = this.EGSave().GetObjectFromJson<DataBacnetGatewaySetting>();
			}
			this.EGEnabledProtocolTool<EGBacnet>();
			this.EGEnabledProtocolTool<EGMqtt>();
			InitMqttClient(DataBacnetGatewaySetting);
			this.EGOnMessage<EGBacnetRequest>();
			this.EGOnMessage<EGBacnetResponse>();
			this.EGOnMessage<EGBacnetWhoIsResponse>();
		}
		public void InitMqttClient(DataBacnetGatewaySetting settings){
			this.EGMqtt().OnMqttConnect.Register(e=>{
				if(e == settings.MqttHost){
					this.EGMqtt().SubScribeTheme(settings.MqttHost,settings.RequestTheme);
					// byte[] testData = {0x3A,0x55};
					// this.EGMqtt().PublishTheme("192.168.1.220","test",testData);
				}
			});
			this.EGRegisterMessageEvent<EGBacnetRequest>((e,sender,protocol)=>{
				GD.Print("Sender："+sender);
				if(protocol == ProtocolType.MQTTClient && sender == settings.MqttHost+"|"+settings.RequestTheme){
					GD.Print("MQTT Received->BACnet："+e.ToProtocolData());
					this.EGSendMessage(e,e.DeviceId.ToString(),ProtocolType.Bacnet);
				}
			});
			this.EGRegisterMessageEvent<EGBacnetResponse>((e,sender,protocol)=>{
				if(protocol == ProtocolType.Bacnet){
					GD.Print("BACnet Received->MQTT："+e.ToProtocolData());
					this.EGSendMessage(e,settings.MqttHost+"|"+settings.ResponseTheme,ProtocolType.MQTTClient);
				}
			});
			this.EGRegisterMessageEvent<EGBacnetWhoIsResponse>((e,sender,protocol)=>{;
				if(protocol == ProtocolType.Bacnet){
					GD.Print("BACnet Who Is Received->MQTT："+e.ToProtocolData());
					this.EGSendMessage(e,settings.MqttHost+"|"+settings.ResponseTheme,ProtocolType.MQTTClient);
				}
			});
			this.EGMqtt().ConnectMQTTServer(settings.MqttHost);
			GD.Print("Init Over");
		}
	}
}

