using System.Collections.Generic;
using Godot;

namespace EGFramework.UI
{
    public partial class EGodotSaveTable : VBoxContainer, IEGFramework
    {
        public HBoxContainer FunctionContainer { set; get; }
        public HBoxContainer TitleContainer { set; get; }
        public VBoxContainer RowDataContainer { set; get; }
        public HBoxContainer PageContainer { set; get; }

        public IEGSaveData SaveData { set; get; }
        public Dictionary<string, string> TitleList { set; get; } = new Dictionary<string, string>();
        private EGodotTablePageAdapter PageAdapter { set; get; }
        private bool IsSearched { set; get; }

        /// <summary>
        /// The max data count for one page.
        /// </summary>
        /// <value></value>
        [Export]
        public int PageLimit { set; get; } = 10;
        /// <summary>
        /// Height mininum for RowDataContainer.
        /// </summary>
        /// <value></value>
        [Export]
        public int MinHeight { set; get; }

        public void InitData<T>(IEGSaveData saveData, string key) where T : new()
        {
            int count = saveData.GetDataCount(key);
            if (PageAdapter == null)
            {
                PageAdapter = new EGodotTablePageAdapter(count, PageLimit);
            }
            else
            {
                PageAdapter.Reload(count, PageLimit);
            }

        }
        public void InitReadOnlyData<T>(IEGSaveDataReadOnly saveData, string key) where T : new()
        {

        }
        
    }
}