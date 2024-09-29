using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;

namespace EGFramework
{
    public interface IEGSave{
        void SetDataToFile<TData>(TData data);
        TData GetDataByFile<TData>() where TData : class,new();
        void InitSaveData(string path);
    }
    public enum TypeEGSave{
        Json = 0,
        Bson = 1,
        Byte = 2,
        Sqlite = 3,
        LiteDB = 4,
        XML = 5
    }

    public class EGSave : EGModule,IEGSave
    {
        private string DefaultPath = "Default/SaveData.json";
        private JObject _SaveObject;
        private JObject SaveObject{ 
            get {
                if(_SaveObject == null){
                    InitSaveObject();
                }
                return _SaveObject;
            }
        }
        public EGSave() {}
        public override void Init()
        {
            if (!Directory.Exists(DefaultPath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(DefaultPath));
                File.WriteAllText(DefaultPath,"{}");
            }else if(!File.Exists(DefaultPath)){
                File.WriteAllText(DefaultPath,"{}");
            }
        }

        private void InitSaveObject(){
            using (StreamReader reader = File.OpenText(DefaultPath))
            {
                _SaveObject = (JObject)JToken.ReadFrom(new JsonTextReader(reader));
            }
        }

        /// <summary>
        /// Push SaveObject data set to file
        /// </summary>
        public void SaveToFile(){
            SaveToFile(DefaultPath);
        }
        private void SaveToFile(string fileName){
            File.WriteAllText(DefaultPath,JsonConvert.SerializeObject(SaveObject,Formatting.Indented));
        }

        /// <summary>
        /// Push data to SaveObject object cache, this function will not save data to file, if you hope not to IO operation frequently, you can use this with SaveToFile.
        /// </summary>
        /// <param name="data"></param>
        /// <typeparam name="TData"></typeparam>
        public void SetData<TData>(TData data){
            //SaveObject = JObject.FromObject(data);
            if(SaveObject.ContainsKey(typeof(TData).ToString())){
                SaveObject[typeof(TData).ToString()] = JToken.FromObject(data);
            }else{
                SaveObject.Add(typeof(TData).ToString(),JToken.FromObject(data));
            }
        }

        /// <summary>
        /// Get data from file, if your data is not in file, then get null.
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        public TData GetDataByFile<TData>() where TData : class,new(){
            if(!SaveObject.ContainsKey(typeof(TData).ToString())){
                return null;
            }
            TData data = SaveObject[typeof(TData).ToString()].ToObject<TData>();
            return data;
        }

        /// <summary>
        /// Save data to file
        /// </summary>
        /// <param name="data">your any type of data</param>
        /// <typeparam name="TData"></typeparam>
        public void SetDataToFile<TData>(TData data)
        {
            SetData(data);
            SaveToFile();
        }

        #region About Godot File's PATH
        // Godot's Path has res:// and user://
        // UserPath is used for every platform such as android. 
        // You can use ProjectSettings.GlobalizePath("") to convert a "local" path like res://path/to/file.txt to an absolute OS path.
        #endregion

        /// <summary>
        /// Init a new save data file or load an other file with json suffix, if you want to load other save data, please use this function to reload;
        /// </summary>
        /// <param name="fileName"></param>
        public void InitSaveData(string path)
        {
            DefaultPath = path;
            if(!File.Exists(path)){
                File.WriteAllText(path,"{}");
            }
            InitSaveObject();
        }
    }

    public static class CanGetEGSaveExtension{
        public static EGSave EGSave(this IEGFramework self){
            return self.GetModule<EGSave>();
        }
        public static EGSave EGSave(this IEGFramework self,string path){
            self.GetModule<EGSave>().InitSaveData(path);
            return self.GetModule<EGSave>();
        }
    }
}