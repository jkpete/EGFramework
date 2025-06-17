using Godot;
using System;
using System.Collections.Generic;

namespace EGFramework.UI
{
    public interface IEGRowData
    {
        public void InitRowData(Dictionary<string, object> data);
        public void RefreshData(Dictionary<string, object> data);
    }
    public partial class EGRowData : PanelContainer, IEGFramework,IEGRowData
    {

        public Button ItemHover { get; set; }
        public ColorRect Line { get; set; }
        public ColorRect BackGround { get; set; }
        public HBoxContainer List { get; set; }

        protected Dictionary<string, object> Data { get; set; }
        protected bool IsInit { set; get; } = false;
        
        public virtual void InitRowData(Dictionary<string, object> data)
        {
            if (IsInit)
            {
                this.Data = data;
                this.RefreshData(data);
                return; 
            }
            this.Data = data;
            BackGround = new ColorRect();
            BackGround.Name = "BackGround";
            BackGround.Color = new Color(0f, 0f, 0f, 0f);
            BackGround.SizeFlagsHorizontal = SizeFlags.ExpandFill;
            BackGround.SizeFlagsVertical = SizeFlags.ExpandFill;
            this.AddChild(BackGround);
            List = new HBoxContainer();
            List.Name = "TableRow_" + Resource.GenerateSceneUniqueId();
            List.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            this.AddChild(List);
            Line = new ColorRect();
            Line.Name = "Line";
            Line.Color = new Color(0f, 0f, 0f, 0f);
            Line.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            Line.SizeFlagsVertical = Control.SizeFlags.ShrinkEnd;
            Line.CustomMinimumSize = new Vector2(0, 1);
            this.AddChild(Line);
            foreach (KeyValuePair<string, object> kv in data)
            {
                this.List.AddChild(new Label()
                {
                    Name = kv.Key,
                    Text = kv.Value.ToString(),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
                });
            }
            this.AddThemeStyleboxOverride("panel", new StyleBoxEmpty());
        }

        public virtual void RefreshData(Dictionary<string, object> data)
        {
            this.List.ClearChildren<Label>();
            foreach (KeyValuePair<string, object> kv in data)
            {
                this.List.AddChild(new Label()
                {
                    Name = kv.Key,
                    Text = kv.Value.ToString(),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
                });
            }
        }
    }
}
