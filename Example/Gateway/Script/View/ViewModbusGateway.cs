using EGFramework;
using EGFramework.Examples.Gateway;
using Godot;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;

namespace EGFramework.Examples.Gateway{
	public partial class ViewModbusGateway : Control,IEGFramework,IGateway
	{
		public DataModbusGatewaySetting Setting { set; get; }

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			if(this.Visible){
				InitGateway();
			}
			// byte[] fData ={0x40,0xB9,0x9B,0x00,0x00,0x00,0x00,0x00};
			// fData.Reverse();
			// GD.Print(fData.ToDoubleArray()[0]);
			// double t = 54.32f;
			// GD.Print(BitConverter.GetBytes(fData.ToDoubleArray()[0]).ToStringByHex()); 
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
		}

		public void InitGateway()
		{
			if(this.EGSave().GetDataByFile<DataModbusGatewaySetting>() == null){
				InitSettings();
				this.EGSave().SetDataToFile(Setting);
			}else{
				Setting = this.EGSave().GetDataByFile<DataModbusGatewaySetting>();
			}
			this.EGEnabledProtocolTool<EGTCPClient>();
			this.EGEnabledProtocolTool<EGSerialPort>();
			this.EGSerialPort().SetBaudRate(9600);
			//this.EGOnMessage<GateWayMessage>();
		}

		public void InitSettings(){
			Setting = new DataModbusGatewaySetting();
			Setting.Delay = 1.0f;
			Setting.Devices485.Add("COM4",new DataModbus485Device(){
				SerialPort = "COM4",
				Address = 0x01,
				BaudRate = 9600
			});
			Setting.Devices485["COM4"].Registers.Add("表头读数",new DataModbusRegister(){
				Address = 0x00,
				RegisterType = ModbusRegisterType.HoldingRegister,
				Name = "表头读数"
			});
		}

		public async void PushDataToGateway(){
			if(!this.Visible){
				return;
			}
			JObject pushData = new JObject();
			foreach(KeyValuePair<string,DataModbus485Device> device485 in Setting.Devices485){
				foreach(KeyValuePair<string,DataModbusRegister> register in Setting.Devices485[device485.Key].Registers){
					ModbusRTU_Response? result = await this.EGModbus().ReadRTUAsync(register.Value.RegisterType,device485.Key,device485.Value.Address,register.Value.Address,0x01);
					if(result != null){
						if(!((ModbusRTU_Response)result).IsError){
							if(register.Value.RegisterType == ModbusRegisterType.HoldingRegister){
								pushData.Add(register.Key,((ModbusRTU_Response)result).HoldingRegister[0]);
							}
						}else{
							GD.Print("Error:"+((ModbusRTU_Response)result).ErrorCode);
						}
					}else{
						GD.Print("Timeout!");
					}
				}
			}
			string resultJson = JsonConvert.SerializeObject(pushData,Formatting.Indented);
			GD.Print(resultJson);
			
			this.EGTCPClient().SendStringData("192.168.1.170",5501,resultJson);
		}
		
		public async void ReadTest(){
			ModbusRTU_Response? result = await this.EGModbus().ReadRTUAsync(ModbusRegisterType.HoldingRegister,"COM4",0x01,0x00,0x01);
			if(result != null){
				if(!((ModbusRTU_Response)result).IsError){
					GD.Print("Register[0]"+((ModbusRTU_Response)result).HoldingRegister[0]);
				}else{
					GD.Print("Error:"+((ModbusRTU_Response)result).ErrorCode);
				}
			}else{
				GD.Print("Timeout!");
			}
		}

		public async void ReadTest2(){
			ModbusRTU_Response? result2 = await this.EGModbus().ReadRTUAsync(ModbusRegisterType.Coil,"COM4",0x01,0x01,0x01);
			if(result2 != null){
				if(!((ModbusRTU_Response)result2).IsError){
					GD.Print("Register[1]"+((ModbusRTU_Response)result2).Coil[0]);
				}else{
					GD.Print("Error:"+((ModbusRTU_Response)result2).ErrorCode);
				}
			}else{
				GD.Print("Timeout!");
			}
		}
		public async void ReadTest3(){
			ModbusRTU_Response? result3 = await this.EGModbus().ReadRTUAsync(ModbusRegisterType.DiscreteInput,"COM4",0x01,0x01,0x01);
			if(result3 != null){
				if(!((ModbusRTU_Response)result3).IsError){
					GD.Print("Register[2]"+((ModbusRTU_Response)result3).DiscreteInput[0]);
				}else{
					GD.Print("Error:"+((ModbusRTU_Response)result3).ErrorCode);
				}
			}else{
				GD.Print("Timeout!");
			}
		}

		public async void WriteTest1(){
			ModbusRTU_Response? result = await this.EGModbus().WriteOnceRTUAsync(ModbusRegisterType.HoldingRegister,"COM4",0x01,0x02,(ushort)0x50);
			if(result != null){
				if(!((ModbusRTU_Response)result).IsError){
					GD.Print("Write[1]"+((ModbusRTU_Response)result).FunctionType);
				}else{
					GD.Print("Error:"+((ModbusRTU_Response)result).ErrorCode);
				}
			}else{
				GD.Print("Timeout!");
			}
		}

		public async void WriteTest2(){
			ModbusRTU_Response? result = await this.EGModbus().WriteOnceRTUAsync(ModbusRegisterType.Coil,"COM4",0x01,0x02,true);
			if(result != null){
				if(!((ModbusRTU_Response)result).IsError){
					GD.Print("Write[2]"+((ModbusRTU_Response)result).FunctionType);
				}else{
					GD.Print("Error:"+((ModbusRTU_Response)result).ErrorCode);
				}
			}else{
				GD.Print("Timeout!");
			}
		}
		
	}
}
