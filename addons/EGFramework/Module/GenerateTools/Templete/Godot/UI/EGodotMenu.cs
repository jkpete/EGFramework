using System.Collections.Generic;
using Godot;

namespace EGFramework.UI
{
    public partial class EGodotMenu : Control, IEGFramework
    {
        public IOCContainer PageContainer { set; get; }

        public void RegisterPage<T>(T page) where T : Control
        {
            PageContainer.Register(page);
        }

        public void OpenPage<T>(T page) where T : Control
        {
            page.Visible = true;
            
        }
    }
    
}