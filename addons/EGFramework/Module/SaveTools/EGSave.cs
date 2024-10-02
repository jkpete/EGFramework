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

        public void LoadObjectFile<TSaveObject>(string path) where TSaveObject:IEGSaveObject,IEGSave,new(){
            TSaveObject saveObject = new TSaveObject();
            saveObject.InitSaveFile(path);
            ObjectFiles.Add(path, saveObject);
        }

        public void SetObject<TObject>(string path,string objectKey,TObject obj){
            ObjectFiles[path].SetObject(objectKey,obj);
        }
        public TObject GetObject<TObject>(string path,string key) where TObject : new(){
            return ObjectFiles[path].GetObject<TObject>(key);
        }

        public void SetData<TData>(string path,string dataKey,TData data,int id){
            DataBaseFiles[path].SetData(dataKey,data,id);
        }
        public void GetData<TData>(string path,string key,int id) where TData : new(){
            DataBaseFiles[path].GetData<TData>(key,id);
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

        public static string GetGodotResPath(this string absPath){
            return ProjectSettings.GlobalizePath("res://"+absPath);
        }

        public static string GetGodotUserPath(this string absPath){
            return ProjectSettings.GlobalizePath("user://"+absPath);
        }

    }
}