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

    public interface IEGSaveReadOnly{
        void InitReadOnly(string data);
        void InitReadOnly(byte[] data);
    }

    public interface IEGSaveObjectReadOnly{
        TObject GetObject<TObject>(string objectKey) where TObject : new();
    }
    public interface IEGSaveDataReadOnly{
        TData GetData<TData>(string dataKey,object id) where TData : new();
        IEnumerable<TData> GetAll<TData>(string dataKey) where TData : new();
        IEnumerable<TData> FindData<TData>(string dataKey,Expression<Func<TData, bool>> expression) where TData : new();
    }
    
    public interface IEGSaveObject : IEGSaveObjectReadOnly{
        void SetObject<TObject>(string objectKey,TObject obj);
    }
    
    // 
    public interface IEGSaveData : IEGSaveDataReadOnly{
        void SetData<TData>(string dataKey,TData data,object id);
    }
}