using System.Collections.Generic;
using System.Linq;

namespace EGFramework.UI
{
    public partial class EGodotSaveTable : EGodotTable, IEGFramework
    {
        public IEGSaveData SaveData { set; get; }
        public Dictionary<string, string> TitleList { set; get; } = new Dictionary<string, string>();
        public string CurrentDataKey { set; get; }

        public EasyEvent QueryPage { set; get; } = new EasyEvent();

        public EasyEvent SearchKey { set; get; } = new EasyEvent();

        public void InitSaveData<TSaveData>(IEGSaveData eGSaveData) where TSaveData : IEGSaveData
        {
            this.SaveData = eGSaveData;

        }
        public void InitData<T>(string key) where T : new()
        {
            int count = SaveData.GetDataCount(key);
            if (PageAdapter == null)
            {
                PageAdapter = new EGodotTablePageAdapter(count, PageLimit);
            }
            else
            {
                PageAdapter.Reload(count, PageLimit);
            }
            this.Vertical = true;
            CurrentDataKey = key;
            EmptyData = new T().EGenerateDictiontaryByObject();
            QueryPage.Register(() => QueryPageData<T>());
            SearchKey.Register(() => SearchDataByKeyword<T>());
            TableName = typeof(T).Name;
            TitleData = typeof(T).EGenerateDictiontaryByType();
            InitFunctionMenu();
            InitTitle(TitleData);
            InitRowData(null);
            InitPageMenu();
        }


        public void QueryPageData<T>() where T : new()
        {
            if (PageAdapter.CurrentPage <= 1)
            {
                PageAdapter.CurrentPage = 1;
            }

            TableData = SaveData.GetPage<T>(CurrentDataKey, PageAdapter.CurrentPage, PageAdapter.PageLimit).EGenerateDictionaryByGroup();
        }

        public void SearchDataByKeyword<T>() where T : new()
        {
            string fieldName = FieldSelect.Text;
            string keyWords = SearchEdit.Text;
            TableData = SaveData.FindData<T>(CurrentDataKey, fieldName, keyWords).EGenerateDictionaryByGroup();
        }

        public override void OnAddData(Dictionary<string, object> data)
        {
            // base.OnAddData(data);
            SaveData.AddData(CurrentDataKey, data);
        }

        public void ModifyData(Dictionary<string, object> eData)
        {
            string primaryKey = "";
            if (eData.ContainsKey("ID")) primaryKey = "ID";
            if (eData.ContainsKey("id")) primaryKey = "id";
            if (eData.ContainsKey("Id")) primaryKey = "Id";
            if (primaryKey == "")
            {
                this.EGAlert("Parmary key 'id' not defined!", "Error");
                return;
            }
            SaveData.UpdateData(CurrentDataKey, eData, eData[primaryKey]);
            InitPageData();
        }


        public override void InitPageData()
        {
            RowDataContainer.ClearChildren();
            QueryPage.Invoke();
            int pointer = 0;
            foreach (Dictionary<string, object> data in TableData)
            {
                EGodotTableRowData rowData = RowDataContainer.CreateNode<EGodotTableRowData>("row" + pointer);
                rowData.Init(data);
                rowData.OnModify.Register(eData =>
                {
                    this.EGEditDialog(eData, ModifyData, "Modify");
                });
                rowData.OnDelete.Register(() =>
                {
                    string primaryKey = "";
                    if (rowData.GetData().ContainsKey("ID")) primaryKey = "ID";
                    if (rowData.GetData().ContainsKey("id")) primaryKey = "id";
                    if (rowData.GetData().ContainsKey("Id")) primaryKey = "Id";
                    if (primaryKey == "")
                    {
                        this.EGAlert("Parmary key 'id' not defined!", "Error");
                        return;
                    }
                    int remove_count = SaveData.RemoveData(CurrentDataKey, rowData.GetData()[primaryKey]);
                    PageAdapter.DataLength -= remove_count;
                    PageAdapter.Reload(PageAdapter.DataLength, PageLimit);
                    InitPageData();
                    OnPageChanged.Invoke();
                });
            }
            ResetSearch();
            //base.InitPageData();
        }

        public override void ExecuteSearch()
        {
            RowDataContainer.ClearChildren();
            string fieldName = FieldSelect.Text;
            string keyWords = SearchEdit.Text;
            SearchKey.Invoke();
            int dataPointer = 0;
            foreach (Dictionary<string, object> data in TableData)
            {
                EGodotTableRowData rowData = RowDataContainer.CreateNode<EGodotTableRowData>("row" + dataPointer);
                dataPointer++;
                rowData.Init(data);
                rowData.OnModify.Register(eData =>
                {
                    this.EGEditDialog(eData, ModifyData, "Modify");
                });
                rowData.OnDelete.Register(() =>
                {
                    string primaryKey = "";
                    if (rowData.GetData().ContainsKey("ID")) primaryKey = "ID";
                    if (rowData.GetData().ContainsKey("id")) primaryKey = "id";
                    if (rowData.GetData().ContainsKey("Id")) primaryKey = "Id";
                    if (primaryKey == "")
                    {
                        this.EGAlert("Parmary key 'id' not defined!", "Error");
                        return;
                    }
                    int remove_count = SaveData.RemoveData(CurrentDataKey, rowData.GetData()[primaryKey]);
                    PageAdapter.DataLength -= remove_count;
                    PageAdapter.Reload(PageAdapter.DataLength, PageLimit);
                    InitPageData();
                    OnPageChanged.Invoke();
                });
            }
        }
    }
}