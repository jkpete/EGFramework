using MySqlConnector;

namespace EGFramework{
    /// <summary>
    /// This Class used Dapper for operate MySQL database.
    /// </summary>
    public class EGMysqlSave : EGDapper
    {
        public string Conn { set; get; }
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
                this.Conn = conn;
            }
            catch (System.Exception e)
            {
                EG.Print("e:" + e);
            }
        }
    }
}