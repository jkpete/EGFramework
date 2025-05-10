using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LiteDB;

namespace EGFramework
{
    public class EGLiteDBSave : IEGSave,IEGSaveData
    {
        private string DefaultPath { set; get; }
        private LiteDatabase _Database { set; get; }
        private LiteDatabase Database{ 
            get {
                if(_Database == null){
                    InitSave(DefaultPath);
                }
                return _Database;
            }
        }
        
        public void InitSave(string path)
        {
            DefaultPath = path;
            if (!Directory.Exists(Path.GetDirectoryName(DefaultPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(DefaultPath));
            }
            // Default is "SaveData/DefaultLiteDBData.db"
            _Database = new LiteDatabase(path);
        }
        
        public TData GetData<TData>(string dataKey, object id) where TData : new()
        {
            LiteCollection<TData> collection = (LiteCollection<TData>)Database.GetCollection<TData>(dataKey);
            TData result = collection.FindById((BsonValue)id);
            return result;
        }
        
        public void SetData<TData>(string dataKey, TData data, object id)
        {
            LiteCollection<TData> collection = (LiteCollection<TData>)Database.GetCollection<TData>(dataKey);
            if(collection.FindById((BsonValue)id)==null){
                collection.Insert((BsonValue)id, data);
            }
            collection.Update(data);
        }

        public IEnumerable<TData> GetAll<TData>(string dataKey) where TData : new()
        {
            LiteCollection<TData> collection = (LiteCollection<TData>)Database.GetCollection<TData>(dataKey);
            return collection.FindAll();
        }

        public IEnumerable<TData> GetPage<TData>(string dataKey, int pageIndex, int pageSize) where TData : new()
        {
            if(pageIndex <= 0){
                pageIndex = 1;
            }
            int startPointer = (pageIndex - 1) * pageSize;
            LiteCollection<TData> collection = (LiteCollection<TData>)Database.GetCollection<TData>(dataKey);
            IEnumerable<TData> result = collection.FindAll().Skip(startPointer).Take(pageSize);
            return result;
        }

        public IEnumerable<TData> FindData<TData>(string dataKey, Expression<Func<TData, bool>> expression) where TData : new()
        {
            LiteCollection<TData> collection = (LiteCollection<TData>)Database.GetCollection<TData>(dataKey);
            return collection.Find(expression);
        }

        public void AddData<TData>(string dataKey, TData data)
        {
            LiteCollection<TData> collection = (LiteCollection<TData>)Database.GetCollection<TData>(dataKey);
            collection.Insert(data);
        }

        public void AddData<TData>(string dataKey, IEnumerable<TData> data)
        {
            LiteCollection<TData> collection = (LiteCollection<TData>)Database.GetCollection<TData>(dataKey);
            collection.Insert(data);
        }

        public void RemoveData<TData>(string dataKey,object id)
        {
            LiteCollection<TData> collection = (LiteCollection<TData>)Database.GetCollection<TData>(dataKey);
            if(collection.FindById((BsonValue)id)==null){
                collection.Delete((BsonValue)id);
            }
        }

        public void UpdateData<TData>(string dataKey, TData data, object id)
        {
            LiteCollection<TData> collection = (LiteCollection<TData>)Database.GetCollection<TData>(dataKey);
            collection.Update((BsonValue)id,data);
        }
        public IEnumerable<string> GetKeys()
        {
            return Database.GetCollectionNames();
        }

        public bool ContainsKey(string dataKey)
        {
            return GetKeys().Contains(dataKey);
        }

        public bool ContainsData(string dataKey, object id)
        {
            return Database.GetCollection(dataKey).Exists((BsonValue)id);
        }

        public int GetDataCount(string dataKey)
        {
            return Database.GetCollection(dataKey).Count();
        }
    }
}