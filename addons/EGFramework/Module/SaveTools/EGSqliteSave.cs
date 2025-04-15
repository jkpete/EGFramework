using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Dapper;
using Microsoft.Data.Sqlite;

namespace EGFramework{
    public class EGSqliteSave : EGDapper
    {
        private string Conn { set; get; }
        public bool IsInit { set; get; }

        /// <summary>
        /// If path not exist, create it.
        /// </summary>
        /// <param name="path"></param>
        public override void InitSave(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            InitDatabase(path);
        }

        /// <summary>
        /// Init database with path.
        /// </summary>
        /// <param name="dataBaseName">name is the file path.</param>
        public void InitDatabase(string dataBaseName)
        {
            Connection = new SqliteConnection("Data Source="+dataBaseName+";Mode=ReadWriteCreate;");            // Open the connection:
            try
            {
                Connection.Open();
            }
            catch (Exception ex)
            {
                ExceptionMsg = ex.ToString();
            }
        }
    }
}