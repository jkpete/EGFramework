using System;
using System.Collections.Generic;
using System.IO.BACnet;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;
using Newtonsoft.Json;

namespace EGFramework{
    public class EGBacnet : IEGFramework, IModule,IProtocolSend,IProtocolReceived
    {
        public BacnetClient BacnetClient;
		// All the present Bacnet Device List
		public Dictionary<uint,BacnetAddress> DevicesList = new Dictionary<uint,BacnetAddress>();
        public Encoding StringEncoding { set; get; } = Encoding.ASCII;

        public Queue<ResponseMsg> ResponseMsgs { set; get; } = new Queue<ResponseMsg>();

        public void Init()
        {
            this.EGRegisterSendAction(request=>{
                if(request.protocolType == ProtocolType.Bacnet){
                    if(request.req.ToProtocolData() != "" && request.req.ToProtocolData() != null){
                        this.SendStringData(request.sender,request.req.ToProtocolData());
                    }
                    if(request.req.ToProtocolByteData().Length > 0 && request.req.ToProtocolByteData() != null){
                        this.SendByteData(request.sender,request.req.ToProtocolByteData());
                    }
                }
            });
            StartIPV4();
        }

        public void StartIPV4(int port = 0xBAC0)
		{
			// Bacnet on UDP/IP/Ethernet 
			BacnetClient = new BacnetClient(new BacnetIpUdpProtocolTransport(port, false));
            BacnetClient.Start();    // go
			BacnetClient.OnIam += new BacnetClient.IamHandler(OnIam);            
			BacnetClient.WhoIs();
            
			// Or Bacnet Ethernet
			// bacnet_client = new BacnetClient(new BacnetEthernetProtocolTransport("Connexion au réseau local"));      
			
			// Send WhoIs in order to get back all the Iam responses :  
			/* Optional Remote Registration as A Foreign Device on a BBMD at @192.168.1.1 on the default 0xBAC0 port

			bacnet_client.RegisterAsForeignDevice("192.168.1.1", 60);
			Thread.Sleep(20);
			bacnet_client.RemoteWhoIs("192.168.1.1");
			*/
		}
        public void StartIPV6(int port = 0xBAC0){
            //Bacnet on IPV6
            BacnetClient = new BacnetClient(new BacnetIpV6UdpProtocolTransport(port));
            BacnetClient.Start();    // go
			BacnetClient.OnIam += new BacnetClient.IamHandler(OnIam);            
			BacnetClient.WhoIs();
        }
        public void StartMSTP(string serialPort = "COM4",int baudRate = 38400){
            // Bacnet Mstp on COM4 à 38400 bps, own master id 8
			BacnetClient = new BacnetClient(new BacnetMstpProtocolTransport(serialPort, baudRate, 8));
            BacnetClient.Start();    // go
			BacnetClient.OnIam += new BacnetClient.IamHandler(OnIam);            
			BacnetClient.WhoIs();
        }
        

        public void OnIam(BacnetClient sender, BacnetAddress adr, uint device_id, uint max_apdu, BacnetSegmentations segmentation, ushort vendor_id)
		{
			lock (DevicesList)
			{
				// Not already in the list
                if(DevicesList.ContainsKey(device_id)){
                    DevicesList[device_id]=adr;
                }else{
				    DevicesList.Add(device_id,adr);   // add it
                }
                if(!IsWaitForIam){
                    WaitForIamResponse();
                }
			}
		}
        public bool IsWaitForIam { set; get; } = false;
        public async void WaitForIamResponse(){
            IsWaitForIam = true;
            await Task.Delay(2000);
            GD.Print("-----Get device----");
            string serializeData = JsonConvert.SerializeObject(new EGBacnetWhoIsResponse(DevicesList.Keys.ToList()));
            this.ResponseMsgs.Enqueue(new ResponseMsg(serializeData,StringEncoding.GetBytes(serializeData),"Who Is",ProtocolType.Bacnet));
            IsWaitForIam = false;
        }

