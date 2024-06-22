using Godot;
using System;
using System.Collections.Generic;
using System.IO.Ports;

namespace EGFramework.Examples.ModbusDebugTool{
    public partial class ViewEdit : Control,IEGFramework
    {
        public DataModbusItem DataModbusItem { set; get; }
        public ModbusRegisterType ModbusRegisterType { set; get; }

        public Label Title { set; get; }
        public LineEdit EditDeviceAddress { set; get; }
        public LineEdit EditRegisterAddress { set; get; }
        public OptionButton SerialPortSelect { set; get; }
        public ViewMenu ViewMenu { set; get; }

        public Dictionary<string,int> SerialPortMapping { set; get; } = new Dictionary<string, int>();

        public override void _Ready()
        {
            Title = this.GetNode<Label>("Title");
            SerialPortSelect = this.GetNode<OptionButton>("List/SerialPortSelect");
            EditDeviceAddress = this.GetNode<LineEdit>("List/Device");
            EditRegisterAddress = this.GetNode<LineEdit>("List/Register");
            ViewMenu = this.GetNode<ViewMenu>("/root/Menu");
            ModbusRegisterType = ModbusRegisterType.HoldingRegister;
            RefreshSerialPort();
        }

        public void RefreshSerialPort(){
            string[] portNames = SerialPort.GetPortNames();
            SerialPortMapping.Clear();
            int index = 0;
            SerialPortSelect.Clear();
            foreach (string portName in portNames)
            {
                SerialPortMapping.Add(portName,index);
                SerialPortSelect.AddItem(portName,index);
                index++;
            }
        }

        public void Modify(DataModbusItem modbusItem){
            DataModbusItem = modbusItem;
            ModbusRegisterType = modbusItem.Type;
            Title.Text = ModbusRegisterType.ToString();
            EditDeviceAddress.Text = modbusItem.DeviceAddress.ToString();
            EditRegisterAddress.Text = modbusItem.RegisterAddress.ToString();
            if(SerialPortMapping.ContainsKey(modbusItem.SerialPort)){
                SerialPortSelect.Selected = SerialPortMapping[modbusItem.SerialPort];
            }
        }

        public void New(ModbusRegisterType type){
            DataModbusItem = null;
            ModbusRegisterType = type;
            Title.Text = ModbusRegisterType.ToString();
        }

        public void OnClose(){
            this.Visible = false;
        }

        public void Save(){
            try
            {
                DataModbusItem modbusItem = new DataModbusItem(){
                    DeviceAddress = (byte)int.Parse(EditDeviceAddress.Text),
                    RegisterAddress = (ushort)int.Parse(EditRegisterAddress.Text),
                    SerialPort = SerialPortSelect.GetItemText(SerialPortSelect.GetSelectedId()),
                    Type = ModbusRegisterType
                };
                if(DataModbusItem != null){
                    this.EGGetObject<DataModbus>().Items.Remove(DataModbusItem.GetKey());
                }
                this.EGGetObject<DataModbus>().Items.Add(modbusItem.GetKey(),modbusItem);
                this.EGSave().SetDataToFile(this.EGGetObject<DataModbus>());
                this.Visible = false;
                this.ViewMenu.RefreshSaveData();
                
            }
            catch (System.Exception ex)
            {
                GD.Print("Save Exception" + ex);
            }
            
        }
    }

    public static class ViewEditExtension{
        public static void OnModifyEdit(this Node self,DataModbusItem dataModbusItem){
            self.GetTree().CurrentScene.GetNode<ViewEdit>("Edit").Visible = true;
            self.GetTree().CurrentScene.GetNode<ViewEdit>("Edit").Modify(dataModbusItem);
        }
        public static void OnNewEdit(this Node self,ModbusRegisterType type){
            self.GetTree().CurrentScene.GetNode<ViewEdit>("Edit").Visible = true;
            self.GetTree().CurrentScene.GetNode<ViewEdit>("Edit").New(type);
        }
    }
}

