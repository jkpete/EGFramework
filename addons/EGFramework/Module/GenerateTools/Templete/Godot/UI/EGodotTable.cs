using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Godot;

namespace EGFramework.UI
{
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

        public Color MainColor { set; get; } = new Color();
        public Color MinorColor { set; get; } = new Color();

        protected EGodotTablePageAdapter PageAdapter { set; get; }
        protected bool IsSearched { set; get; }
        protected EasyEvent OnPageChanged { set; get; } = new EasyEvent();
        protected IUnRegister PageChangedRealease { set; get; }

        protected List<Dictionary<string, object>> TableData { set; get; }
        protected Dictionary<string, object> EmptyData { set; get; }
        protected Dictionary<string, object> TitleData { set; get; } = new Dictionary<string, object>();

        protected EasyEvent<Dictionary<string, object>> AddData { set; get; } = new EasyEvent<Dictionary<string, object>>();

        public Vector2 MinimumFunctionButtonSize = new Vector2(60, 0);

        public string TableName { set; get; } = "-";

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
            EmptyData = typeof(T).EGenerateEmptyDictiontaryByType();
            TitleData = typeof(T).EGenerateDictiontaryByType();
            TableName = typeof(T).Name;
            InitFunctionMenu();
            InitTitle(TitleData);
            InitRowData(tableData.EGenerateDictionaryByGroup());
            InitPageMenu();
        }

        public virtual void OnAddData(Dictionary<string, object> data)
        {
            GD.Print("Add : " + data["Name"]);
            string primaryKey = data.EGetDefaultPrimaryKey();
            if (primaryKey != "")
            {
                data[primaryKey] = TableData.Count.ToString();
            }
            TableData.Add(new Dictionary<string, object>(data));
            PageAdapter.DataLength++;
            PageAdapter.Reload(PageAdapter.DataLength, PageLimit);
            InitPageData();
            OnPageChanged.Invoke();
        }

        public virtual void OnOutputFile(string path)
        {
            GD.Print("File has been Saved at " + path);
            EGCsvSave eGCsvSave = new EGCsvSave();
            eGCsvSave.InitSave(path);
            eGCsvSave.AddGroup("", TableData);
            OS.ShellOpen(path);
        }

        public virtual void OnInputFile(string path)
        {

        }

        protected OptionButton FieldSelect { set; get; }
        protected LineEdit SearchEdit { set; get; }
        public virtual void InitFunctionMenu()
        {
            if (FunctionContainer == null)
            {
                FunctionContainer = this.CreateNode<BoxContainer>("FunctionContainer");
                FunctionContainer.Vertical = false;

                Button add = FunctionContainer.CreateNode<Button>("add");
                add.Text = "Add";
                add.Connect("pressed", Callable.From(() => this.EGEditDialog(EmptyData, OnAddData, "Add")));
                add.FocusMode = FocusModeEnum.None;
                add.CustomMinimumSize = MinimumFunctionButtonSize;

                Button refresh = FunctionContainer.CreateNode<Button>("refresh");
                refresh.Text = "Refresh";
                refresh.Connect("pressed", Callable.From(InitPageData));
                refresh.FocusMode = FocusModeEnum.None;
                refresh.CustomMinimumSize = MinimumFunctionButtonSize;

                Button output = FunctionContainer.CreateNode<Button>("output");
                output.Text = "Output";
                output.Connect("pressed", Callable.From(() => this.EGFileSave(TableName + ".csv", OnOutputFile)));
                output.FocusMode = FocusModeEnum.None;
                output.CustomMinimumSize = MinimumFunctionButtonSize;

                Button input = FunctionContainer.CreateNode<Button>("input");
                input.Text = "Input";
                input.FocusMode = FocusModeEnum.None;
                input.CustomMinimumSize = MinimumFunctionButtonSize;

                FieldSelect = FunctionContainer.CreateNode<OptionButton>("fieldSelect");
                FieldSelect.FocusMode = FocusModeEnum.None;
                FieldSelect.SizeFlagsHorizontal = SizeFlags.Expand | SizeFlags.ShrinkEnd;
                FieldSelect.CustomMinimumSize = MinimumFunctionButtonSize;
                foreach (string titleParam in TitleData.Keys)
                {
                    FieldSelect.AddItem(titleParam);
                }


                SearchEdit = FunctionContainer.CreateNode<LineEdit>("searchEdit");
                SearchEdit.PlaceholderText = "PlaceholderSearch";
                SearchEdit.SizeFlagsHorizontal = SizeFlags.ShrinkEnd;
                SearchEdit.CustomMinimumSize = new Vector2(MinimumFunctionButtonSize.X * 2, MinimumFunctionButtonSize.Y);

                Button search = FunctionContainer.CreateNode<Button>("search");
                search.Text = "Search";
                search.FocusMode = FocusModeEnum.None;
                search.SizeFlagsHorizontal = SizeFlags.ShrinkEnd;
                search.CustomMinimumSize = MinimumFunctionButtonSize;
                search.Connect("pressed", Callable.From(Search));

                Button reset = FunctionContainer.CreateNode<Button>("reset");
                reset.Text = "Reset";
                reset.FocusMode = FocusModeEnum.None;
                reset.SizeFlagsHorizontal = SizeFlags.ShrinkEnd;
                reset.CustomMinimumSize = MinimumFunctionButtonSize;
                reset.Connect("pressed", Callable.From(ResetSearch));
            }
        }

        public virtual void InitTitle(Dictionary<string, object> titleData)
        {
            if (Title == null)
            {
                Title = this.CreateNode<EGodotRowData>("TitleContainer");
                titleData.Add("Operate", "Operate");
                Title.Init(titleData);
            }
            else
            {
                Title.RefreshData(titleData);
            }
        }

        public void InitRowData(List<Dictionary<string, object>> rowDataList)
        {
            RowDataScroll = this.CreateNode<ScrollContainer>("Scroll");
            RowDataScroll.SizeFlagsVertical = SizeFlags.ExpandFill;

            RowDataContainer = RowDataScroll.CreateNode<BoxContainer>("RowData");
            RowDataContainer.SizeFlagsHorizontal = SizeFlags.ExpandFill;
            RowDataContainer.Vertical = true;

            this.TableData = rowDataList;
            ToFirstPage();
            InitPageData();
        }

        public virtual void InitPageData()
        {
            if (PageAdapter.CurrentPage <= 1)
            {
                PageAdapter.CurrentPage = 1;
            }
            int dataPointer = (PageAdapter.CurrentPage - 1) * PageAdapter.PageLimit;
            int dataEndPointer = dataPointer + PageAdapter.PageLimit;
            if (dataEndPointer >= PageAdapter.DataLength)
            {
                dataEndPointer = PageAdapter.DataLength;
            }
            RowDataContainer.ClearChildren();
            for (int dataId = dataPointer; dataId < dataEndPointer; dataId++)
            {
                EGodotTableRowData rowData = RowDataContainer.CreateNode<EGodotTableRowData>("row" + dataPointer);
                rowData.Init(TableData[dataId]);
                rowData.OnModify.Register(data =>
                {
                    this.EGEditDialog(data, rowData.OnDataEdit, "Modify");
                });
                rowData.OnDelete.Register(() =>
                {
                    GD.Print("Delete : " + rowData.GetData()["Name"]);
                    this.TableData.Remove(rowData.GetData());
                    PageAdapter.DataLength--;
                    PageAdapter.Reload(PageAdapter.DataLength, PageLimit);
                    InitPageData();
                    OnPageChanged.Invoke();
                });
            }
            ResetSearch();
        }

        public void InitPageMenu()
        {
            if (PageContainer == null)
            {
                PageContainer = this.CreateNode<BoxContainer>("PageContainer");
                PageContainer.Vertical = false;
                PageContainer.Alignment = AlignmentMode.End;


                Label labelCount = PageContainer.CreateNode<Label>("to");
                labelCount.Text = Tr("Data") + " " + Tr("Count") +" : " + PageAdapter.DataLength + " "+Tr("Page")+" : " + PageAdapter.CurrentPage;

                Control empty1 = PageContainer.CreateNode<Control>("empty1");
                empty1.CustomMinimumSize = new Vector2(32, 0);

                Button firstPage = PageContainer.CreateNode<Button>("firstPage");
                firstPage.Text = "<<";
                firstPage.Connect("pressed", Callable.From(ToFirstPage));
                firstPage.FocusMode = FocusModeEnum.None;

                Button lastPage = PageContainer.CreateNode<Button>("lastPage");
                lastPage.Text = "<";
                lastPage.Connect("pressed", Callable.From(LastPage));
                lastPage.FocusMode = FocusModeEnum.None;

                Label currentPage = PageContainer.CreateNode<Label>("currenLabel");
                currentPage.Text = PageAdapter.CurrentPage.ToString();


                Button nextPage = PageContainer.CreateNode<Button>("next");
                nextPage.Text = ">";
                nextPage.Connect("pressed", Callable.From(NextPage));
                nextPage.FocusMode = FocusModeEnum.None;

                Button endPage = PageContainer.CreateNode<Button>("firstPage");
                endPage.Text = ">>";
                endPage.Connect("pressed", Callable.From(ToEndPage));
                endPage.FocusMode = FocusModeEnum.None;

                Control empty2 = PageContainer.CreateNode<Control>("empty2");
                empty2.CustomMinimumSize = new Vector2(32, 0);

                Label labelTo = PageContainer.CreateNode<Label>("to");
                labelTo.Text = "To";
                SpinBox inputPage = PageContainer.CreateNode<SpinBox>("pageEdit");
                inputPage.SetSize(new Vector2(120, 60));
                inputPage.MinValue = 1;
                inputPage.MaxValue = PageAdapter.MaxPage;
                inputPage.Alignment = HorizontalAlignment.Center;
                inputPage.Connect("value_changed", Callable.From<int>(ToPage));

                Label labelPage = PageContainer.CreateNode<Label>("page");
                labelPage.Text = "Page";
                firstPage.Disabled = true;
                lastPage.Disabled = true;

                PageChangedRealease = this.OnPageChanged.Register(() =>
                {
                    labelCount.Text = Tr("Data") + " " + Tr("Count") +" : " + PageAdapter.DataLength + " "+Tr("Page")+" : " + PageAdapter.CurrentPage;
                    currentPage.Text = PageAdapter.CurrentPage.ToString();
                    if (PageAdapter.IsFirstPage())
                    {
                        firstPage.Disabled = true;
                        lastPage.Disabled = true;
                    }
                    else
                    {
                        firstPage.Disabled = false;
                        lastPage.Disabled = false;
                    }
                    if (PageAdapter.IsLastPage())
                    {
                        endPage.Disabled = true;
                        nextPage.Disabled = true;
                    }
                    else
                    {
                        endPage.Disabled = false;
                        nextPage.Disabled = false;
                    }
                    InitPageData();
                });
            }
        }

        public void ToFirstPage()
        {
            this.PageAdapter.CurrentPage = 1;
            OnPageChanged.Invoke();
        }
        public void ToEndPage()
        {
            this.PageAdapter.CurrentPage = this.PageAdapter.MaxPage;
            OnPageChanged.Invoke();
        }
        public void LastPage()
        {
            if (this.PageAdapter.CurrentPage > 1)
            {
                this.PageAdapter.CurrentPage--;
                OnPageChanged.Invoke();
            }
        }
        public void NextPage()
        {
            if (this.PageAdapter.CurrentPage < this.PageAdapter.MaxPage)
            {
                this.PageAdapter.CurrentPage++;
                OnPageChanged.Invoke();
            }
        }
        public void ToPage(int pageId)
        {
            if (pageId >= 1 && pageId <= this.PageAdapter.MaxPage)
            {
                this.PageAdapter.CurrentPage = pageId;
                OnPageChanged.Invoke();
            }
        }

        public void Search()
        {
            if (SearchEdit.Text == "" || FieldSelect.Text == "")
            {
                this.EGAlert("MissingMessage", "MessageNotEnough");
            }
            else
            {
                IsSearched = true;
                PageContainer.Visible = false;
                ExecuteSearch();
            }
        }

        public void ResetSearch()
        {
            if (IsSearched)
            {
                IsSearched = false;
                PageContainer.Visible = true;
                InitPageData();
            }
        }

        public virtual void ExecuteSearch()
        {
            RowDataContainer.ClearChildren();
            string fieldName = FieldSelect.Text;
            string keyWords = SearchEdit.Text;
            List<Dictionary<string, object>> SearchData = TableData.ESearchByKeyword(fieldName, keyWords);
            int dataPointer = 0;
            foreach (Dictionary<string, object> searchrow in SearchData)
            {
                EGodotTableRowData rowData = RowDataContainer.CreateNode<EGodotTableRowData>("row" + dataPointer);
                dataPointer++;
                rowData.Init(searchrow);
                rowData.OnModify.Register(data =>
                {
                    this.EGEditDialog(data, rowData.OnDataEdit, "Modify");
                });
                rowData.OnDelete.Register(() =>
                {
                    GD.Print("Delete : " + rowData.GetData()["Name"]);
                    this.TableData.Remove(rowData.GetData());
                    PageAdapter.DataLength--;
                    PageAdapter.Reload(PageAdapter.DataLength, PageLimit);
                    InitPageData();
                    OnPageChanged.Invoke();
                });
            }
        }
        public override void _ExitTree()
        {
            this.PageChangedRealease.UnRegister();
            base._ExitTree();
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
            if (dataLength % pageLimit == 0)
            {
                this.MaxPage -= 1;
            }
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

        public bool IsFirstPage()
        {
            return this.CurrentPage == 1;
        }
        public bool IsLastPage()
        {
            return this.CurrentPage == MaxPage;
        }

    }

}