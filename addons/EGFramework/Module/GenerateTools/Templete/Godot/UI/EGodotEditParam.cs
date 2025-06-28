using System;
using System.Collections.Generic;
using Godot;

namespace EGFramework.UI
{
    public partial class EGodotEditParam : EGodotParam, IEGFramework
    {
        public LineEdit ParamEdit { get; set; }
        public OptionButton ParamOption { get; set; }
        public CheckButton ParamCheck { get; set; }
        public List<CheckBox> ParamCheckList { get; set; }
        public Label ParamReadOnly { get; set; }
        public SpinBox ParamSpinBox { get; set; }
        public HSlider ParamSlider { get; set; }
        public Button ParamOperate { get; set; }
        private Type ValueType { set; get; }

        public override void Init(KeyValuePair<string, object> editValue)
        {
            base.Init(editValue);
            if (editValue.Key == "id" || editValue.Key == "ID" || editValue.Key == "Id")
            {
                this.ParamReadOnly = new Label();
                ParamReadOnly.Name = "ParamReadOnly";
                ParamReadOnly.SizeFlagsHorizontal = SizeFlags.ExpandFill;
                ParamReadOnly.Text = editValue.Value.ToString();
                this.AddChild(ParamReadOnly);
                return;
            }
            if (editValue.Value is string || editValue.Value is null)
            {
                this.ParamEdit = new LineEdit();
                ParamEdit.Name = "ParamEdit";
                ParamEdit.SizeFlagsHorizontal = SizeFlags.ExpandFill;
                ParamEdit.PlaceholderText = "Please input " + editValue.Key;
                this.AddChild(ParamEdit);
                ParamEdit.Text = (string)editValue.Value;
            }
            else if (editValue.Value is bool)
            {
                this.ParamCheck = new CheckButton();
                ParamCheck.Name = "ParamCheck";
                ParamCheck.SizeFlagsHorizontal = SizeFlags.ExpandFill;
                ParamCheck.Text = "";
                ParamCheck.ButtonPressed = (bool)editValue.Value;
                this.AddChild(ParamCheck);
            }
            else if (editValue.Value is IEGReadOnlyString)
            {
                this.ParamReadOnly = new Label();
                ParamReadOnly.Name = "ParamReadOnly";
                ParamReadOnly.SizeFlagsHorizontal = SizeFlags.ExpandFill;
                ParamReadOnly.Text = ((IEGReadOnlyString)editValue.Value).GetString();
                this.AddChild(ParamReadOnly);
            }
            else if (editValue.Value is EGSelectParam)
            {
                this.ParamOption = new OptionButton();
                ParamOption.Name = "ParamOption";
                ParamOption.SizeFlagsHorizontal = SizeFlags.ExpandFill;
                this.AddChild(ParamOption);
                foreach (KeyValuePair<int, string> selectOptions in ((EGSelectParam)editValue.Value).SelectList)
                {
                    this.ParamOption.AddItem(selectOptions.Value, selectOptions.Key);
                }
                this.ParamOption.Selected = this.ParamOption.GetItemIndex(((EGSelectParam)editValue.Value).SelectID);
            }
            else if (editValue.Value is int)
            {
                this.ParamSpinBox = new SpinBox();
                ParamSpinBox.Name = "ParamSpinBox";
                ParamSpinBox.SizeFlagsHorizontal = SizeFlags.ExpandFill;
                ParamSpinBox.MaxValue = int.MaxValue;
                ParamSpinBox.MinValue = int.MinValue;
                ParamSpinBox.Value = (int)editValue.Value;
                this.AddChild(ParamSpinBox);
                ValueType = typeof(int);
            }
            else if (editValue.Value is float)
            {
                this.ParamSpinBox = new SpinBox();
                ParamSpinBox.Name = "ParamSpinBox";
                ParamSpinBox.SizeFlagsHorizontal = SizeFlags.ExpandFill;
                ParamSpinBox.MaxValue = float.MaxValue;
                ParamSpinBox.MinValue = float.MinValue;
                ParamSpinBox.Value = (float)editValue.Value;
                ParamSpinBox.Step = 0.01f;
                this.AddChild(ParamSpinBox);
                ValueType = typeof(float);
            }
            else if (editValue.Value is double)
            {
                this.ParamSpinBox = new SpinBox();
                ParamSpinBox.Name = "ParamSpinBox";
                ParamSpinBox.SizeFlagsHorizontal = SizeFlags.ExpandFill;
                ParamSpinBox.MaxValue = double.MaxValue;
                ParamSpinBox.MinValue = double.MinValue;
                ParamSpinBox.Value = (double)editValue.Value;
                ParamSpinBox.Step = 0.0001f;
                this.AddChild(ParamSpinBox);
                ValueType = typeof(double);
            }
            else if (editValue.Value is EGRangeParam)
            {
                this.ParamSlider = new HSlider();
                ParamSlider.Name = "ParamSlider";
                ParamSlider.SizeFlagsHorizontal = SizeFlags.ExpandFill;
                EGRangeParam rangeParam = (EGRangeParam)editValue.Value;
                ParamSlider.MinValue = rangeParam.Min;
                ParamSlider.MaxValue = rangeParam.Max;
                ParamSlider.Step = rangeParam.Step;
                ParamSlider.Value = rangeParam.Value;
                this.AddChild(ParamSlider);
            }
            else if (editValue.Value is EGPathSelect)
            {
                EGPathSelect pathSelect = (EGPathSelect)editValue.Value;
                this.ParamReadOnly = new Label();
                ParamReadOnly.Name = "ParamReadOnly";
                ParamReadOnly.SizeFlagsHorizontal = SizeFlags.ExpandFill;
                ParamReadOnly.Text = pathSelect.Path;
                this.AddChild(ParamReadOnly);
                this.ParamOperate = new Button();
                ParamOperate.Name = "SelectBtn";
                ParamOperate.Text = "Select file";
                ParamOperate.SizeFlagsHorizontal = SizeFlags.ExpandFill;
                ParamOperate.Pressed += () =>
                {
                    this.EGFileOpen("res://", str =>
                    {
                        ParamReadOnly.Text = str;
                    });
                };
                // ParamOperate.Pressed +=
                this.AddChild(ParamOperate);
            }
        }

        public string GetKey()
        {
            return ParamValue.Key;
        }

        public object GetValue()
        {
            if (ParamEdit != null)
            {
                return ParamEdit.Text;
            }
            else if (ParamCheck != null)
            {
                return ParamCheck.ButtonPressed;
            }
            else if (ParamOption != null)
            {
                return ParamOption.Selected;
            }
            else if (ParamReadOnly != null)
            {
                return ParamReadOnly.Text;
            }
            else if (ParamSpinBox != null)
            {
                if (ValueType == typeof(int))
                {
                    return (int)ParamSpinBox.Value;
                }
                else if(ValueType == typeof(float))
                {
                    return (float)ParamSpinBox.Value;
                }
                return ParamSpinBox.Value;
            }
            else if (ParamSlider != null)
            {
                return ParamSlider.Value;
            }
            return null;
        }

        public override KeyValuePair<string, object> GetData()
        {
            return new KeyValuePair<string, object>(GetKey(), GetValue());
        }
        
        public override void RefreshData(KeyValuePair<string, object> data)
        {
            //this param cannot be Refreshed,please remove and recreate a new EGodotEditParam.
        }
    }
}