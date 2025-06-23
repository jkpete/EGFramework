using System.Collections.Generic;
using Godot;

namespace EGFramework.UI
{
    public partial class EGodotSaveTable : EGodotTable, IEGFramework
    {
        public IEGSaveData SaveData { set; get; }
        public Dictionary<string, string> TitleList { set; get; } = new Dictionary<string, string>();
    }
}