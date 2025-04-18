using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EGFramework
{
    #region SaveInit

    public interface IEGSave{
        void InitSave(string path);
    }
    public interface IEGSaveReadOnly{
        void InitReadOnly(string data);
        void InitReadOnly(byte[] data);
    }

    public interface IEGSaveAsync{
        Task InitSaveFileAsync(string path);
    }
    public interface IEGSaveReadOnlyAsync{
        Task InitReadOnlyAsync(string data);
        Task InitReadOnlyAsync(byte[] data);
    }

    #endregion
    
    #region DBConnection
    public interface IEGCanGetDBConnection{
        DbConnection GetConnection();
    }
    #endregion

    #region Object
    public interface IEGSaveObjectReadOnly{
        TObject GetObject<TObject>(string objectKey) where TObject : new();
        IEnumerable<string> GetKeys();
        bool ContainsKey(string objectKey);
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
    public interface IEGSaveObjectReadOnlyAsync{
        Task<TObject> GetObjectAsync<TObject>(string objectKey) where TObject : new();
    }
    public interface IEGSaveObjectAsync : IEGSaveObjectReadOnlyAsync{
        Task SetObjectAsync<TObject>(string objectKey,TObject obj);
    }
    #endregion
    
    #region Data
    public interface IEGSaveDataReadOnly{
        TData GetData<TData>(string dataKey,object id) where TData : new();
        IEnumerable<TData> GetAll<TData>(string dataKey) where TData : new();
        IEnumerable<TData> FindData<TData>(string dataKey,Expression<Func<TData, bool>> expression) where TData : new();
        IEnumerable<string> GetKeys();
        bool ContainsKey(string dataKey);
        bool ContainsData(string dataKey,object id);
    }

    public interface IEGSaveData : IEGSaveDataReadOnly{
        void SetData<TData>(string dataKey,TData data,object id);
        void AddData<TData>(string dataKey,TData data);
        void AddData<TData>(string dataKey,IEnumerable<TData> data);
        void RemoveData<TData>(string dataKey,object id);
        void UpdateData<TData>(string dataKey,TData data,object id);
    }
    
    public interface IEGSaveDataReadOnlyAsync{
        Task<TData> GetDataAsync<TData>(string dataKey,object id) where TData : new();
        Task<IEnumerable<TData>> GetAllAsync<TData>(string dataKey) where TData : new();
        Task<IEnumerable<TData>> FindDataAsync<TData>(string dataKey,Expression<Func<TData, bool>> expression) where TData : new();
    }
    
    public interface IEGSaveDataAsync : IEGSaveDataReadOnlyAsync{
        Task SetDataAsync<TData>(string dataKey,TData data,object id);
    }
    #endregion

    #region File
    public interface IEGFileMsg{
        public string FileName {  get; }
        public bool IsCollection { get; }
        /// <summary>
        /// unit is kb
        /// </summary>
        public long? Size { get; }
        public string Uri { get; }
        public DateTime? LastModify { get; }
        public void Init(string fileName,bool isCollection,string uri,long? size,DateTime? lastmodify);
    }
    public struct EGFileMsg : IEGFileMsg{
        public string FileName { get; set; }
        public bool IsCollection { get; set; }
        public long? Size { get; set; }
        public string Uri { get; set; }
        public DateTime? LastModify { get; set; }
        public void Init(string fileName,bool isCollection,string uri,long? size,DateTime? lastmodify){
            this.FileName = fileName;
            this.IsCollection = isCollection;
            this.Uri = uri;
            this.Size = size;
            this.LastModify = lastmodify;
        }
    }
    public interface IEGSaveFileReadOnly{
        IEnumerable<IEGFileMsg> ListRemoteFilePath(string remotePath);
        bool IsRemoteFileExist(string remotePath);
        bool IsRemoteDirectoryExist(string remotePath);
        void DownloadFile(string remotePath,string localPath);
        Stream DownloadFile(string remotePath);
        void SyncFile(string remotePath,string localPath);
    }
    public interface IEGSaveFile:IEGSaveFileReadOnly{
        void UploadFile(FileStream localFileStream,string remotePath);
        void UploadFile(string localPath,string remotePath);
        void CopyFile(string sourcePath,string copyPath);
        void MoveFile(string sourcePath,string movePath);
        void RemoveFile(string remotePath);
        void MakeDirectory(string remotePath);
    }
    #endregion
}