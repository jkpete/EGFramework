using System.Collections.Generic;
using Godot;

namespace EGFramework.UI
{
    public partial class EGodotMenu : Control, IEGFramework
    {
        public Dictionary<string, Control> PageContainer { set; get; } = new Dictionary<string, Control>();

        public void RegisterPage<T>(string name,T page) where T : Control
        {
            if (PageContainer.ContainsKey(name))
            {
                PageContainer[name] = page;
            }
            else
            {
                PageContainer.Add(name,page);
            }
        }

        public void OpenPage<T>(string name,T page) where T : Control
        {
            page.Visible = true;
            
        }
    }
    
}