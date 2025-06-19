using System.Collections.Generic;
using Godot;

namespace EGFramework.UI
{
    public interface IEGodotParam : IEGodotData
    {
        public void Init(KeyValuePair<string, object> data);
        public void RefreshData(KeyValuePair<string, object> data);
        public KeyValuePair<string, object> GetData();
    }
    public abstract partial class EGodotParam : BoxContainer, IEGFramework, IEGodotParam
    {
        public Label ParamName { get; set; }
        public KeyValuePair<string, object> ParamValue { get; set; }

        public virtual void Init(KeyValuePair<string, object> paramValue)
        {
            ParamValue = paramValue;
            this.ParamName = new Label();
            ParamName.Name = "ParamName";
            ParamName.Text = ParamValue.Key;
            ParamName.SizeFlagsHorizontal = SizeFlags.ExpandFill;
            this.AddChild(ParamName);
        }
        public void RefreshData()
        {
            this.RefreshData(this.ParamValue);
        }
        public abstract KeyValuePair<string, object> GetData();

        public abstract void RefreshData(KeyValuePair<string, object> data);
    }
}