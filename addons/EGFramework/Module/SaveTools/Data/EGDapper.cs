using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Dapper;
//ORM Save tools. First support SQLite and MySQL,In future we will support other Database who implement DBConnection.
namespace EGFramework
{
    public abstract class EGDapper : IEGSave, IEGSaveData, IEGCanGetDBConnection
    {
        public DbConnection Connection { get; set; }
        public string ExceptionMsg;

        public abstract void InitSave(string conn);
        public IEnumerable<TData> GetAll<TData>(string dataKey) where TData : new()
        {
            IEnumerable<TData> result = Connection.Query<TData>("select * from "+dataKey);
            return result;
        }

        public IEnumerable<TData> GetPage<TData>(string dataKey, int pageIndex, int pageSize) where TData : new()
        {
            if(pageIndex <= 0){
                pageIndex = 1;
            }
            int startPointer = (pageIndex - 1) * pageSize;
            IEnumerable<TData> result = Connection.Query<TData>("select * from "+dataKey+" limit "+startPointer+","+pageIndex);
            return result;
        }

        public TData GetData<TData>(string dataKey, object id) where TData : new()
        {
            TData result = Connection.QuerySingle<TData>("select * from "+dataKey+" where ID = @ID",new {ID = id});
            return result;
        }

        public IEnumerable<TData> FindData<TData>(string dataKey, Expression<Func<TData, bool>> expression) where TData : new()
        {
            IEnumerable<TData> sourceList = Connection.Query<TData>("select * from "+dataKey);
            return sourceList.Where(expression.Compile());
        }

        public void SetData<TData>(string dataKey, TData data, object id)
        {
            if(data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }else{
                if(this.ContainsData(dataKey,id)){
                    UpdateData(dataKey,data,id);
                }else{
                    AddData(dataKey,data);
                }
                //EG.Print("data:" + data);
            }
        }

        public void AddData<TData>(string dataKey, TData data)
        {
            if(data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            // throw new System.NotImplementedException();
            Type DataType = typeof(TData);
            var properties = DataType.GetProperties();
            string keySet = "";
            string keySetParam = "";
            foreach(PropertyInfo key in properties){
                keySet += key.Name + ",";
                keySetParam += "@" +  key.Name + ",";
            }
            keySet = keySet.TrimEnd(',');
            keySetParam = keySetParam.TrimEnd(',');
            int count = Connection.Execute(@"insert "+dataKey+"("+keySet+") values("+keySetParam+")",data);
            //EG.Print("count:" + count);
        }

        public void AddData<TData>(string dataKey, IEnumerable<TData> data)
        {
            Type DataType = typeof(TData);
            var properties = DataType.GetProperties();
            string keySet = "";
            string keySetParam = "";
            foreach(PropertyInfo key in properties){
                keySet += key.Name + ",";
                keySetParam += "@" +  key.Name + ",";
            }
            keySet = keySet.TrimEnd(',');
            keySetParam = keySetParam.TrimEnd(',');
            string sql = @"insert "+dataKey+"("+keySet+") values("+keySetParam+")";
            int count = Connection.Execute(sql,data);
            //EG.Print("count:" + count);
        }

        public void RemoveData<TData>(string dataKey, object id)
        {
            int count = Connection.Execute(@"delete from "+dataKey+" where ID = @ID",new {ID = id});
            //EG.Print("count:" + count);
        }

        public void UpdateData<TData>(string dataKey, TData data, object id)
        {
            if(data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            Type DataType = typeof(TData);
            EG.Print("----"+DataType.Name);
            var properties = DataType.GetProperties();
            string keyMap = "";
            foreach(PropertyInfo key in properties){
                if(key.Name=="ID"){
                    continue;
                }
                keyMap += key.Name + " = @"+key.Name +",";
            }
            keyMap = keyMap.TrimEnd(',');
            string sql = @"update "+DataType.Name+" set "+ keyMap +" where ID = " + id;
            EG.Print(sql);
            int count = Connection.Execute(sql,data);
            //EG.Print("count:" + count);
        }

        public IEnumerable<string> GetKeys()
        {
            IEnumerable<string> result = Connection.Query<string>("show tables");
            return result;
        }

        public bool ContainsKey(string dataKey)
        {
            return GetKeys().Contains(dataKey);
        }

        public bool ContainsData(string dataKey, object id)
        {
            try
            {
                var result = Connection.QuerySingle("select * from "+dataKey+" where ID = @ID",new {ID = id});
                if(result == null){
                    return false;
                }else{
                    return true;
                }
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public int GetDataCount(string dataKey)
        {
            int count = Connection.QuerySingle<int>("select COUNT(*) from " + dataKey);
            return count;
        }

        public DbConnection GetConnection()
        {
            return Connection;
        }
    }
}