using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EGFramework
{
    public class EGCsvSave : IEGSaveData, IEGSave
    {
        public void InitSaveFile(string path)
        {
            throw new NotImplementedException();
        }

        public void SetData<TData>(string dataKey, TData data, object id)
        {
            throw new NotImplementedException();
        }
        public TData GetData<TData>(string dataKey, object id) where TData : new()
        {
            throw new NotImplementedException();
        }
        public IEnumerable<TData> QueryData<TData>(string dataKey, string sql) where TData : new()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TData> GetAll<TData>(string dataKey) where TData : new()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TData> FindData<TData>(string dataKey, Expression<Func<TData, bool>> expression) where TData : new()
        {
            throw new NotImplementedException();
        }
    }
}