using System;
using System.Collections.Generic;
using Godot;

namespace EGFramework.UI{
    public partial class EGodotTableRowData : Control,IEGFramework
    {
        public Button ItemHover { get; set; }
        public ColorRect Line { get; set; }
        public ColorRect BackGround { get; set; }
        public HBoxContainer List { get; set; }

        public Control Operate { get; set; }
        public Button Modify { get; set; }
        public Button Delete { get; set; }
        public string[] Data { get; set; }

        // private Action<Dictionary<string,string>> OnDataEdit;

        public void InitRowData(){

        }
        public void RefreshRow(){
            
        }
    }
}