        public bool ReadProperty(BacnetAddress bacnetAddress, EGBacnetRequest bacnetData, out IList<BacnetValue> Value)
		{
			IList<BacnetValue> NoScalarValue;
			Value = new List<BacnetValue>();
			// Property Read
			if (BacnetClient.ReadPropertyRequest(bacnetAddress, new BacnetObjectId(bacnetData.ObjectTypes, bacnetData.RegisterAddress), bacnetData.PropertyIds, out NoScalarValue)==false)
				return false;

			Value = NoScalarValue;
			return true;
		}
        public bool WriteProperty(BacnetAddress bacnetAddress,EGBacnetRequest bacnetData)
		{
			// Property Write
			BacnetValue[] NoScalarValue = { new BacnetValue(bacnetData.Value) };
            //GD.Print("ValueType is " + bacnetData.ValueType+"|" +bacnetData.Value.GetType());
			if (BacnetClient.WritePropertyRequest(bacnetAddress, new BacnetObjectId(bacnetData.ObjectTypes, bacnetData.RegisterAddress), bacnetData.PropertyIds, NoScalarValue) == false)
				return false;
			return true;
		}
        public bool ExecuteBacnetData(EGBacnetRequest bacnetRequest){
            bool executeResult = false;
            GD.Print("Execute bacnet request");
            if(bacnetRequest.OperateCode == EGBacnetOperateCode.WhoIsRequest){
                GD.Print("Request for Who Is");
                BacnetClient.WhoIs();
                return true;
            }
            if(!DevicesList.ContainsKey(bacnetRequest.DeviceId)){
                return false;
            }
            BacnetAddress address = DevicesList[bacnetRequest.DeviceId];
            IList<BacnetValue> bacnetValueQuery = null;
            switch(bacnetRequest.OperateCode){
                case EGBacnetOperateCode.ReadPropertyRequest:
                    executeResult = ReadProperty(address,bacnetRequest,out bacnetValueQuery);
                    break;
                case EGBacnetOperateCode.WritePropertyRequest:
                    executeResult = WriteProperty(address,bacnetRequest);
                    break;
            }
            EGBacnetResponse response = new EGBacnetResponse(bacnetRequest,bacnetValueQuery);
            response.IsSuccess = executeResult;
            string serializeData = JsonConvert.SerializeObject(response);
            this.ResponseMsgs.Enqueue(new ResponseMsg(serializeData,StringEncoding.GetBytes(serializeData),bacnetRequest.DeviceId.ToString(),ProtocolType.Bacnet));
            return executeResult;
        }

        public EGBacnetWhoIsResponse WhoIs(){
            return new EGBacnetWhoIsResponse(DevicesList.Keys.ToList());
        }

        public EGBacnetResponse ReadRegisterProperty(EGBacnetRequest bacnetRequest){
            bool executeResult = false;
            try
            {
                BacnetAddress address = DevicesList[bacnetRequest.DeviceId];
                IList<BacnetValue> bacnetValueQuery = null;
                switch(bacnetRequest.OperateCode){
                    case EGBacnetOperateCode.ReadPropertyRequest:
                        executeResult = ReadProperty(address,bacnetRequest,out bacnetValueQuery);
                        break;
                        
                }
                GD.Print(bacnetValueQuery[0].Value.GetType());
                EGBacnetResponse response = new EGBacnetResponse(bacnetRequest,bacnetValueQuery);
                response.IsSuccess = executeResult;
                return response;
            }
            catch (System.Exception e)
            {
                EGBacnetResponse response = new EGBacnetResponse(bacnetRequest);
                response.IsSuccess = executeResult;
                response.FailedReason = e.ToString();
                return response;
            }
        }

