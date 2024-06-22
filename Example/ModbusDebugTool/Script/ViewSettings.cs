using Godot;
using System;

namespace EGFramework.Examples.ModbusDebugTool{
    public partial class ViewSettings : Control,IEGFramework
    {
        public LineEdit EditBaudRate { set; get; }

        public override void _Ready()
        {
            EditBaudRate = this.GetNode<LineEdit>("List/Device");
            this.Visible = false;
            LoadSettings();
        }

        public void LoadSettings(){
            DataModbusSettings settings = this.EGSave().GetDataByFile<DataModbusSettings>();
            this.EGRegisterObject(settings);
            UpdateSettings(settings);
            EditBaudRate.Text = settings.BaudRate.ToString();
        }

        public void OnClose(){
            this.Visible = false;
        }

        public void UpdateSettings(DataModbusSettings settings){
            this.EGSerialPort().SetBaudRate(settings.BaudRate);
        }

        public void Save(){
            try
            {
                DataModbusSettings settings = new DataModbusSettings(){
                    BaudRate = int.Parse(EditBaudRate.Text)
                };
                UpdateSettings(settings);
                this.EGRegisterObject(settings);
                this.EGSave().SetDataToFile(this.EGGetObject<DataModbusSettings>());
                this.Visible = false;
            }
            catch (System.Exception ex)
            {
                GD.Print("Save Exception" + ex);
            }
        }

    }

    public static class ViewSettingsExtension{
        public static ViewSettings ViewSettings(this Node self){
            return self.GetTree().CurrentScene.GetNode<ViewSettings>("Settings");
        }
    }
}
