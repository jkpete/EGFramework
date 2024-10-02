using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EGFramework
{
    public class EGJsonSave : IEGSave,IEGSaveObject
    {
        private string DefaultPath { set; get; }
        private JObject _SaveObject;
        private JObject SaveObject{ 
            get {
                if(_SaveObject == null){
                    InitSaveFile(DefaultPath);
                }
                return _SaveObject;
            }
        }

        /// <summary>
        /// Init a new save data file or load an other file with json suffix, if you want to load other save data, please use this function to reload;
        /// </summary>
        public void InitSaveFile(string path)
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

        public void SetObject<TObject>(string objectKey,TObject obj)
        {
            if(SaveObject.ContainsKey(typeof(TObject).ToString())){
                SaveObject[typeof(TObject).ToString()] = JToken.FromObject(obj);
            }else{
                SaveObject.Add(typeof(TObject).ToString(),JToken.FromObject(obj));
            }
            File.WriteAllText(DefaultPath,JsonConvert.SerializeObject(SaveObject,Formatting.Indented));
        }

        /// <summary>
        /// Get data from file, if your data is not in file, then throw an exception.
        /// </summary>
        public TObject GetObject<TObject>(string objectKey) where TObject : new()
        {
            if(!SaveObject.ContainsKey(typeof(TObject).ToString())){
                throw new Exception("Key not found!");
            }
            TObject data = SaveObject[typeof(TObject).ToString()].ToObject<TObject>();
            return data;
        }
    }
}