        public EGBacnetResponse WriteRegisterProperty(EGBacnetRequest bacnetRequest){
            bool executeResult = false;
            try
            {
                if(bacnetRequest.ValueType != BacnetApplicationTags.BACNET_APPLICATION_TAG_NULL){
                    bacnetRequest.Value = bacnetRequest.Value.ConvertBacnetValueType(bacnetRequest.ValueType);
                }
                GD.Print(bacnetRequest.Value.GetType());
                BacnetAddress address = DevicesList[bacnetRequest.DeviceId];
                switch(bacnetRequest.OperateCode){
                    case EGBacnetOperateCode.WritePropertyRequest:
                        executeResult = WriteProperty(address,bacnetRequest);
                        break;
                }
                EGBacnetResponse response = new EGBacnetResponse(bacnetRequest);
                response.IsSuccess = executeResult;
                return response;
            }
            catch (System.Exception e)
            {
                EGBacnetResponse response = new EGBacnetResponse(bacnetRequest);
                response.IsSuccess = executeResult;
                response.FailedReason = e.ToString();
                return response;
            }
        }

        public EGBacnetResponseReadMulti ReadRegisterMulti(EGBacnetRequestReadMulti bacnetRequest){
            bool executeResult = false;
            try
            {
                if(!DevicesList.ContainsKey(bacnetRequest.DeviceId)){
                    return new EGBacnetResponseReadMulti(){
                        FailedReason = "Device " + bacnetRequest.DeviceId + " is offline!"
                    };
                }
                BacnetAddress address = DevicesList[bacnetRequest.DeviceId];
                List<EGBacnetRegisterValueInfo> resultSet = new List<EGBacnetRegisterValueInfo>();
                switch(bacnetRequest.OperateCode){
                    case EGBacnetOperateCode.ReadMultiRequest:
                        foreach(EGBacnetRegisterInfo info in bacnetRequest.RegisterInfos){
                            EGBacnetRegisterValueInfo result = ReadRegisterOne(address,info);
                            resultSet.Add(result);
                        }
                        executeResult = true;
                        break;
                }
                EGBacnetResponseReadMulti response = new EGBacnetResponseReadMulti(resultSet);
                response.IsSuccess = executeResult;
                return response;
            }
            catch (System.Exception e)
            {
                return new EGBacnetResponseReadMulti(){
                    FailedReason = e.ToString()
                };
            }
            
        }
        public EGBacnetRegisterValueInfo ReadRegisterOne(BacnetAddress bacnetAddress, EGBacnetRegisterInfo registerInfo){
            EGBacnetRegisterValueInfo value = new EGBacnetRegisterValueInfo(){
                ObjectTypes = registerInfo.ObjectTypes,
                RegisterAddress = registerInfo.RegisterAddress,
                PropertyIds = registerInfo.PropertyIds
            };
            IList<BacnetValue> NoScalarValue;
            try
            {
                bool IsSuccess = BacnetClient.ReadPropertyRequest(bacnetAddress, new BacnetObjectId(registerInfo.ObjectTypes, registerInfo.RegisterAddress), registerInfo.PropertyIds, out NoScalarValue);
                if(IsSuccess){
                    value.IsSuccess = true;
                    value.Value = NoScalarValue[0];
                }
            }
            catch (System.Exception e)
            {
                GD.Print(e);
                throw;
            }
            return value;
        }

