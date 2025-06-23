using System;
using Godot;

namespace EGFramework.UI {
    public partial class EGodotConfirmationDialog : ConfirmationDialog, IEGFramework
    {
        private EasyEventOnce<bool> OnConfirm { set; get; } = new EasyEventOnce<bool>();
        private bool IsInit { set; get; } = false;
        public void Init(Action<bool> callback)
        {
            OnConfirm.Register(callback);
            if (!IsInit)
            {
                this.Confirmed += () => OnConfirm.Invoke(true);
                this.Canceled += () => OnConfirm.Invoke(false);
                IsInit = true;
            }
        }
    }
}
