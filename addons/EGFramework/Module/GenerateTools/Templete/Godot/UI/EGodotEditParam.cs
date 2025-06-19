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
    public partial class EGodotEditParam : HBoxContainer, IEGFramework
    {
        public Label ParamName { get; set; }
        public LineEdit ParamEdit { get; set; }
        public OptionButton ParamOption { get; set; }
        public CheckButton ParamCheck { get; set; }
        public List<CheckBox> ParamCheckList { get; set; }
        public Label ParamReadOnly { get; set; }
        public SpinBox ParamSpinBox { get; set; }
        public HSlider ParamSlider { get; set; }

        public KeyValuePair<string, object> EditValue { get; set; }

        public void Init(KeyValuePair<string, object> editValue)
        {
            EditValue = editValue;
            this.ParamName = new Label();
            ParamName.Name = "ParamName";
            ParamName.Text = editValue.Key;
            ParamName.SizeFlagsHorizontal = SizeFlags.ExpandFill;
            this.AddChild(ParamName);
            ParamName.Text = editValue.Key;
            if (editValue.Value is string)
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
                ParamSpinBox.Value = (int)editValue.Value;
                ParamSpinBox.MaxValue = int.MaxValue;
                ParamSpinBox.MinValue = int.MinValue;
                this.AddChild(ParamSpinBox);
            }
            else if (editValue.Value is float)
            {
                this.ParamSpinBox = new SpinBox();
                ParamSpinBox.Name = "ParamSpinBox";
                ParamSpinBox.SizeFlagsHorizontal = SizeFlags.ExpandFill;
                ParamSpinBox.Value = (float)editValue.Value;
                ParamSpinBox.MaxValue = float.MaxValue;
                ParamSpinBox.MinValue = float.MinValue;
                ParamSpinBox.Step = 0.01f;
                this.AddChild(ParamSpinBox);
            }
            else if (editValue.Value is double)
            {
                this.ParamSpinBox = new SpinBox();
                ParamSpinBox.Name = "ParamSpinBox";
                ParamSpinBox.SizeFlagsHorizontal = SizeFlags.ExpandFill;
                ParamSpinBox.Value = (double)editValue.Value;
                ParamSpinBox.MaxValue = double.MaxValue;
                ParamSpinBox.MinValue = double.MinValue;
                ParamSpinBox.Step = 0.0001f;
                this.AddChild(ParamSpinBox);
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
        }

        public string GetKey()
        {
            return EditValue.Key;
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
                return ParamSpinBox.Value;
            }
            else if (ParamSlider != null)
            {
                return ParamSlider.Value;
            }
            return null;
        }

    }
}