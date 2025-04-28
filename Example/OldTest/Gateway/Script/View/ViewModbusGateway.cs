using EGFramework;
using EGFramework.Examples.Gateway;
using Godot;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

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
			// byte[] fData = {0x42,0x0D,0x33,0x33};
			// GD.Print(fData.ToFloatArrayBigEndian()[0]);
			this.EGRegisterMessageEvent<TypeTCPSetRotateData>(e=>{
				if(e.ValueSet.ContainsKey("rotational_speed")){
					WriteHoldingRegisterTCP("rotational_speed",e.ValueSet["rotational_speed"]);
					GD.Print("Write success!");
				}
			});
			this.EGOnMessage<TypeTCPSetRotateData>();
			this.GetModule<EGMessage>().SetDelay(0);
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
		}

		public void InitGateway()
		{
			if(this.EGSave().GetObjectFromJson<DataModbusGatewaySetting>() == null){
				InitSettings();
				this.EGSave().SetObjectToJson(Setting);
			}else{
				Setting = this.EGSave().GetObjectFromJson<DataModbusGatewaySetting>();
			}
			this.EGEnabledProtocolTool<EGTCPClient>();
			this.EGEnabledProtocolTool<EGSerialPort>();
			this.EGSerialPort().SetBaudRate(9600);
			//this.EGOnMessage<GateWayMessage>();
		}

		public void InitSettings(){
			Setting = new DataModbusGatewaySetting();
			Setting.Delay = 1.0f;
			// Setting.Devices485.Add("COM4",new DataModbus485Device(){
			// 	SerialPort = "COM4",
			// 	Address = 0x01,
			// 	BaudRate = 9600
			// });
			// Setting.Devices485["COM4"].ValueRegisters.Add("表头读数",new DataModbusValue(){
			// 	Address = 0x00,
			// 	Length = 0x01,
			// 	RegisterType = ModbusRegisterType.HoldingRegister,
			// 	Name = "表头读数"
			// });
			string IpPort = "192.168.1.170:8234";
			Setting.DevicesTCP.Add(IpPort,new DataModbusTCPDevice(){
				Host = IpPort.GetHostByIp(),
				Port = IpPort.GetPortByIp(),
				Address = 0x01
			});
			Setting.DevicesTCP[IpPort].ValueRegisters.Add("温度",new DataModbusValue(){
				Address = 0x04B0,
				Length = 0x01,
				RegisterType = ModbusRegisterType.HoldingRegister,
				Name = "温度"
			});
		}

		public async void PushDataToGateway(){
			if(!this.Visible){
				return;
			}
			JObject pushData = new JObject();
			foreach(KeyValuePair<string,DataModbus485Device> device485 in Setting.Devices485){
				foreach(KeyValuePair<string,DataModbusValue> register in Setting.Devices485[device485.Key].ValueRegisters){
					ModbusRTU_Response? result = await this.EGModbus().ReadRTUAsync(register.Value.RegisterType,device485.Key,device485.Value.Address,register.Value.Address,register.Value.Length);
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

		public async void PushTCPDataToGateway(){
			if(!this.Visible){
				return;
			}
			JObject pushData = new JObject();
			foreach(KeyValuePair<string,DataModbusTCPDevice> deviceTCP in Setting.DevicesTCP){
				foreach(KeyValuePair<string,DataModbusValue> register in Setting.DevicesTCP[deviceTCP.Key].ValueRegisters){
					ModbusTCP_Response? result = await this.EGModbus().ReadTCPAsync(register.Value.RegisterType,deviceTCP.Key,deviceTCP.Value.Address,register.Value.Address,register.Value.Length);
					if(result != null){
						if(!((ModbusTCP_Response)result).IsError){
							if(register.Value.RegisterType == ModbusRegisterType.HoldingRegister){
								object onceValue = 0;
								switch(register.Value.ValueType){
									case DataModbusValueType.Float_:
										onceValue = ((ModbusTCP_Response)result).SourceValueData?.ToFloatArrayBigEndian()[0];
										break;
									case DataModbusValueType.UShort_:
										onceValue = ((ModbusTCP_Response)result).HoldingRegister[0];
										break;
								}
								GD.Print(register.Key+"[Read]:"+(float)onceValue);
								pushData.Add(register.Key, (float)onceValue);
							}
						}else{
							GD.Print("Error:"+((ModbusTCP_Response)result).ErrorCode);
						}
					}else{
						GD.Print("Timeout!");
					}
				}
			}
			string resultJson = JsonConvert.SerializeObject(pushData);
			resultJson = JsonConvert.SerializeObject(new TypeTCPRotateData(resultJson),Formatting.Indented);
			// GD.Print(resultJson);
			JObject loginData = new JObject
            {
                { "type", "SpeedControlDeviceLogin" }
            };
			// this.EGTCPClient().SendStringData("192.168.1.11",9966,loginData.ToString());
			this.EGTCPClient().SendStringData(Setting.TCPClientAddress.Host,Setting.TCPClientAddress.Port,loginData.ToString());
			await Task.Delay(50);
			// this.EGTCPClient().SendStringData("192.168.1.11",9966,resultJson);
			this.EGTCPClient().SendStringData(Setting.TCPClientAddress.Host,Setting.TCPClientAddress.Port,resultJson);
		}
		public async void WriteHoldingRegisterTCP(string registerKey,object value){
			DataModbusValue modbusValue = null;
			string ipPort = "";
			foreach(KeyValuePair<string,DataModbusTCPDevice> deviceTCP in Setting.DevicesTCP){
				if(Setting.DevicesTCP[deviceTCP.Key].ValueRegisters.ContainsKey(registerKey)){
					modbusValue = Setting.DevicesTCP[deviceTCP.Key].ValueRegisters[registerKey];
					ipPort = Setting.DevicesTCP[registerKey].Host+":"+Setting.DevicesTCP[registerKey].Port;
				}
			}
			if(modbusValue == null){
				return;
			}
			ModbusTCP_Response? result = await this.EGModbus().WriteOnceTCPAsync(modbusValue.RegisterType,ipPort,0x01,modbusValue.Address,value);
			if(result != null){
				if(!((ModbusTCP_Response)result).IsError){
					GD.Print("Write"+((ModbusTCP_Response)result).FunctionType);
				}else{
					GD.Print("Error:"+((ModbusTCP_Response)result).ErrorCode);
				}
			}else{
				GD.Print("Timeout!");
			}
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

		public async void ReadTestTCP(ushort address){
			ModbusTCP_Response? result3 = await this.EGModbus().ReadTCPAsync(ModbusRegisterType.HoldingRegister,"192.168.1.170:8234",0x01,address,0x01);
			if(result3 != null){
				if(!((ModbusTCP_Response)result3).IsError){
					GD.Print("Register"+((ModbusTCP_Response)result3).HoldingRegister[0]);
				}else{
					GD.Print("Error:"+((ModbusTCP_Response)result3).ErrorCode);
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

		public async void WriteTestTCP(){
			ModbusTCP_Response? result = await this.EGModbus().WriteOnceTCPAsync(ModbusRegisterType.HoldingRegister,"192.168.1.170:8234",0x01,2005,(ushort)20);
			if(result != null){
				if(!((ModbusTCP_Response)result).IsError){
					GD.Print("Write"+((ModbusTCP_Response)result).FunctionType);
				}else{
					GD.Print("Error:"+((ModbusTCP_Response)result).ErrorCode);
				}
			}else{
				GD.Print("Timeout!");
			}
		}
		
	}
	public struct TypeTCPRotateData{
		public string type;
		public string data;
		public TypeTCPRotateData(string data){
			this.type = "SendRotationalSpeed";
			this.data = data;
		}
	}
	public struct TypeTCPSetRotateData:IResponse{
		public int code { set; get; }
		public string data { set; get; }
		public Dictionary<string,float> ValueSet { set; get; }

        public bool TrySetData(string protocolData, byte[] protocolBytes)
        {
            try
			{
				if(protocolBytes[0]=='{'){
					
					TypeTCPSetRotateData receivedData = JsonConvert.DeserializeObject<TypeTCPSetRotateData>(protocolData);
					this.code = receivedData.code;
					this.data = receivedData.data;
					if(data != null && data != ""){
						ValueSet = JsonConvert.DeserializeObject<Dictionary<string,float>>(data);
					}else{
						return false;
					}
					return true;
				}else{
					return false;
				}
			}
			catch (System.Exception)
			{
				return false;
				throw;
			}
        }
    }
}
