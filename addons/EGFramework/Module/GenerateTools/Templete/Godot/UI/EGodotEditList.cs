using Godot;
using System;
using System.Collections.Generic;

namespace EGFramework.UI
{
    public partial class EGodotEditList : VBoxContainer
    {
        public Label Title { set; get; }
        public HBoxContainer OperateGroup { set; get; }
        public VBoxContainer EditList { get; set; }
        public Button ConfirmButton { set; get; }
        public Button CancelButton { set; get; }
        public List<HBoxContainer> EditListItem { get; set; }
        public Label ErrorTips { get; set; }

        public EasyEvent<Dictionary<string, object>> OnEdit { set; get; } = new EasyEvent<Dictionary<string, object>>();
        private Dictionary<string, object> EditCache { set; get; } = new Dictionary<string, object>();
        private IUnRegister OnDataEdit { set; get; }

        public List<EGodotEditParam> ParamUIs { set; get; } = new List<EGodotEditParam>();
        private bool IsInit { set; get; } = false;

        const int DefaultWidth = 640;
        const int DefaultHeight = 320;

        public void InitList(Dictionary<string, object> data, Action<Dictionary<string, object>> onDataEdit, string title = "Edit Data")
        {
            if (!IsInit)
            {
                Title = new Label();
                Title.Name = "Title";
                Title.HorizontalAlignment = HorizontalAlignment.Center;
                this.AddChild(Title);
                this.Name = "EditList";
                this.ErrorTips = new Label();
                ErrorTips.Name = "ErrorTips";
                this.AddChild(ErrorTips);
                ErrorTips.Visible = false;

                EditList = new VBoxContainer();
                EditList.Name = "EditContainer";
                EditList.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
                this.AddChild(EditList);

                this.OperateGroup = new HBoxContainer();
                this.OperateGroup.Name = "OperateGroup";
                this.AddChild(OperateGroup);

                this.ConfirmButton = new Button();
                this.ConfirmButton.Name = "ConfirmButton";
                this.ConfirmButton.Text = "OK";
                this.ConfirmButton.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
                this.ConfirmButton.Connect("pressed", Callable.From(OnConfirm));
                this.CancelButton = new Button();
                this.CancelButton.Name = "CancelButton";
                this.CancelButton.Text = "Cancel";
                this.CancelButton.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
                this.OperateGroup.AddChild(ConfirmButton);
                this.OperateGroup.AddChild(CancelButton);

                IsInit = true;
            }
           



            EditList.ClearChildren();
            ParamUIs.Clear();
            this.Title.Text = title;
            OnDataEdit = OnEdit.Register(onDataEdit);
            foreach (KeyValuePair<string, object> param in data)
            {
                EGodotEditParam paramUI = new EGodotEditParam();
                EditList.AddChild(paramUI);
                paramUI.Init(param);
                ParamUIs.Add(paramUI);
            }


        }

        public void OnConfirm()
        {
            EditCache.Clear();
            foreach (EGodotEditParam paramUI in ParamUIs)
            {
                EditCache.Add(paramUI.GetKey(), paramUI.GetValue());
            }
            try
            {
                OnEdit.Invoke(EditCache);
                OnDataEdit.UnRegister();
                this.Visible = false;
            }
            catch (NullReferenceException)
            {
                this.OnErrorTips("某项数据不能为空!");
            }
            catch (FormatException)
            {
                this.OnErrorTips("某项数据格式不准确!");
            }
            catch (Exception e)
            {
                this.OnErrorTips(e.ToString());
                throw;
            }
        }

        public void OnErrorTips(string tips)
        {
            ErrorTips.Visible = true;
            ErrorTips.Text = tips;
        }

        public void OnCancel()
        {
            OnDataEdit.UnRegister();
            this.Visible = false;
        }
    }
}
