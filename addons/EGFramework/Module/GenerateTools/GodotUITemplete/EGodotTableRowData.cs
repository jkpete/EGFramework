using System;
using System.Collections.Generic;
using Godot;

namespace EGFramework.UI{
    public partial class EGodotTableRowData : PanelContainer,IEGFramework
    {
        public Button ItemHover { get; set; }
        public ColorRect Line { get; set; }
        public ColorRect BackGround { get; set; }
        public HBoxContainer List { get; set; }

        public Control Operate { get; set; }
        public Button Modify { get; set; }
        public Button Delete { get; set; }

        private Dictionary<string,object> Data { get; set; }
        private Action<Dictionary<string,object>> OnDataEdit;

        public void InitRowData(Dictionary<string,object> data){
            this.Data = data;
            BackGround = new ColorRect();
            BackGround.Name = "BackGround";
            BackGround.Color = new Color(0.5f,0.5f,1f);
            BackGround.SizeFlagsHorizontal = SizeFlags.ExpandFill;
            BackGround.SizeFlagsVertical = SizeFlags.ExpandFill;
            this.AddChild(BackGround);
            List = new HBoxContainer();
            List.Name = "TableRow_"+Resource.GenerateSceneUniqueId();
            List.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            this.AddChild(List);
            Line = new ColorRect();
            Line.Name = "Line";
            Line.Color = new Color(0.5f,0.5f,0.5f);
            Line.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            Line.SizeFlagsVertical = Control.SizeFlags.ShrinkEnd;
            Line.CustomMinimumSize = new Vector2(0,1);
            this.AddChild(Line);
            foreach(KeyValuePair<string,object> kv in data){
                this.List.AddChild(new Label(){
                    Name = kv.Key,
                    Text = kv.Value.ToString(),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
                });
            }
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
            OnDataEdit = e => {

            };
            this.AddThemeStyleboxOverride("panel",new StyleBoxEmpty());
        }

        public void OnEdit(){
            // if(Data == null){
            //     return ;
            // }
            // this.EditParams(Data.GetModifyParams(),OnDataEdit,"修改");
        }

        public void OnDelete(){

        }
    }
}