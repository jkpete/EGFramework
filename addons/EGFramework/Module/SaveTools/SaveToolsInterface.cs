using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EGFramework
{
    #region Sync_Interface
    public interface IEGSave{
        void InitSave(string path);
    }

    public interface IEGSaveReadOnly{
        void InitReadOnly(string data);
        void InitReadOnly(byte[] data);
    }

    public interface IEGSaveObjectReadOnly{
        TObject GetObject<TObject>(string objectKey) where TObject : new();
        IEnumerable<string> GetKeys();
        bool ContainsKey(string objectKey);
    }
    public interface IEGSaveDataReadOnly{
        TData GetData<TData>(string dataKey,object id) where TData : new();
        IEnumerable<TData> GetAll<TData>(string dataKey) where TData : new();
        IEnumerable<TData> FindData<TData>(string dataKey,Expression<Func<TData, bool>> expression) where TData : new();
        IEnumerable<string> GetKeys();
        bool ContainsKey(string dataKey);
        bool ContainsData(string dataKey,object id);
    }
    
    public interface IEGSaveObject : IEGSaveObjectReadOnly{
        /// <summary>
        /// SetObject will add a object if it not exisits, replace the object if it already exists.
        /// </summary>
        /// <param name="objectKey"></param>
        /// <param name="obj"></param>
        /// <typeparam name="TObject"></typeparam>
        void SetObject<TObject>(string objectKey,TObject obj);
        void RemoveObject<TObject>(string objectKey);
        void AddObject<TObject>(string objectKey,TObject obj);
        void UpdateObject<TObject>(string objectKey,TObject obj);
    }
    
    public interface IEGSaveData : IEGSaveDataReadOnly{
        void SetData<TData>(string dataKey,TData data,object id);
        void AddData<TData>(string dataKey,TData data);
        void AddData<TData>(string dataKey,IEnumerable<TData> data);
        void RemoveData<TData>(string dataKey,object id);
        void UpdateData<TData>(string dataKey,TData data,object id);
    }
    #endregion

    #region Async_Interface
    public interface IEGSaveAsync{
        Task InitSaveFileAsync(string path);
    }
    public interface IEGSaveReadOnlyAsync{
        Task InitReadOnlyAsync(string data);
        Task InitReadOnlyAsync(byte[] data);
    }
    public interface IEGSaveObjectReadOnlyAsync{
        Task<TObject> GetObjectAsync<TObject>(string objectKey) where TObject : new();
    }
    public interface IEGSaveDataReadOnlyAsync{
        Task<TData> GetDataAsync<TData>(string dataKey,object id) where TData : new();
        Task<IEnumerable<TData>> GetAllAsync<TData>(string dataKey) where TData : new();
        Task<IEnumerable<TData>> FindDataAsync<TData>(string dataKey,Expression<Func<TData, bool>> expression) where TData : new();
    }
    public interface IEGSaveObjectAsync : IEGSaveObjectReadOnlyAsync{
        Task SetObjectAsync<TObject>(string objectKey,TObject obj);
    }
    public interface IEGSaveDataAsync : IEGSaveDataReadOnlyAsync{
        Task SetDataAsync<TData>(string dataKey,TData data,object id);
    }
    #endregion
}