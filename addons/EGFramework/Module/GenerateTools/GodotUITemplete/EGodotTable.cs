using System.Collections.Generic;
using Godot;

namespace EGFramework.UI{
    public partial class EGodotTable : VBoxContainer,IEGFramework
    {
        public HBoxContainer FunctionContainer { set; get; }
        public HBoxContainer TitleContainer { set; get; }
        public VBoxContainer RowDataContainer { set; get; }
        public HBoxContainer PageContainer { set; get; }

        public IEGSaveData SaveData { set; get; }
        public Dictionary<string,string> TitleList { set; get; } = new Dictionary<string, string>();
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
        
        public void InitData<T>(IEGSaveData saveData,string key) where T: new(){
            int count = saveData.GetDataCount(key);
            if(PageAdapter == null){
                PageAdapter = new EGodotTablePageAdapter(count,PageLimit);
            }else{
                PageAdapter.Reload(count,PageLimit);
            }
            
        }
        public void InitReadOnlyData<T>(IEGSaveDataReadOnly saveData,string key)where T:new(){

        }
    }
    public class EGodotTablePageAdapter{
        public int DataLength { set; get; }
        public int CurrentPage { set; get; }
        public int MaxPage { set; get; }
        public int PageLimit { set; get; }

        public EasyEvent OnLoad = new EasyEvent();

        public EGodotTablePageAdapter(int dataLength,int pageLimit = 10){
            this.DataLength = dataLength;
            this.PageLimit = pageLimit;
            this.MaxPage = dataLength/PageLimit+1;
            this.CurrentPage = 1;
        }

        public void Reload(int dataLength,int pageLimit = 10){
            this.DataLength = dataLength;
            this.PageLimit = pageLimit;
            this.MaxPage = dataLength/PageLimit+1;
            this.CurrentPage = 1;
            OnLoad.Invoke();
        }
    }
}