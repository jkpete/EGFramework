using System.Collections.Generic;
using System.Linq;
using Godot;

namespace EGFramework.UI
{
    public partial class EGodotSaveTable : EGodotTable, IEGFramework
    {
        public IEGSaveData SaveData { set; get; }
        public Dictionary<string, string> TitleList { set; get; } = new Dictionary<string, string>();

        public void InitSaveData<TSaveData>(IEGSaveData eGSaveData) where TSaveData : IEGSaveData
        {
            this.SaveData = eGSaveData;

        }
        public void InitData<T>(string key) where T : new()
        {
            int count = SaveData.GetAll<T>(key).Count();
            if (PageAdapter == null)
            {
                PageAdapter = new EGodotTablePageAdapter(count, PageLimit);
            }
            else
            {
                PageAdapter.Reload(count, PageLimit);
            }
            this.Vertical = true;
            InitFunctionMenu();
            InitTitle(typeof(T).EGenerateDictiontaryByType());
            InitRowData(SaveData.GetAll<T>(key).EGenerateDictionaryByGroup());
            InitPageMenu();
        }
    }
}