using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EGFramework
{
    public interface IEGSave{
        void InitSaveFile(string path);
    }

    public interface IEGSaveObject{
        void SetObject<TObject>(string objectKey,TObject obj);
        TObject GetObject<TObject>(string objectKey) where TObject : new();
    }
    
    // 
    public interface IEGSaveData{
        void SetData<TData>(string dataKey,TData data,object id);
        TData GetData<TData>(string dataKey,object id) where TData : new();
        IEnumerable<TData> GetAll<TData>(string dataKey) where TData : new();
        IEnumerable<TData> FindData<TData>(string dataKey,Expression<Func<TData, bool>> expression) where TData : new();
        IEnumerable<TData> QueryData<TData>(string dataKey,string sql) where TData : new();
    }
}