        public void SendByteData(string destination, byte[] data)
        {
            try
            {
                string DataJson = StringEncoding.GetString(data);
                SendStringData(destination,DataJson);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public void SendStringData(string destination, string data)
        {
            try
            {
                EGBacnetRequest bacnetData = JsonConvert.DeserializeObject<EGBacnetRequest>(data);
                ExecuteBacnetData(bacnetData);
            }
            catch (System.Exception e)
            {
                GD.PrintErr(e);
                throw;
            }
        }

        public void SetEncoding(Encoding textEncoding)
        {
            this.StringEncoding = textEncoding;
        }

        public Queue<ResponseMsg> GetReceivedMsg()
        {
            return this.ResponseMsgs;
        }

        public IArchitecture GetArchitecture()
        {
            return EGArchitectureImplement.Interface;
        }
    }

    #region  Who Is
    public class EGBacnetWhoIsResponse : IResponse,IRequest
    {
        public string FunctionCode { set; get; } = "WhoIsResponse";
        public List<uint> DevicesList = new List<uint>();
        public EGBacnetWhoIsResponse(){
        }
        public EGBacnetWhoIsResponse(List<uint> devicesList){
            this.DevicesList = devicesList;
        }
        public bool TrySetData(string protocolData, byte[] protocolBytes)
        {
            try
            {
                EGBacnetWhoIsResponse response = JsonConvert.DeserializeObject<EGBacnetWhoIsResponse>(protocolData);
                if(response.FunctionCode != FunctionCode){
                    return false;
                }
                this.DevicesList = response.DevicesList;
                //throw new System.NotImplementedException();
                return true;
            }
            catch (System.Exception e)
            {
                GD.PrintErr("Who is error:"+ e);
                return false;
            }
        }
        public string ToProtocolData()
        {
            return JsonConvert.SerializeObject(this,Formatting.Indented);
        }

        public byte[] ToProtocolByteData()
        {
            return null;
        }
    }
    #endregion
    
    #region Read&Write One Register
    public class EGBacnetRequest : IRequest,IResponse{
        public string FunctionCode { set; get; } = "OperateRequest";
        public EGBacnetOperateCode OperateCode { set; get; }
        public uint DeviceId { set; get; }
        public BacnetObjectTypes ObjectTypes { set; get; }
        public uint RegisterAddress { set; get; }
        public BacnetPropertyIds PropertyIds { set; get; }
        public object Value { set; get; }
        public BacnetApplicationTags ValueType { set; get; }

        public byte[] ToProtocolByteData()
        {
            return null;
        }

        public string ToProtocolData()
        {
            return JsonConvert.SerializeObject(this,Formatting.Indented);
        }

        public bool TrySetData(string protocolData, byte[] protocolBytes)
        {
            try
            {
                //GD.Print("Init Request");
                EGBacnetRequest request = JsonConvert.DeserializeObject<EGBacnetRequest>(protocolData);
                if(request.FunctionCode != FunctionCode){
                    return false;
                }
                //GD.Print("OperateCode is " + request.OperateCode);
                this.OperateCode = request.OperateCode;
                this.DeviceId = request.DeviceId;
                this.ObjectTypes = request.ObjectTypes;
                this.RegisterAddress = request.RegisterAddress;
                this.PropertyIds = request.PropertyIds;
                this.Value = request.Value;
                this.ValueType = request.ValueType;
                return true;
            }
            catch (System.Exception e)
            {
                GD.PrintErr(e);
                return false;
            }
        }
    }
    public class EGBacnetResponse : IResponse,IRequest
    {
        public string FunctionCode { set; get; } = "OperateResponse";
        public bool IsSuccess { set; get; } = false;
        public EGBacnetRequest Request { set; get; }
        public IList<BacnetValue> ValueQuery { set; get; }
        public string FailedReason { set; get; }

        public EGBacnetResponse(){

        }
        public EGBacnetResponse(EGBacnetRequest request){
            this.Request = request;
        }
        public EGBacnetResponse(EGBacnetRequest request,IList<BacnetValue> values){
            this.Request = request;
            this.ValueQuery = values;
        }
        public bool TrySetData(string protocolData, byte[] protocolBytes)
        {
            try
            {
                EGBacnetResponse response = JsonConvert.DeserializeObject<EGBacnetResponse>(protocolData);
                if(response.FunctionCode != FunctionCode){
                    return false;
                }
                this.IsSuccess = response.IsSuccess;
                this.Request = response.Request;
                this.ValueQuery = response.ValueQuery;
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
            //throw new System.NotImplementedException();
        }

        public string ToProtocolData()
        {
            return JsonConvert.SerializeObject(this,Formatting.Indented);
        }

        public byte[] ToProtocolByteData()
        {
            return null;
        }
    }
    
    #endregion
    
    #region Multi Operate
    public class EGBacnetRequestReadMulti : IRequest,IResponse{
        public string FunctionCode { set; get; } = "ReadMultiRequest";
        public EGBacnetOperateCode OperateCode { set; get; }
        public uint DeviceId { set; get; }
        public List<EGBacnetRegisterInfo> RegisterInfos { set; get; } = new List<EGBacnetRegisterInfo>();

        public byte[] ToProtocolByteData()
        {
            return null;
        }

        public string ToProtocolData()
        {
            return JsonConvert.SerializeObject(this,Formatting.Indented);
        }

        public bool TrySetData(string protocolData, byte[] protocolBytes)
        {
            try
            {
                //GD.Print("Init Request");
                EGBacnetRequestReadMulti request = JsonConvert.DeserializeObject<EGBacnetRequestReadMulti>(protocolData);
                if(request.FunctionCode != FunctionCode){
                    return false;
                }
                //GD.Print("OperateCode is " + request.OperateCode);
                this.OperateCode = request.OperateCode;
                this.DeviceId = request.DeviceId;
                this.RegisterInfos = request.RegisterInfos;
                return true;
            }
            catch (System.Exception e)
            {
                GD.PrintErr(e);
                return false;
            }
        }
    }

    public class EGBacnetResponseReadMulti : IResponse,IRequest
    {
        public string FunctionCode { set; get; } = "ReadMultiResponse";
        public bool IsSuccess { set; get; } = false;
        public List<EGBacnetRegisterValueInfo> RegisterInfos { set; get; } = new List<EGBacnetRegisterValueInfo>();
        public string FailedReason { set; get; }

        public EGBacnetResponseReadMulti(){}
        
        public EGBacnetResponseReadMulti(List<EGBacnetRegisterValueInfo> valueSet){
            this.RegisterInfos = valueSet;
        }

        public bool TrySetData(string protocolData, byte[] protocolBytes)
        {
            try
            {
                EGBacnetResponseReadMulti response = JsonConvert.DeserializeObject<EGBacnetResponseReadMulti>(protocolData);
                if(response.FunctionCode != FunctionCode){
                    FailedReason = "[Error] FunctionCode is error!";
                    return false;
                }
                this.IsSuccess = response.IsSuccess;
                this.RegisterInfos = response.RegisterInfos;
                this.FailedReason = response.FailedReason;
                return true;
            }
            catch (System.Exception e)
            {
                GD.Print(e);
                return false;
            }
            //throw new System.NotImplementedException();
        }

        public string ToProtocolData()
        {
            return JsonConvert.SerializeObject(this,Formatting.Indented);
        }

        public byte[] ToProtocolByteData()
        {
            return null;
        }
    }
    

    public struct EGBacnetRegisterInfo{
        public BacnetObjectTypes ObjectTypes { set; get; }
        public uint RegisterAddress { set; get; }
        public BacnetPropertyIds PropertyIds { set; get; }
    }

    public struct EGBacnetRegisterValueInfo{
        public BacnetObjectTypes ObjectTypes { set; get; }
        public uint RegisterAddress { set; get; }
        public BacnetPropertyIds PropertyIds { set; get; }
        public bool IsSuccess { set; get; }
        public BacnetValue Value{ set; get; }
    }
    
    #endregion

    public enum EGBacnetOperateCode{
        ReadPropertyRequest = 0,
        WritePropertyRequest = 1,
        WhoIsRequest = 2,
        ReadMultiRequest = 3,
    }

    public static class CanGetEGBacnetExtension{
        public static EGBacnet EGBacnet(this IEGFramework self){
            return self.GetModule<EGBacnet>();
        }

        public static object ConvertBacnetValueType(this object value,BacnetApplicationTags valueType){
            object resultValue = value;
            switch(valueType){
                case BacnetApplicationTags.BACNET_APPLICATION_TAG_BOOLEAN:
                    resultValue = Convert.ToBoolean(value);
                    break;
                case BacnetApplicationTags.BACNET_APPLICATION_TAG_UNSIGNED_INT:
                    resultValue = Convert.ToUInt32(value);
                    break;
                case BacnetApplicationTags.BACNET_APPLICATION_TAG_SIGNED_INT:
                    resultValue = Convert.ToInt32(value);
                    break;
                case BacnetApplicationTags.BACNET_APPLICATION_TAG_REAL:
                    resultValue = Convert.ToSingle(value);
                    break;
                case BacnetApplicationTags.BACNET_APPLICATION_TAG_DOUBLE:
                    resultValue = Convert.ToDouble(value);
                    break;
            }
            return resultValue;
        }
    }
}