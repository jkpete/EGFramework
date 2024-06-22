using EGFramework;
using EGFramework.Examples.Gateway;
using Godot;
using System;
namespace EGFramework.Examples.Gateway{
	public partial class ViewModbusGateway : Control,IEGFramework,IGateway
	{
		public DataModbusGatewaySetting Setting { set; get; }

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			this.EGEnabledProtocolTool<EGSerialPort>();
			this.EGEnabledProtocolTool<EGTCPClient>();
			this.EGSerialPort().SetBaudRate(9600);
			ReadTest();
			ReadTest2();
			ReadTest3();
			ReadTest3();
			ReadTest2();
			ReadTest();
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
		}

		public void InitGateway()
		{
			if(this.EGSave().GetDataByFile<DataModbusGatewaySetting>() == null){
				Setting = new DataModbusGatewaySetting();
				this.EGSave().SetDataToFile(Setting);
			}else{
				Setting = this.EGSave().GetDataByFile<DataModbusGatewaySetting>();
			}
			this.EGEnabledProtocolTool<EGTCPClient>();
			this.EGEnabledProtocolTool<EGSerialPort>();
			//this.EGOnMessage<GateWayMessage>();
		}
		
		public async void ReadTest(){
			ModbusRTU_Response? result = await this.EGModbus().ReadRTUAsync(ModbusRegisterType.HoldingRegister,"COM4",0x01,0x00,0x01);
			if(result != null){
				GD.Print("Register[0]"+((ModbusRTU_Response)result).HoldingRegister[0]);
			}else{
				GD.Print("Timeout!");
			}
		}

		public async void ReadTest2(){
			ModbusRTU_Response? result2 = await this.EGModbus().ReadRTUAsync(ModbusRegisterType.HoldingRegister,"COM4",0x01,0x01,0x01);
			if(result2 != null){
				GD.Print("Register[1]"+((ModbusRTU_Response)result2).HoldingRegister[0]);
			}else{
				GD.Print("Timeout!");
			}
		}
		public async void ReadTest3(){
			ModbusRTU_Response? result3 = await this.EGModbus().ReadRTUAsync(ModbusRegisterType.HoldingRegister,"COM4",0x01,0x10,0x01);
			if(result3 != null){
				GD.Print("Register[2]"+((ModbusRTU_Response)result3).HoldingRegister[0]);
			}else{
				GD.Print("Timeout!");
			}
		}
		
	}
}