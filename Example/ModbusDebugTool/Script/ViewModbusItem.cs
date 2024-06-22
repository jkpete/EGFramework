using Godot;
using System;

namespace EGFramework.Examples.ModbusDebugTool{
    public partial class ViewModbusItem : Node,IEGFramework
    {
        public DataModbusItem DataModbusItem { set; get; }

        public Label ID { set; get; }
        
        public Label Title { set; get; }

        public Label Value { set; get; }

        public LineEdit InputData { set; get; }
        
        private bool IsLoadUI { set; get; }

        public override void _Ready()
        {

        }

        public void LoadUI(){
            if(!IsLoadUI){
                Title = this.GetNode<Label>("Title");
                Value = this.GetNode<Label>("Value");
                ID = this.GetNode<Label>("ID");
                InputData = this.GetNode<LineEdit>("WriteEdit");
                InputData.Text = "0";
                IsLoadUI = true;
            }
        }

        public void LoadData(DataModbusItem data){
            LoadUI();
            this.DataModbusItem = data;
            Title.Text = data.Type.ToString();
            ID.Text = data.DeviceAddress+"-"+data.RegisterAddress;
            Value.Text = "";
        }
        public void OnModifyItem(){
            this.OnModifyEdit(DataModbusItem);
        }

        public void WriteValue(){
            try
            {
                IRequest WriteRequest;
                switch(DataModbusItem.Type){
                    case ModbusRegisterType.HoldingRegister:
                        WriteRequest = new ModbusRTU_WriteSingleHoldingRegister
                            (DataModbusItem.DeviceAddress,DataModbusItem.RegisterAddress,ushort.Parse(InputData.Text));
                        this.AppendMessage("【发送-"+DataModbusItem.SerialPort+"】 "+WriteRequest.ToProtocolByteData().ToStringByHex());
                        this.EGSendMessage(WriteRequest,DataModbusItem.SerialPort,ProtocolType.SerialPort);
                        break;
                }
            }
            catch (System.Exception ex)
            {
                GD.PrintErr(ex);
            }
        }

        public void OnDeleteItem(){
            if(this.EGGetObject<DataModbus>().Items.ContainsKey(DataModbusItem.GetKey())){
                this.EGGetObject<DataModbus>().Items.Remove(DataModbusItem.GetKey());
            }
            this.EGSave().SetDataToFile(this.EGGetObject<DataModbus>());
            this.GetNode<ViewMenu>("/root/Menu").RefreshSaveData();
        }

        public void GetValue(){
            IRequest ReadRequest;
            switch(DataModbusItem.Type){
                case ModbusRegisterType.HoldingRegister:
                    ReadRequest = new ModbusRTU_ReadHoldingRegisters
                        (DataModbusItem.DeviceAddress,DataModbusItem.RegisterAddress,1);
                    this.AppendMessage("【发送-"+DataModbusItem.SerialPort+"】 "+ReadRequest.ToProtocolByteData().ToStringByHex());
                    this.EGSendMessage(ReadRequest,DataModbusItem.SerialPort,ProtocolType.SerialPort);
                    this.EGSerialPort().SetExpectReceivedDataLength(6);
                    break;
            }
            this.GetNode<ViewMenu>("/root/Menu").OnModbusRTUGet.Register(OnValueGet);
        }

        public void OnValueGet(ModbusRTU_Response e){
            if((int)e.FunctionType == (int)DataModbusItem.Type && DataModbusItem.DeviceAddress == e.DeviceAddress){
                switch(DataModbusItem.Type){
                    case ModbusRegisterType.HoldingRegister:
                        this.Value.Text = e.HoldingRegister[0].ToString();
                        break;
                }
            }
            this.GetNode<ViewMenu>("/root/Menu").OnModbusRTUGet.UnRegister(OnValueGet);
        }
    }

}
