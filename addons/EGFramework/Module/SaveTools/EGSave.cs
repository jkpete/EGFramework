using System;
using System.IO;
using System.Collections.Generic;
using Godot;

namespace EGFramework
{
    public enum TypeEGSave{
        Json = 0,
        Bson = 1,
        Byte = 2,
        XML = 3
    }
    public enum TypeDBSave{
        Csv = 0,
        Sqlite = 1,
        LiteDB = 2,
    }

    public class EGSave : EGModule
    {
        #region About Godot File's PATH
        // Godot's Path has res:// and user://
        // UserPath is used for every platform such as android. 
        // You can use ProjectSettings.GlobalizePath("") to convert a "local" path like res://path/to/file.txt to an absolute OS path.
        #endregion

        private Dictionary<string,IEGSaveData> DataBaseFiles = new Dictionary<string,IEGSaveData>();
        private Dictionary<string,IEGSaveObject> ObjectFiles = new Dictionary<string,IEGSaveObject>(); 

        private Dictionary<string,IEGSaveDataReadOnly> DataBaseReadOnly = new Dictionary<string, IEGSaveDataReadOnly>();
        private Dictionary<string,IEGSaveObjectReadOnly> ObjectReadOnly = new Dictionary<string,IEGSaveObjectReadOnly>();
        public EGSave() {}
        public override void Init()
        {
            LoadObjectFile<EGJsonSave>("SaveData/DefaultJsonSave.json".GetGodotResPath());
        }

        public void LoadDataFile<TSaveData>(string path) where TSaveData:IEGSaveData,IEGSave,new(){
            TSaveData saveData = new TSaveData();
            saveData.InitSaveFile(path);
            DataBaseFiles.Add(path,saveData);
        }

        public void ReadData<TReadOnlyData>(string key,string data)where TReadOnlyData:IEGSaveDataReadOnly,IEGSaveReadOnly,new(){
            TReadOnlyData readOnlyData = new TReadOnlyData();
            readOnlyData.InitReadOnly(data);
            DataBaseReadOnly.Add(key,readOnlyData);
        }

        public void LoadObjectFile<TSaveObject>(string path) where TSaveObject:IEGSaveObject,IEGSave,new(){
            TSaveObject saveObject = new TSaveObject();
            saveObject.InitSaveFile(path);
            ObjectFiles.Add(path, saveObject);
        }

        public void ReadObject<TReadOnlyObject>(string key,string data)where TReadOnlyObject:IEGSaveObjectReadOnly,IEGSaveReadOnly,new(){
            TReadOnlyObject readOnlyObject = new TReadOnlyObject();
            readOnlyObject.InitReadOnly(data);
            ObjectReadOnly.Add(key,readOnlyObject);
        }

        public void SetObject<TObject>(string path,string objectKey,TObject obj){
            if(ObjectFiles.ContainsKey(path)){
                ObjectFiles[path].SetObject(objectKey,obj);
            }else{
                throw new Exception("File not loaded, you should use LoadObjectFile(key) first.");
            }
        }
        
        public TObject GetObject<TObject>(string path,string key) where TObject : new(){
            if(ObjectFiles.ContainsKey(path)){
                return ObjectFiles[path].GetObject<TObject>(key);
            }else if(ObjectReadOnly.ContainsKey(path)){
                return ObjectReadOnly[path].GetObject<TObject>(key);
            }else{
                throw new Exception("File not loaded, you should use LoadObjectFile(key) or ReadObject(key) first.");
            }
        }

        public void SetData<TData>(string path,string dataKey,TData data,int id){
            if(DataBaseFiles.ContainsKey(path)){
                DataBaseFiles[path].SetData(dataKey,data,id);
            }else{
                throw new Exception("File not loaded, you should use LoadDataFile(path) first.");
            }
        }
        public TData GetData<TData>(string path,string key,int id) where TData : new(){
            if(DataBaseFiles.ContainsKey(path)){
                return DataBaseFiles[path].GetData<TData>(key,id);
            }else if(DataBaseReadOnly.ContainsKey(path)){
                return DataBaseReadOnly[path].GetData<TData>(key,id);
            }else{
                throw new Exception("File not loaded, you should use LoadDataFile(key) or ReadData(key,data) first.");
            }
        }

        public IEnumerable<TData> GetAllData<TData>(string path,string key) where TData : new(){
            if(DataBaseFiles.ContainsKey(path)){
                return DataBaseFiles[path].GetAll<TData>(key);
            }else if(DataBaseReadOnly.ContainsKey(path)){
                return DataBaseReadOnly[path].GetAll<TData>(key);
            }else{
                throw new Exception("File not loaded, you should use LoadDataFile(key) or ReadData(key,data) first.");
            }
        }


        //------------------------------------------------------------------------------//

        #region Default Json Operation
        public void SetObjectToJson<TObject>(TObject obj){
            ObjectFiles["SaveData/DefaultJsonSave.json"].SetObject(typeof(TObject).ToString(),obj);
        }
        public TObject GetObjectFromJson<TObject>() where TObject : new(){
            return ObjectFiles["SaveData/DefaultJsonSave.json"].GetObject<TObject>(typeof(TObject).ToString());
        }
        #endregion

        //------------------------------------------------------------------------------//
        

        public void OpenResPath(){
            OS.ShellOpen("".GetGodotResPath());   
        }

        public void OpenUserPath(){
            OS.ShellOpen("".GetGodotUserPath());   
        }

        
    }
        
    public static class CanGetEGSaveExtension{
        public static EGSave EGSave(this IEGFramework self){
            return self.GetModule<EGSave>();
        }

        public static string GetGodotResPath(this string path){
            return ProjectSettings.GlobalizePath("res://"+path);
        }

        public static string GetGodotUserPath(this string path){
            return ProjectSettings.GlobalizePath("user://"+path);
        }

    }
}