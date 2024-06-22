using Godot;
using System.Collections.Generic;

namespace EGFramework.Examples.ModbusDebugTool{
    public partial class ViewMenu : Node,IEGFramework
    {
        [Export] public PackedScene StorageItem = GD.Load<PackedScene>("res://Example/ModbusDebugTool/Component/modbus_item.tscn");
        public PopupMenu MenuAdd;
        public PopupMenu MenuSettings;
        public GridContainer ModbusItemContainer;
        public ViewEdit Edit;

        public EasyEvent<ModbusRTU_Response> OnModbusRTUGet { set; get; } = new EasyEvent<ModbusRTU_Response>();

        public override void _Ready()
        {
            MenuAdd = this.GetNode<PopupMenu>("MenuBar/添加");
            MenuSettings = this.GetNode<PopupMenu>("MenuBar/设置");
            ModbusItemContainer = this.GetNode<GridContainer>("Scroll/ModbusList");
            Edit = this.GetNode<ViewEdit>("Edit");
            Edit.Visible = false;
            this.EGEnabledProtocolTools();
            DataModbus dataModbus = this.EGSave().GetDataByFile<DataModbus>();
            if (dataModbus == null)
            {
                dataModbus = new DataModbus();
            }
            this.EGRegisterObject(dataModbus);
            RefreshSaveData();
            this.EGRegisterMessageEvent<ModbusRTU_Response>((e,sender,ProtocolType)=>{
                if(ProtocolType == ProtocolType.SerialPort){
                    this.AppendMessage("【接收-"+sender+"】 "+e.SourceData.ToStringByHex());
                    OnModbusRTUGet.Invoke(e);
                }
            });
            this.EGOnMessage<ModbusRTU_Response>();
        }

        public void OpenEdit(int AddMenuId){
            GD.Print(MenuAdd.GetItemText(AddMenuId));
            switch(MenuAdd.GetItemText(AddMenuId)){
                case "保持寄存器":
                    this.OnNewEdit(ModbusRegisterType.HoldingRegister);
                    break;
                default:
                    break;
            }
        }

        public void OpenSettings(int OtherId){
            switch(MenuSettings.GetItemText(OtherId)){
                case "查看报文":
                    this.ViewMessage().Visible = true;
                    break;
                case "设置":
                    this.ViewSettings().Visible = true;
                    break;
                default:
                    break;
            }
        }

        public void ReadAll(int ReadMenuId){
            GD.Print(MenuAdd.GetItemText(ReadMenuId));
        }

        public void RefreshSaveData(){
            ModbusItemContainer.ClearChildren();
            if(this.EGGetObject<DataModbus>() != null){
                foreach(DataModbusItem item in this.EGGetObject<DataModbus>().Items.Values){
                    ViewModbusItem viewItem = StorageItem.Instantiate<ViewModbusItem>();
                    ModbusItemContainer.AddChild(viewItem);
                    viewItem.LoadData(item);
                }
            }
        }
    }
}
