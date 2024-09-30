using EGFramework;
using Godot;
using System;
namespace EGFramework.Examples.Gateway{
	public partial class ViewBacnetHttpServer : Control,IGateway,IEGFramework
	{
		public DataBacnetGatewaySetting DataBacnetGatewaySetting { set; get; }
		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			if(this.Visible){
				InitGateway();
			}
		}
		public void InitGateway(){
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
			this.EGHttpServerListen(DataBacnetGatewaySetting.HttpServerPrefix+"WhoIs/",requestMsg=>{
				GD.Print("---WhoIsRequest---");
				return this.EGBacnet().WhoIs();
			});
			this.EGHttpServerListen(DataBacnetGatewaySetting.HttpServerPrefix+"ReadRegisterProperty/",requestMsg=>{
				GD.Print("---ReadRegisterPropertyRequest---");
				EGBacnetRequest bacnetRequest = new EGBacnetRequest();
				bacnetRequest.TrySetData(requestMsg.stringData,null);
				EGBacnetResponse response = this.EGBacnet().ReadRegisterProperty(bacnetRequest);
				return response;
			});
			this.EGHttpServerListen(DataBacnetGatewaySetting.HttpServerPrefix+"ReadMultiRegister/",requestMsg=>{
				GD.Print("---ReadMultiRequest---");
				EGBacnetRequestReadMulti bacnetRequest = new EGBacnetRequestReadMulti();
				bacnetRequest.TrySetData(requestMsg.stringData,null);
				GD.Print("Get "+bacnetRequest.RegisterInfos.Count+" Values Request");
				EGBacnetResponseReadMulti response = this.EGBacnet().ReadRegisterMulti(bacnetRequest);
				return response;
			});

			this.EGHttpServerListen(DataBacnetGatewaySetting.HttpServerPrefix+"WriteRegisterProperty/",requestMsg=>{
				GD.Print("---WriteRegisterPropertyRequest---");
				EGBacnetRequest bacnetRequest = new EGBacnetRequest();
				bacnetRequest.TrySetData(requestMsg.stringData,null);
				EGBacnetResponse response = this.EGBacnet().WriteRegisterProperty(bacnetRequest);
				return response;
			});
		}
	}
}
