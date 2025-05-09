using System;
using System.Collections.Generic;
using Godot;

namespace EGFramework.UI
{
    public partial class EGodotEditDialog : ConfirmationDialog,IEGFramework
    {
        public VBoxContainer EditList { get; set; }
        public List<HBoxContainer> EditListItem { get; set; }
        public Label ErrorTips { get; set; }

        public EasyEvent<Dictionary<string,object>> OnEdit { set; get; } = new EasyEvent<Dictionary<string, object>>();
        private Dictionary<string,object> EditCache { set; get; } = new Dictionary<string, object>();
        private IUnRegister OnDataEdit { set; get; }

        public List<EGodotEditParam> ParamUIs { set; get; } = new List<EGodotEditParam>();
        private bool IsInit { set; get; } = false;

        public void InitDialog(Dictionary<string,object> data,Action<Dictionary<string,object>> onDataEdit,string title = "Edit Data"){
            if(!IsInit){
                EditList = new VBoxContainer();
                EditList.Name = "EditList";
                EditList.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
                this.AddChild(EditList);
                this.ErrorTips = new Label();
                ErrorTips.Name = "ErrorTips";
                ErrorTips.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
                EditList.AddChild(ErrorTips);
                IsInit = true;
            }
            this.Title = title;
            ErrorTips.Visible = false;
            OnDataEdit = OnEdit.Register(onDataEdit);
            this.EditList.ClearChildren();
            ParamUIs.Clear();
            foreach(KeyValuePair<string,object> param in data){
                EGodotEditParam paramUI = new EGodotEditParam();
                this.EditList.AddChild(paramUI);
                paramUI.Init(param);
                ParamUIs.Add(paramUI);
            }
            this.Connect("confirmed",Callable.From(OnConfirm));
            this.PopupCentered();
        }

        public void OnConfirm(){
            EditCache.Clear();
            foreach(EGodotEditParam paramUI in ParamUIs){
                EditCache.Add(paramUI.GetKey(),paramUI.GetValue());
            }
            try
            {
                OnEdit.Invoke(EditCache);
                OnDataEdit.UnRegister();
                this.Visible = false;
            }catch(NullReferenceException){
                this.OnErrorTips("某项数据不能为空!");
            }catch(FormatException){
                this.OnErrorTips("某项数据格式不准确!");
            }catch (Exception e)
            {
                this.OnErrorTips(e.ToString());
                throw;
            }
        }

        public void OnErrorTips(string tips){
            ErrorTips.Visible = true;
            ErrorTips.Text = tips;
        }

        public void OnCancel(){
            OnDataEdit.UnRegister();
            this.Visible = false;
        } 
    }
}