using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MySql.Data.MySqlClient;
using Dapper;
using System.Reflection;
using System.Linq;
using System.Data.Common;

//ORM Save tools. First support SQLite and MySQL,In future we will support other Database who implement DBConnection.
namespace EGFramework{
    /// <summary>
    /// This Class used Dapper for operate MySQL database.
    /// </summary>
    public class EGMysqlSave : EGDapper
    {
        private string Conn { set; get; }
        public bool IsInit { set; get; }
        /// <summary>
        /// "server="+Address+";port="+Port+";uid="+UserName+";pwd="+Password+";database="+DataBase+";"
        /// </summary>
        /// <param name="conn">files conn Str or address ip port,username and passwd</param>
        public override void InitSave(string conn)
        {
            try
            {
                Connection = new MySqlConnection(conn);
                IsInit = true;
            }
            catch (System.Exception e)
            {
                EG.Print("e:" + e);
            }
        }
    }
}