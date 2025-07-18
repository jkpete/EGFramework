using System.Collections.Generic;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace EGFramework{
    public class EGRedisSave : IEGSave, IEGSaveObject
    {
        public string Conn { set; get; }
        public bool IsInit { set; get; }
        public ConnectionMultiplexer Redis { set; get; }
        public IDatabase Database { set; get; }
        public ISubscriber Subscriber { set; get; }

        /// <summary>
        /// connect to redis server
        /// </summary>
        /// <param name="conn"> such as server1:6379,server2:6379 </param>
        public void InitSave(string conn)
        {
            try
            {
                Redis = ConnectionMultiplexer.Connect(conn);
                IsInit = true;
                this.Conn = conn;
                Redis = ConnectionMultiplexer.Connect(conn);
                Database = Redis.GetDatabase();
                Subscriber = Redis.GetSubscriber();
            }
            catch (System.Exception)
            {
                //EG.Print("e:" + e);
            }
        }

        public void AddObject<TObject>(string objectKey, TObject obj)
        {
            if(!Database.KeyExists(objectKey)){
                Database.SetAdd(objectKey, JsonConvert.SerializeObject(obj));
            }else{
                throw new System.Exception("Key already exists in redis database.");
            }
            
        }

        public void UpdateObject<TObject>(string objectKey, TObject obj)
        {
            if(Database.KeyExists(objectKey)){
                Database.SetAdd(objectKey, JsonConvert.SerializeObject(obj));
            }else{
                throw new System.Exception("Key not exists in redis database.");
            }
        }

        public void RemoveObject<TObject>(string objectKey)
        {
            Database.KeyDelete(objectKey);
        }

        public void SetObject<TObject>(string objectKey, TObject obj)
        {
            Database.SetAdd(objectKey, JsonConvert.SerializeObject(obj));
        }

        public TObject GetObject<TObject>(string objectKey)
        {
            try
            {
                string result = Database.StringGet(objectKey);
                TObject resultObj = JsonConvert.DeserializeObject<TObject>(result);
                return resultObj;
            }
            catch (System.Exception)
            {
                throw;
            }
        }
        
        public bool ContainsKey(string objectKey)
        {
            return Database.KeyExists(objectKey);
        }

        public IEnumerable<string> GetKeys()
        {
            throw new System.NotImplementedException();
        }
    }
}