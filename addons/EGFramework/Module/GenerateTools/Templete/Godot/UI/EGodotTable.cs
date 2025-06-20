using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Godot;

namespace EGFramework.UI{ 
    public interface IEGodotTable
    {
        
    }
    public partial class EGodotTable : BoxContainer, IEGFramework, IEGodotTable
    {
        public BoxContainer FunctionContainer { set; get; }
        public EGodotRowData Title { set; get; }
        public BoxContainer RowDataContainer { set; get; }
        public ScrollContainer RowDataScroll { set; get; }
        public BoxContainer PageContainer { set; get; }

        public IEGSaveData SaveData { set; get; }
        public Dictionary<string, string> TitleList { set; get; } = new Dictionary<string, string>();
        private EGodotTablePageAdapter PageAdapter { set; get; }
        private bool IsSearched { set; get; }

        public Color MainColor { set; get; } = new Color();
        public Color MinorColor { set; get; } = new Color();

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
                Title = this.CreateNode<EGodotRowData>("TitleContainer");
                titleData.Add("Operate", "");
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
                rowData.OnModify.Register(data=>
                {
                    this.EGEditDialog(data,rowData.OnDataEdit,"Modify");
                });
                dataPointer++;
            }
        }
        public void InitPageMenu()
        {
            if (PageContainer == null)
            {
                PageContainer = this.CreateNode<BoxContainer>("PageContainer");
                PageContainer.Vertical = false;
                PageContainer.Alignment = AlignmentMode.End;


                Label labelCount = PageContainer.CreateNode<Label>("to");
                labelCount.Text = "Data count : "+PageAdapter.DataLength;

                Control empty1 = PageContainer.CreateNode<Control>("empty1");
                empty1.CustomMinimumSize = new Vector2(32, 0);

                Button firstPage = PageContainer.CreateNode<Button>("firstPage");
                firstPage.Text = "<<";

                Button lastPage = PageContainer.CreateNode<Button>("lastPage");
                lastPage.Text = "<";

                Label currentPage = PageContainer.CreateNode<Label>("currenLabel");
                currentPage.Text = PageAdapter.CurrentPage.ToString();
                
                Button nextPage = PageContainer.CreateNode<Button>("next");
                nextPage.Text = ">";
                
                Button endPage = PageContainer.CreateNode<Button>("firstPage");
                endPage.Text = ">>";

                Control empty2 = PageContainer.CreateNode<Control>("empty2");
                empty2.CustomMinimumSize = new Vector2(32, 0);

                Label labelTo = PageContainer.CreateNode<Label>("to");
                labelTo.Text = "To";
                SpinBox inputPage = PageContainer.CreateNode<SpinBox>("pageEdit");
                inputPage.SetSize(new Vector2(120, 60));
                inputPage.MinValue = 0;
                inputPage.MaxValue = PageAdapter.MaxPage;
                inputPage.Alignment = HorizontalAlignment.Center;
                Label labelPage = PageContainer.CreateNode<Label>("page");
                labelPage.Text = "page";

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