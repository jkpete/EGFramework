using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;

namespace EGFramework.UI{ 
    public interface IEGodotTable
    {
        
    }
    public partial class EGodotTable : BoxContainer, IEGFramework, IEGodotTable
    {
        public BoxContainer FunctionContainer { set; get; }
        public EGodotTableRowData Title { set; get; }
        public BoxContainer RowDataContainer { set; get; }
        public ScrollContainer RowDataScroll { set; get; }
        public BoxContainer PageContainer { set; get; }

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

        public void InitData<T>(IEnumerable<T> tableData) where T : new()
        {
            int count = tableData.Count();
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
            InitRowData(tableData.EGenerateDictionaryByGroup());
            InitPageMenu();
        }


        public virtual void InitFunctionMenu()
        {
            if (FunctionContainer == null)
            {
                FunctionContainer = this.CreateNode<BoxContainer>("FunctionContainer");
                FunctionContainer.Vertical = false;
            }
        }
        public void InitTitle(Dictionary<string, object> titleData)
        {
            if (Title == null)
            {
                Title = this.CreateNode<EGodotTableRowData>("TitleContainer");
                Title.Init(titleData);
            }
            else
            {
                Title.RefreshData(titleData);
            }
        }

        public void InitRowData(List<Dictionary<string, object>> rowDataList)
        {
            int dataPointer = 0;
            foreach (Dictionary<string, object> row in rowDataList)
            {
                EGodotTableRowData rowData = this.CreateNode<EGodotTableRowData>("row" + dataPointer);
                rowData.Init(row);
                dataPointer++;
            }
        }
        public void InitPageMenu()
        {
            if (PageContainer == null)
            {
                PageContainer = this.CreateNode<BoxContainer>("PageContainer");
                PageContainer.Vertical = false;
            }
        }

    }
    
    public static class EGGodotTableGenerator
    {
        // public static EGodotTable CreateTable(this Node self, string[][] tableStr, string tableName = "Table")
        // {
        //     VBoxContainer Table = new VBoxContainer();
        //     Table.Name = tableName;
        //     int dataPointer = 0;
        //     foreach (string[] s in tableStr)
        //     {
        //         Table.CreateRowData(s, "tableRowData" + dataPointer);
        //         dataPointer++;
        //     }
        //     self.AddChild(Table);
        //     return Table;
        // }
        // public static EGodotTable CreateTable<T>(this Node self, IEnumerable<T> tableData, string tableName = "ObjectTable", int limit = 0)
        // {
        //     EGodotTable Table = new EGodotTable();
        //     Table.Name = tableName;
        //     MemberInfo[] propertyNames = typeof(T).GetProperties();
        //     MemberInfo[] fieldNames = typeof(T).GetFields();
        //     MemberInfo[] memberInfos = propertyNames.Concat(fieldNames).ToArray();
        //     string[] propertyName = new string[memberInfos.Length];
        //     int dataPointer = 0;
        //     for (int i = 0; i < memberInfos.Length; i++)
        //     {
        //         propertyName[i] = memberInfos[i].Name;
        //     }
        //     Table.CreateRowData(propertyName, "Title");
        //     foreach (T t in tableData)
        //     {
        //         string[] s = t.GetType().GetProperties().Select(p => p.GetValue(t)?.ToString()).ToArray();
        //         string[] a = t.GetType().GetFields().Select(p => p.GetValue(t)?.ToString()).ToArray();
        //         string[] result = s.Concat(a).ToArray();
        //         Table.CreateRowData(result, "tableRowData" + dataPointer);
        //         dataPointer++;
        //     }
        //     self.AddChild(Table);
        //     return Table;
        // }
    }
    public class EGodotTablePageAdapter
    {
        public int DataLength { set; get; }
        public int CurrentPage { set; get; }
        public int MaxPage { set; get; }
        public int PageLimit { set; get; }

        public EasyEvent OnLoad = new EasyEvent();

        public EGodotTablePageAdapter(int dataLength, int pageLimit = 10)
        {
            this.DataLength = dataLength;
            this.PageLimit = pageLimit;
            this.MaxPage = dataLength / PageLimit + 1;
            this.CurrentPage = 1;
        }

        public void Reload(int dataLength, int pageLimit = 10)
        {
            this.DataLength = dataLength;
            this.PageLimit = pageLimit;
            this.MaxPage = dataLength / PageLimit + 1;
            this.CurrentPage = 1;
            OnLoad.Invoke();
        }
    }
}