using Godot;
using System;
using System.Net.Sockets;

namespace EGFramework.Examples.Gateway{
	public partial class ViewTcpGateway : Control,IEGFramework,IGateway
	{
		public DataTcpGatewaySetting DataTcpGatewaySetting { set; get; }
		public override void _Ready()
		{
			if(this.Visible){
				InitGateway();
			}
		}

		public void InitGateway(){
			if(this.EGSave().GetDataByFile<DataTcpGatewaySetting>() == null){
				DataTcpGatewaySetting = new DataTcpGatewaySetting();
				DataTcpGatewaySetting.DataTcpGatewayDevices.Add(new DataTcpGatewayDevice(){
					Host = "127.0.0.1",
					Port = 8234,
					MqttHost = "192.168.1.220",
					ResponseTheme = "/LocalTCPResponse",
					RequestTheme = "/LocalTCPRequest"
				});
				this.EGSave().SetDataToFile(DataTcpGatewaySetting);
			}else{
				DataTcpGatewaySetting = this.EGSave().GetDataByFile<DataTcpGatewaySetting>();
			}
			this.EGEnabledProtocolTool<EGTCPClient>();
			this.EGEnabledProtocolTool<EGMqtt>();
			InitMqttClient(DataTcpGatewaySetting);
			this.EGOnMessage<GateWayMessage>();
		}

		public void InitMqttClient(DataTcpGatewaySetting settings){
			foreach(DataTcpGatewayDevice deviceSetting in settings.DataTcpGatewayDevices){
				InitOneMqttClient(deviceSetting);
			}
		}
		
		public async void InitOneMqttClient(DataTcpGatewayDevice deviceSetting){
			this.EGMqtt().OnMqttConnect.Register(e=>{
				if(e == deviceSetting.MqttHost){
					this.EGMqtt().SubScribeTheme(deviceSetting.MqttHost,deviceSetting.RequestTheme);
					// byte[] testData = {0x3A,0x55};
					// this.EGMqtt().PublishTheme("192.168.1.220","test",testData);
				}
			});
			this.EGRegisterMessageEvent<GateWayMessage>((e,sender,protocol)=>{
				GD.Print("Sender："+sender);
				if(protocol == ProtocolType.MQTTClient && sender == deviceSetting.MqttHost+"|"+deviceSetting.RequestTheme){
					GD.Print("MQTT Received->TCP："+e.DataBytes.ToStringByHex());
					this.EGSendMessage(new GateWayMessage(e.DataBytes),deviceSetting.Host+":"+deviceSetting.Port,ProtocolType.TCPClient);
				}
			});
			this.EGRegisterMessageEvent<GateWayMessage>((e,sender,protocol)=>{
				if(protocol == ProtocolType.TCPClient && sender == deviceSetting.Host+":"+deviceSetting.Port){
					GD.Print("TCP Received->MQTT："+e.DataBytes.ToStringByHex());
					this.EGSendMessage(new GateWayMessage(e.DataBytes),deviceSetting.MqttHost+"|"+deviceSetting.ResponseTheme,ProtocolType.MQTTClient);
				}
			});
			this.EGMqtt().ConnectMQTTServer(deviceSetting.MqttHost);
			await this.EGTCPClient().ConnectTCP(deviceSetting.Host,deviceSetting.Port);
			GD.Print("Init Over");
		}

		public override void _Process(double delta)
		{
		}
	}

    public class GateWayMessage : IRequest, IResponse
    {
		public byte[] DataBytes;
		public string DataString;
		public GateWayMessage(){
		}
		public GateWayMessage(byte[] bytes){
			this.DataBytes = bytes;
		}
        public byte[] ToProtocolByteData()
        {
            return DataBytes;
        }

        public string ToProtocolData()
        {
            return DataString;
        }


        public bool TrySetData(string protocolData, byte[] protocolBytes)
        {
            try
			{
				this.DataBytes = protocolBytes;
				this.DataString = protocolData;
				return true;
			}
			catch (System.Exception e)
			{
				GD.PrintErr(e);
				return false;
				throw;
			}
        }
    }
}
