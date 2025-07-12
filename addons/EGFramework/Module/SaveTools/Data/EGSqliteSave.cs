using System;
using System.IO;
using Microsoft.Data.Sqlite;

namespace EGFramework{
    public class EGSqliteSave : EGDapper
    {
        public string Conn { set; get; }
        public bool IsInit { set; get; }

        /// <summary>
        /// If path not exist, create it.
        /// </summary>
        /// <param name="path">please add *.db suffix or your db file suffix</param>
        public override void InitSave(string path)
        {
            if (!File.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            InitDatabase(path);
        }

        /// <summary>
        /// Init database with path.
        /// </summary>
        /// <param name="dataBaseName">name is the file path.such as SaveData.db</param>
        public void InitDatabase(string dataBaseName)
        {
            Connection = new SqliteConnection("Data Source="+dataBaseName+";Mode=ReadWriteCreate;");            // Open the connection:
            try
            {
                // Connection.Open();
                this.Conn = dataBaseName;
                IsInit = true;
            }
            catch (Exception ex)
            {
                ExceptionMsg = ex.ToString();
            }
        }
    }
}