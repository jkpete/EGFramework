using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;

namespace EGFramework{
    public interface IEGSqlite{
        void SaveData<TData>(TData data) where TData : new();
        List<TData> GetDataSet<TData>() where TData : new();
        void InitDatabase(string dataBaseName);
    }
    public class EGSqlite : EGModule,IEGSqlite
    {
        public string DBName = "Default";
        private string DefaultDBFolder = "SaveData";
        public SqliteConnection SqliteConn;
        public string ExceptionMsg;
        
        public override void Init()
        {
            if (!Directory.Exists(DefaultDBFolder))
            {
                Directory.CreateDirectory(DefaultDBFolder);
            }
            InitDatabase(DBName);
        }
        
        public void InitDatabase(string dataBaseName)
        {
            SqliteConn = new SqliteConnection("Data Source="+DefaultDBFolder+"/"+dataBaseName+".db;Mode=ReadWriteCreate;");            // Open the connection:
            try
            {
                SqliteConn.Open();
            }
            catch (Exception ex)
            {
                ExceptionMsg = ex.ToString();
            }
        }

        //Save data to default sqlite database;
        public void SaveData<TData>(TData data) where TData : new()
        {
            // if table is not exist, create table and insert data to table,else insert into data to table  
            if(IsTableExist<TData>()){
                InsertData(data);
            }else{
                CreateTable<TData>();
                InsertData(data);
            }
        }
        /// <summary>
        /// Get data from table where named type of TData
        /// </summary>
        /// <typeparam name="TData">Table name</typeparam>
        /// <returns></returns>
        public List<TData> GetDataSet<TData>() where TData : new()
        {
            // query dataSet from table TData_List
            List<TData> dataSet = new List<TData>();
            if(IsTableExist<TData>()){
                dataSet = SelectData<TData>();
            }else{
                ExceptionMsg = "No such table,ensure one data with type of TData has been saved at least!";
                return null;
            }
            return dataSet;
        }

        #region SQL Operation
        /// <summary>
        /// Create table where table name is type of TData
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <returns></returns>
        public string CreateTable<TData>() where TData: new() {
            string result = "Success:";
            try
            {
                string sqlCommand = "CREATE TABLE " + typeof(TData).Name;
                sqlCommand += "(\"ID\" INTEGER NOT NULL UNIQUE,";
                var properties = typeof(TData).GetFields();
                foreach(var property in properties){
                    if(property.FieldType == typeof(int) || property.FieldType == typeof(bool) || property.FieldType.IsEnum){
                        sqlCommand += "\"" + property.Name + "\"   INTEGER" + "     NOT NULL,";
                    }else if(property.FieldType == typeof(double) || property.FieldType == typeof(float)){
                        sqlCommand += "\"" + property.Name + "\"   REAL" + "     NOT NULL,";
                    }
                    else{
                        sqlCommand += "\"" + property.Name + "\"   TEXT" + "     NOT NULL,";
                    }
                }
                sqlCommand += "PRIMARY KEY(\"ID\" AUTOINCREMENT))";
                SqliteCommand createCommand = new SqliteCommand(sqlCommand,SqliteConn);
                result = result + createCommand.ExecuteNonQuery().ToString();
            }
            catch (System.Exception e)
            {
                return "Error:"+e;
            }
            return result;
        }
        
        /// <summary>
        /// Drop table where table name is type of TData
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <returns></returns>
        public string DropTable<TData>() where TData: new(){
            string result = "Success:";
            try
            {
                string sqlCommand = "DROP TABLE " + typeof(TData).Name;
                SqliteCommand createCommand = new SqliteCommand(sqlCommand,SqliteConn);
                result = result + createCommand.ExecuteNonQuery().ToString();
            }
            catch (System.Exception e)
            {
                return "Error:"+e;
            }
            return result;
        }

