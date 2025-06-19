using System;
using System.Collections.Generic;
using Godot;

namespace EGFramework.UI{
    public partial class EGodotTableRowData : EGodotRowData
    {
        public Control Operate { get; set; }
        public Button Modify { get; set; }
        public Button Delete { get; set; }

        public override void Init(Dictionary<string, object> data)
        {
            base.Init(data);
            Operate = new HBoxContainer();
            Operate.Name = "Operate";
            Operate.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            List.AddChild(Operate);
            Modify = new Button();
            Modify.Name = "Modify";
            Modify.Text = "Modify";
            Modify.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            Operate.AddChild(Modify);
            Delete = new Button();
            Delete.Name = "Delete";
            Delete.Text = "Delete";
            Delete.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            Operate.AddChild(Delete);
            Modify.Connect("pressed",Callable.From(OnEdit));
            Delete.Connect("pressed",Callable.From(OnDelete));
        }
        public void OnEdit(){
            if(Data == null){
                return ;
            }
            this.EGEditDialog(Data,OnDataEdit,"修改");
        }

        public virtual void OnDataEdit(Dictionary<string, object> e)
        {
            this.Data = e;
            this.RefreshData();
        }

        public override void RefreshData(Dictionary<string, object> data)
        {
            base.RefreshData(data);
            Operate.ToEnd();
        }

        public void OnDelete()
        {

        }
    }
}