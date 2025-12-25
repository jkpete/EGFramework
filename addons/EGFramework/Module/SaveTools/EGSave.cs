using System;
using System.IO;
using System.Collections.Generic;
using Dapper;
using System.Linq;

namespace EGFramework
{
    public class EGSave : EGModule
    {
        #region About Godot File's PATH
        // Godot's Path has res:// and user://
        // UserPath is used for every platform such as android. 
        // You can use ProjectSettings.GlobalizePath("") to convert a "local" path like res://path/to/file.txt to an absolute OS path.
        #endregion

        public HashSet<string> KeySet = new HashSet<string>();
        public Dictionary<string, object> SaveMapping = new Dictionary<string, object>();

        public EGJsonSave DefaultJsonSave { set; get; }
        public EGSave() { }
        public override void Init()
        {
            // DefaultJsonSave = Load<EGJsonSave>("SaveData/DefaultJsonSave.json");
        }
        #region Load Data or Object and Unload
        public TSaveData Load<TSaveData>(string path) where TSaveData : IEGSave, new()
        {
            KeySet.Add(path);
            TSaveData saveData = new TSaveData();
            saveData.InitSave(path);
            SaveMapping.Add(path, saveData);
            return saveData;
        }

        public TReadOnlyData Read<TReadOnlyData>(string key, string data) where TReadOnlyData : IEGSaveReadOnly, new()
        {
            KeySet.Add(key);
            TReadOnlyData readOnlyData = new TReadOnlyData();
            readOnlyData.InitReadOnly(data);
            SaveMapping.Add(key, readOnlyData);
            return readOnlyData;
        }

        public TReadOnlyData Read<TReadOnlyData>(string key, byte[] data) where TReadOnlyData : IEGSaveReadOnly, new()
        {
            KeySet.Add(key);
            TReadOnlyData readOnlyData = new TReadOnlyData();
            readOnlyData.InitReadOnly(data);
            SaveMapping.Add(key, readOnlyData);
            return readOnlyData;
        }
        
        #endregion


        #region Default Json Operation
        public void SetObjectToJson<TObject>(TObject obj)
        {
            if(DefaultJsonSave == null)
            {
                DefaultJsonSave = Load<EGJsonSave>("SaveData/DefaultJsonSave.json");
            }
            DefaultJsonSave.SetObject(typeof(TObject).ToString(), obj);
        }
        public TObject GetObjectFromJson<TObject>() where TObject : new(){
            if(DefaultJsonSave == null)
            {
                DefaultJsonSave = Load<EGJsonSave>("SaveData/DefaultJsonSave.json");
            }
            return DefaultJsonSave.GetObject<TObject>(typeof(TObject).ToString());
        }
        #endregion

    }
        
    public static class CanGetEGSaveExtension{
        public static EGSave EGSave(this IEGFramework self){
            return self.GetModule<EGSave>();
        }
        
        public static string GetDirectoryName(this string path){
            return Path.GetDirectoryName(path);
        }

    }
}