        /// <summary>
        /// Insert data to table where table name is type of TData
        /// </summary>
        /// <param name="data"></param>
        /// <typeparam name="TData"></typeparam>
        /// <returns>success or error</returns>
        public string InsertData<TData>(TData data) where TData: new(){
            string result = "Success:";
            try
            {
                string sqlCommand = "INSERT INTO " + typeof(TData).Name;
                var properties = typeof(TData).GetFields();
                Dictionary<string,object> dataParams = new Dictionary<string, object>();
                foreach(var property in properties){
                    dataParams.Add(property.Name,property.GetValue(data));
                    if(property.FieldType==typeof(bool) || property.FieldType.IsEnum){
                        // If property is bool type , save data to data base should be 0 or 1 instead of false or true;
                        // If property is Enum type , then transform data to int;
                        dataParams[property.Name] = System.Convert.ToInt32(dataParams[property.Name]);
                    }else if(property.FieldType.IsClass || property.FieldType.IsValueType && !property.FieldType.IsPrimitive && property.FieldType != typeof(string)){
                        dataParams[property.Name] = JsonConvert.SerializeObject(dataParams[property.Name]);
                    }
                }
                sqlCommand += "(";
                string keySet = "";
                foreach(string key in dataParams.Keys){
                    keySet += key + ",";
                }
                keySet = keySet.TrimEnd(',');
                sqlCommand += keySet;
                sqlCommand += ") VALUES (";
                string valueSet = "";
                foreach(var value in dataParams.Values){
                    if(value.GetType() == typeof(int) || value.GetType() == typeof(float) || value.GetType() == typeof(double)){
                        valueSet += value + ",";
                    }else{
                        valueSet += "'" + value + "',";
                    }
                }
                valueSet = valueSet.TrimEnd(',');
                sqlCommand += valueSet;
                sqlCommand += ")";
                SqliteCommand createCommand = new SqliteCommand(sqlCommand,SqliteConn);
                result = result + createCommand.ExecuteNonQuery().ToString();
            }
            catch (System.Exception e)
            {
                ExceptionMsg = e.ToString();
                return "Error:"+ExceptionMsg;
            }
            return result;
        }
        
        /// <summary>
        /// Query Data and return object list with TData type,Support Data Type：ClassObject,Enum,int,string.float,struct.Not support double,if double then auto convert to float
        /// </summary>
        /// <returns>List of TData or null ,if null then you can print ExceptionMsg to check your error</returns>
        public List<TData> SelectData<TData>() where TData: new(){
            List<TData> resultList = new List<TData>();
            try
            {
                string sqlCommand = "SELECT * FROM " + typeof(TData).Name;
                SqliteCommand selectCommand = new SqliteCommand(sqlCommand,SqliteConn);
                SqliteDataReader reader = selectCommand.ExecuteReader();
                var properties = typeof(TData).GetFields();
                
                while (reader.Read())
                {
                    TData dataRow = new TData();
                    foreach(var property in properties){
                        if(property.FieldType == reader[property.Name].GetType()){
                            property.SetValue(dataRow,reader[property.Name]);
                        }else if(property.FieldType.IsEnum){
                            object propertyEnum = Enum.Parse(property.FieldType,reader[property.Name].ToString());
                            property.SetValue(dataRow,propertyEnum);
                        }
                        else if(property.FieldType.IsPrimitive) {
                            object propertyObject = System.Convert.ChangeType(reader[property.Name],property.FieldType);
                            property.SetValue(dataRow,propertyObject);
                        }else{
                            object classObject = JsonConvert.DeserializeObject(reader[property.Name].ToString(),property.FieldType);
                            property.SetValue(dataRow,classObject);
                        }
                    }
                    resultList.Add(dataRow);
                }
            }
            catch (System.Exception e)
            {
                ExceptionMsg = e.ToString();
                return null;
            }
            return resultList;
        }
        
        public bool IsTableExist<TData>() where TData:new(){
             try
            {
                string sqlCommand = "SELECT name FROM sqlite_sequence";
                SqliteCommand selectCommand = new SqliteCommand(sqlCommand,SqliteConn);
                SqliteDataReader reader = selectCommand.ExecuteReader();
                while (reader.Read()){
                    if(reader["name"].ToString()==typeof(TData).Name){
                        return true;
                    }
                }
            }
            catch (System.Exception e)
            {
                ExceptionMsg = e.ToString();
                return false;
            }
            return false;
        }
        #endregion
    }

    public static class CanGetEGSqliteExtension{
        public static EGSqlite EGSqlite(this IEGFramework self){
            return EGArchitectureImplement.Interface.GetModule<EGSqlite>();
        }
    }
}
