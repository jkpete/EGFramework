using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EGFramework
{
    public class EGJsonSave : IEGSave,IEGSaveReadOnly,IEGSaveObject
    {
        public bool IsReadOnly { get; set; }
        public Encoding StringEncoding { set; get; } = Encoding.UTF8;
        private string DefaultPath { set; get; }
        private JObject _SaveObject;
        private JObject SaveObject{ 
            get {
                if(_SaveObject == null){
                    InitSave(DefaultPath);
                }
                return _SaveObject;
            }
        }

        /// <summary>
        /// Init a new save data file or load an other file with json suffix, if you want to load other save data, please use this function to reload;
        /// </summary>
        public void InitSave(string path)
        {
            DefaultPath = path;
            if(!File.Exists(path)){
                if (!Directory.Exists(Path.GetDirectoryName(DefaultPath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(DefaultPath));
                    File.WriteAllText(DefaultPath,"{}");
                }else if(!File.Exists(DefaultPath)){
                    File.WriteAllText(DefaultPath,"{}");
                }
            }
            using (StreamReader reader = File.OpenText(path))
            {
                _SaveObject = (JObject)JToken.ReadFrom(new JsonTextReader(reader));
            }
        }

        public void InitReadOnly(string data)
        {
            _SaveObject = JObject.Parse(data);
            IsReadOnly = true;
        }

        public void InitReadOnly(byte[] data)
        {
            _SaveObject = JObject.Parse(StringEncoding.GetString(data));
            IsReadOnly = true;
        }

        public void SetObject<TObject>(string objectKey,TObject obj)
        {
            if(IsReadOnly){
                throw new Exception("This file is readonly! can't set any object to file.");
            }
            if(SaveObject.ContainsKey(objectKey)){
                SaveObject[objectKey] = JToken.FromObject(obj);
            }else{
                SaveObject.Add(objectKey,JToken.FromObject(obj));
            }
            File.WriteAllText(DefaultPath,JsonConvert.SerializeObject(SaveObject,Formatting.Indented));
        }

        /// <summary>
        /// Get data from file, if your data is not in file, then throw an exception.
        /// </summary>
        public TObject GetObject<TObject>(string objectKey) where TObject : new()
        {
            if(!SaveObject.ContainsKey(objectKey)){
                throw new Exception("Key not found!");
            }
            TObject data = SaveObject[objectKey].ToObject<TObject>();
            return data;
        }

        public void RemoveObject<TObject>(string objectKey)
        {
            if(IsReadOnly){
                throw new Exception("This file is readonly! can't set any object to file.");
            }
            if(SaveObject.ContainsKey(objectKey)){
                SaveObject.Remove(objectKey);
            }
            File.WriteAllText(DefaultPath,JsonConvert.SerializeObject(SaveObject,Formatting.Indented));
        }

        public void AddObject<TObject>(string objectKey, TObject obj)
        {
            if(IsReadOnly){
                throw new Exception("This file is readonly! can't set any object to file.");
            }
            if(!SaveObject.ContainsKey(objectKey)){
                SaveObject.Add(objectKey,JToken.FromObject(obj));
            }else{
                throw new Exception("Key already exists!");
            }
            File.WriteAllText(DefaultPath,JsonConvert.SerializeObject(SaveObject,Formatting.Indented));
        }

        public void UpdateObject<TObject>(string objectKey, TObject obj)
        {
            if(IsReadOnly){
                throw new Exception("This file is readonly! can't set any object to file.");
            }
             if(SaveObject.ContainsKey(objectKey)){
                SaveObject[objectKey] = JToken.FromObject(obj);
            }else{
                throw new Exception("Key not found!");
            }
            File.WriteAllText(DefaultPath,JsonConvert.SerializeObject(SaveObject,Formatting.Indented));
        }

        public IEnumerable<string> GetKeys()
        {
            List<string> keys = new List<string>();
            foreach(string key in SaveObject.Properties())
            {
                keys.Add(key);
            }
            return keys;
        }

        public bool ContainsKey(string objectKey)
        {
            return SaveObject.ContainsKey(objectKey);
        }

    }
}