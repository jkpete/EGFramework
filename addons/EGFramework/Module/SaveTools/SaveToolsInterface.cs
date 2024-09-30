using System;
using System.Collections.Generic;
using System.Linq;
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
        void SetData<TData>(TData data,string dataKey,int id);
        TData GetData<TData>(string dataKey,int id) where TData : new();
        IList<TData> QueryData<TData>(string dataKey,string sql) where TData : new();
    }
}