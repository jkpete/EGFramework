using System;
using System.Collections.Generic;
using Godot;

namespace EGFramework.UI
{
    public partial class EGodotEditDialog : ConfirmationDialog, IEGFramework
    {
        public VBoxContainer EditList { get; set; }
        public List<HBoxContainer> EditListItem { get; set; }
        public Label ErrorTips { get; set; }

        public EasyEvent<Dictionary<string, object>> OnEdit { set; get; } = new EasyEvent<Dictionary<string, object>>();
        private Dictionary<string, object> EditCache { set; get; } = new Dictionary<string, object>();
        private IUnRegister OnDataEdit { set; get; }

        public List<EGodotEditParam> ParamUIs { set; get; } = new List<EGodotEditParam>();
        private bool IsInit { set; get; } = false;

        const int DefaultWidth = 640;
        const int DefaultHeight = 320;

        public void InitDialog(Dictionary<string, object> data, Action<Dictionary<string, object>> onDataEdit, string title = "Edit Data", int width = DefaultWidth, int height = DefaultHeight)
        {
            if (!IsInit)
            {
                EditList = new VBoxContainer();
                EditList.Name = "EditList";
                EditList.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
                this.AddChild(EditList);
                this.Connect("confirmed", Callable.From(OnConfirm));
                this.Size = new Vector2I(width, height);
                IsInit = true;
            }
            this.EditList.ClearChildren();
            ParamUIs.Clear();
            this.Title = title;
            OnDataEdit = OnEdit.Register(onDataEdit);
            this.ErrorTips = new Label();
            ErrorTips.Name = "ErrorTips";
            ErrorTips.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            EditList.AddChild(ErrorTips);
            ErrorTips.Visible = false;
            foreach (KeyValuePair<string, object> param in data)
            {
                EGodotEditParam paramUI = new EGodotEditParam();
                this.EditList.AddChild(paramUI);
                paramUI.Init(param);
                ParamUIs.Add(paramUI);
            }
            this.PopupCentered();
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
    public static class EGodotEditDialogExtension
    {
        public static EGodotEditDialog EGEditDialog(this Node self, Dictionary<string, object> data, Action<Dictionary<string, object>> onDataEdit, string title = "Edit")
        {
            EGodotEditDialog editDialog = self.SingletoneNode<EGodotEditDialog>("FileDialog");
            editDialog.InitDialog(data, onDataEdit, title);
            editDialog.PopupCentered();
            return editDialog;
        }
    }
}