using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq.Expressions;

//ORM Save tools. First support SQLite and MySQL,In future we will support other Database who implement DBConnection.
namespace EGFramework{
    public class EGDapper : IEGSave, IEGSaveData
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="conn">files conn Str or address ip port,username and passwd</param>
        public void InitSaveFile(string conn)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<TData> FindData<TData>(string dataKey, Expression<Func<TData, bool>> expression) where TData : new()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TData> GetAll<TData>(string dataKey) where TData : new()
        {
            throw new NotImplementedException();
        }

        public TData GetData<TData>(string dataKey, object id) where TData : new()
        {
            throw new NotImplementedException();
        }

        public void SetData<TData>(string dataKey, TData data, object id)
        {
            throw new NotImplementedException();
        }
    }
}