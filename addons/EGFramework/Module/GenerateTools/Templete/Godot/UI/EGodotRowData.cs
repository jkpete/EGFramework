using Godot;
using System;
using System.Collections.Generic;

namespace EGFramework.UI
{

    public interface IEGodotRowData:IEGodotData
    {
        public void Init(Dictionary<string, object> data);
        public void RefreshData(Dictionary<string, object> data);
        public Dictionary<string, object> GetData();
    }
    public partial class EGodotRowData : PanelContainer, IEGFramework, IEGodotRowData
    {

        public Button ItemHover { get; set; }
        public ColorRect Line { get; set; }
        public ColorRect BackGround { get; set; }
        public HBoxContainer List { get; set; }

        protected Dictionary<string, object> Data { get; set; }
        protected bool IsInit { set; get; } = false;

        public Dictionary<string, object> GetData()
        {
            return this.Data;
        }


        public virtual void Init(Dictionary<string, object> data)
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
                    Text = kv.Value?.ToString(),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
                });
            }
            this.AddThemeStyleboxOverride("panel", new StyleBoxEmpty());
            IsInit = true;
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

        public void RefreshData()
        {
            this.RefreshData(this.Data);
        }
        public void SetBackgroundColor(Color color)
        {
            if (this.BackGround != null)
            {
                this.BackGround.Color = color;
            }
        }
        public void SetLineColor(Color color)
        {
            if (this.Line != null)
            {
                this.Line.Color = color;
            }
        }
    }
}
