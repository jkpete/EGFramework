using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EGFramework
{
    public interface IEGSave{
        void SetDataToFile<TData>(TData data);
        TData GetDataByFile<TData>() where TData : class,new();
        void InitSaveData(string fileName);
    }
    public class EGSave : EGModule,IEGSave
    {
        private string DefaultSaveFile = "Default";
        private string DefaultSaveFolder = "SaveData";
        private JObject _SaveObject;
        private JObject SaveObject{ 
            get {
                if(_SaveObject == null){
                    InitSaveObject();
                }
                return _SaveObject;
            }
        }

        public EGSave(){

        }
        /// <summary>
        /// if you want to define default save data file name, please use "this.RegisterModule(new EGSave("FileName"))"in your architecture code(Init function);
        /// </summary>
        /// <param name="fileName"></param>
        public EGSave(string fileName){
            this.DefaultSaveFile = fileName;
        }
        public override void Init()
        {
            if (!Directory.Exists(DefaultSaveFolder))
            {
                Directory.CreateDirectory(DefaultSaveFolder);
                File.WriteAllText(DefaultSaveFolder + "/" + DefaultSaveFile + ".json","{}");
            }else if(!File.Exists(DefaultSaveFolder + "/" + DefaultSaveFile + ".json")){
                File.WriteAllText(DefaultSaveFolder + "/"  + DefaultSaveFile + ".json","{}");
            }
        }

        private void InitSaveObject(){
            using (StreamReader reader = File.OpenText(DefaultSaveFolder + "/" + DefaultSaveFile + ".json"))
            {
                _SaveObject = (JObject)JToken.ReadFrom(new JsonTextReader(reader));
            }
        }

        /// <summary>
        /// Push SaveObject data set to file
        /// </summary>
        public void SaveToFile(){
            SaveToFile(DefaultSaveFile);
        }
        private void SaveToFile(string fileName){
            File.WriteAllText(DefaultSaveFolder + "/" + fileName + ".json",JsonConvert.SerializeObject(SaveObject,Formatting.Indented));
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

        /// <summary>
        /// Init a new save data file or load an other file with json suffix, if you want to load other save data, please use this function to reload;
        /// </summary>
        /// <param name="fileName"></param>
        public void InitSaveData(string fileName)
        {
            DefaultSaveFile = fileName;
            if(!File.Exists(DefaultSaveFolder + "/" + DefaultSaveFile + ".json")){
                File.WriteAllText(DefaultSaveFolder + "/" + DefaultSaveFile + ".json","{}");
            }
            InitSaveObject();
        }
    }

    public static class CanGetEGSaveExtension{
        public static EGSave EGSave(this IEGFramework self){
            return EGArchitectureImplement.Interface.GetModule<EGSave>();
        }
    }
}