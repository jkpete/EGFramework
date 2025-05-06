public interface ITableData
{
    /// <summary>
    /// Get the data of the table.
    /// </summary>
    /// <returns></returns>
    string[][] GetTableData();
    /// <summary>
    /// Get the header of the table.
    /// </summary>
    /// <returns></returns>
    string[] GetTableHeader();
}

public interface ITableRowData
{
    string[] GetRowData();
}