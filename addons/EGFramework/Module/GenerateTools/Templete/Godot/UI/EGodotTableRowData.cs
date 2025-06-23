using System;
using System.Collections.Generic;
using Godot;
using Tmds.Linux;

namespace EGFramework.UI{
    public partial class EGodotTableRowData : EGodotRowData
    {
        public Control Operate { get; set; }
        public Button Modify { get; set; }
        public Button Delete { get; set; }

        public EasyEvent<Dictionary<string, object>> OnModify { set; get; } = new EasyEvent<Dictionary<string, object>>();
        public EasyEvent OnDelete { set; get; } = new EasyEvent();

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
            Modify.Connect("pressed", Callable.From(OnEdit));
            Delete.Connect("pressed", Callable.From(OnDeleteSelf));
            this.CustomMinimumSize = new Vector2(0, 32);
        }
        public void OnEdit()
        {
            if (Data == null)
            {
                return;
            }
            OnModify.Invoke(Data);
        }

        public virtual void OnDataEdit(Dictionary<string, object> e)
        {
            foreach (var pair in e)
            {
                this.Data[pair.Key] = pair.Value;
            }
            // this.Data = e;
            this.RefreshData();
        }

        public override void RefreshData(Dictionary<string, object> data)
        {
            base.RefreshData(data);
            Operate.ToEnd();
        }
        public void OnDeleteSelf()
        {
            this.EGConfirm("Delete this data? this operate cannot be canceled.", e =>
            {
                if (e)
                {
                    OnDelete.Invoke();
                }
            }, "Delete");
        }

        public override void _ExitTree()
        {
            Modify.Disconnect("pressed", Callable.From(OnEdit));
            Delete.Disconnect("pressed", Callable.From(OnDeleteSelf));
            base._ExitTree();
        }
    }
}