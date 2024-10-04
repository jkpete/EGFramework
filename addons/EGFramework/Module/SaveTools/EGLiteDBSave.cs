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
                    InitSaveFile(DefaultPath);
                }
                return _Database;
            }
        }
        
        public void InitSaveFile(string path)
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

        public IEnumerable<TData> FindData<TData>(string dataKey, Expression<Func<TData, bool>> expression) where TData : new()
        {
            LiteCollection<TData> collection = (LiteCollection<TData>)Database.GetCollection<TData>(dataKey);
            return collection.Find(expression);
        }

        
    }
}