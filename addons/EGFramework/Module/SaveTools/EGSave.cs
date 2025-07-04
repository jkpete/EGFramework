using System;
using System.IO;
using System.Collections.Generic;
using Dapper;

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
            LoadObjectFile<EGJsonSave>("SaveData/DefaultJsonSave.json");
        }
        #region Load Data or Object and Unload
        public TSaveData LoadDataFile<TSaveData>(string path) where TSaveData : IEGSaveData, IEGSave, new()
        {
            TSaveData saveData = new TSaveData();
            saveData.InitSave(path);
            if (!DataBaseFiles.ContainsKey(path))
            {
                DataBaseFiles.Add(path, saveData);
            }
            else
            {
                DataBaseFiles[path] = saveData;
            }
            return saveData;
        }

        public TReadOnlyData ReadData<TReadOnlyData>(string key, string data) where TReadOnlyData : IEGSaveDataReadOnly, IEGSaveReadOnly, new()
        {
            TReadOnlyData readOnlyData = new TReadOnlyData();
            readOnlyData.InitReadOnly(data);
            if (!DataBaseReadOnly.ContainsKey(key))
            {
                DataBaseReadOnly.Add(key, readOnlyData);
            }
            else
            {
                DataBaseReadOnly[key] = readOnlyData;
            }
            return readOnlyData;
        }

        public TReadOnlyData ReadData<TReadOnlyData>(string key, byte[] data) where TReadOnlyData : IEGSaveDataReadOnly, IEGSaveReadOnly, new()
        {
            TReadOnlyData readOnlyData = new TReadOnlyData();
            readOnlyData.InitReadOnly(data);
            if (!DataBaseReadOnly.ContainsKey(key))
            {
                DataBaseReadOnly.Add(key, readOnlyData);
            }
            else
            {
                DataBaseReadOnly[key] = readOnlyData;
            }
            return readOnlyData;
        }

        public TSaveObject LoadObjectFile<TSaveObject>(string path) where TSaveObject:IEGSaveObject,IEGSave,new(){
            TSaveObject saveObject = new TSaveObject();
            saveObject.InitSave(path);
            if(!ObjectFiles.ContainsKey(path)){
                ObjectFiles.Add(path, saveObject);
            }else{
                ObjectFiles[path] = saveObject;
            }
            return saveObject;
        }

        public TReadOnlyObject ReadObject<TReadOnlyObject>(string key, string data) where TReadOnlyObject : IEGSaveObjectReadOnly, IEGSaveReadOnly, new()
        {
            TReadOnlyObject readOnlyObject = new TReadOnlyObject();
            readOnlyObject.InitReadOnly(data);
            if (!ObjectReadOnly.ContainsKey(key))
            {
                ObjectReadOnly.Add(key, readOnlyObject);
            }
            else
            {
                ObjectReadOnly[key] = readOnlyObject;
            }
            return readOnlyObject;
        }

        public TReadOnlyObject ReadObject<TReadOnlyObject>(string key,byte[] data) where TReadOnlyObject:IEGSaveObjectReadOnly,IEGSaveReadOnly,new(){
            TReadOnlyObject readOnlyObject = new TReadOnlyObject();
            readOnlyObject.InitReadOnly(data);
            if(!ObjectReadOnly.ContainsKey(key)){
                ObjectReadOnly.Add(key,readOnlyObject);
            }else{
                ObjectReadOnly[key] = readOnlyObject;
            }
            return readOnlyObject;
        }

        public void Unload(string keyOrPath){
            if(DataBaseReadOnly.ContainsKey(keyOrPath)){
                DataBaseReadOnly.Remove(keyOrPath);
            }else if(ObjectReadOnly.ContainsKey(keyOrPath)){
                ObjectReadOnly.Remove(keyOrPath);
            }else if(DataBaseFiles.ContainsKey(keyOrPath)){
                DataBaseFiles.Remove(keyOrPath);
            }else if(ObjectFiles.ContainsKey(keyOrPath)){
                ObjectFiles.Remove(keyOrPath);
            }else{
                throw new Exception("Key is not found!");
            }
        }
        #endregion

        #region Keys Operation

        /// <summary>
        /// Get all file keys which has been loaded.
        /// </summary>
        /// <returns></returns>
        public List<string> GetKeys(){
            List<string> keys = new List<string>();
            foreach(string key in DataBaseReadOnly.Keys){
                keys.Add(key);
            }
            foreach(string key in ObjectReadOnly.Keys){
                keys.Add(key);
            }
            foreach(string key in DataBaseFiles.Keys){
                keys.Add(key);
            }
            foreach(string key in ObjectFiles.Keys){
                keys.Add(key);
            }
            return keys;
        }

        /// <summary>
        /// Get all data file keys (or relational database in remote such as mysql) which has been loaded.
        /// </summary>
        /// <returns></returns>
        public List<string> GetDataKeys(){
            List<string> keys = new List<string>();
            foreach(string key in DataBaseReadOnly.Keys){
                keys.Add(key);
            }
            foreach(string key in DataBaseFiles.Keys){
                keys.Add(key);
            }
            return keys;
        }
        
        /// <summary>
        /// Get all object file keys (or key-value database in remote such as redis) which has been loaded.
        /// </summary>
        /// <returns></returns>
        public List<string> GetObjectKeys(){
            List<string> keys = new List<string>();
            foreach(string key in ObjectReadOnly.Keys){
                keys.Add(key);
            }
            foreach(string key in ObjectFiles.Keys){
                keys.Add(key);
            }
            return keys;
        }

        #endregion

        #region Get or Search data and object

        public bool ContainsObject(string path, string key)
        {
            if (ObjectFiles.ContainsKey(path))
            {
                return ObjectFiles[path].ContainsKey(key);
            }
            else if (ObjectReadOnly.ContainsKey(path))
            {
                return ObjectReadOnly[path].ContainsKey(key);
            }
            else
            {
                throw new Exception("File not loaded, you should use LoadObjectFile(key) or ReadObject(key) first.");
            }
        }

        public bool ContainsData(string path, string key)
        {
            if (DataBaseFiles.ContainsKey(path))
            {
                return DataBaseFiles[path].ContainsKey(key);
            }
            else if (DataBaseReadOnly.ContainsKey(path))
            {
                return DataBaseReadOnly[path].ContainsKey(key);
            }
            else
            {
                throw new Exception("File not loaded, you should use LoadDataFile(key) or ReadData(key) first.");
            }
        }

        public IEnumerable<string> ListObject(string path)
        {
            if (ObjectFiles.ContainsKey(path))
            {
                return ObjectFiles[path].GetKeys();
            }
            else if (ObjectReadOnly.ContainsKey(path))
            {
                return ObjectReadOnly[path].GetKeys();
            }
            else
            {
                throw new Exception("File not loaded, you should use LoadObjectFile(key) or ReadObject(key) first.");
            }
        }

        public IEnumerable<string> ListData(string path, string key)
        {
            if (DataBaseFiles.ContainsKey(path))
            {
                return DataBaseFiles[path].GetKeys();
            }
            else if (DataBaseReadOnly.ContainsKey(path))
            {
                return DataBaseReadOnly[path].GetKeys();
            }
            else
            {
                throw new Exception("File not loaded, you should use LoadDataFile(key) or ReadData(key) first.");
            }
        }

        public TObject GetObject<TObject>(string path, string key) where TObject : new()
        {
            if (ObjectFiles.ContainsKey(path))
            {
                return ObjectFiles[path].GetObject<TObject>(key);
            }
            else if (ObjectReadOnly.ContainsKey(path))
            {
                return ObjectReadOnly[path].GetObject<TObject>(key);
            }
            else
            {
                throw new Exception("File not loaded, you should use LoadObjectFile(key) or ReadObject(key) first.");
            }
        }
        public TData GetData<TData>(string keyOrPath,string key,int id) where TData : new(){
            if(DataBaseFiles.ContainsKey(keyOrPath)){
                return DataBaseFiles[keyOrPath].GetData<TData>(key,id);
            }else if(DataBaseReadOnly.ContainsKey(keyOrPath)){
                return DataBaseReadOnly[keyOrPath].GetData<TData>(key,id);
            }else{
                throw new Exception("File not loaded, you should use LoadDataFile(key) or ReadData(key,data) first.");
            }
        }

        public IEnumerable<TData> GetAllData<TData>(string keyOrPath,string key) where TData : new(){
            if(DataBaseFiles.ContainsKey(keyOrPath)){
                return DataBaseFiles[keyOrPath].GetAll<TData>(key);
            }else if(DataBaseReadOnly.ContainsKey(keyOrPath)){
                return DataBaseReadOnly[keyOrPath].GetAll<TData>(key);
            }else{
                throw new Exception("File not loaded, you should use LoadDataFile(key) or ReadData(key,data) first.");
            }
        }

        public IEnumerable<TData> FindData<TData>(string keyOrPath,string key,System.Linq.Expressions.Expression<Func<TData, bool>> expression) where TData : new(){
            if(DataBaseFiles.ContainsKey(keyOrPath)){
                return DataBaseFiles[keyOrPath].FindData<TData>(key,expression);
            }else if(DataBaseReadOnly.ContainsKey(keyOrPath)){
                return DataBaseReadOnly[keyOrPath].FindData<TData>(key,expression);
            }else{
                throw new Exception("File not loaded, you should use LoadDataFile(key) or ReadData(key,data) first.");
            }
        }
        #endregion
        
        #region Set or Add or Update data and object
        public void SetObject<TObject>(string path,string objectKey,TObject obj){
            if(ObjectFiles.ContainsKey(path)){
                ObjectFiles[path].SetObject(objectKey,obj);
            }else{
                throw new Exception("File not loaded, you should use LoadObjectFile(key) first.");
            }
        }

        public void SetData<TData>(string path,string dataKey,TData data,int id){
            if(DataBaseFiles.ContainsKey(path)){
                DataBaseFiles[path].SetData(dataKey,data,id);
            }else{
                throw new Exception("File not loaded, you should use LoadDataFile(path) first.");
            }
        }
        
        public void AddObject<TObject>(string path,string objectKey,TObject obj){
            if(ObjectFiles.ContainsKey(path)){
                ObjectFiles[path].AddObject(objectKey,obj);
            }else{
                throw new Exception("File not loaded, you should use LoadObjectFile(key) first.");
            }
        }
        public void AddData<TData>(string path,string dataKey,TData data){
            if(DataBaseFiles.ContainsKey(path)){
                DataBaseFiles[path].AddData(dataKey,data);
            }else{
                throw new Exception("File not loaded, you should use LoadDataFile(path) first.");
            }
        }
        public void AddData<TData>(string path,string dataKey,IEnumerable<TData> data){
            if(DataBaseFiles.ContainsKey(path)){
                DataBaseFiles[path].AddData(dataKey,data);
            }else{
                throw new Exception("File not loaded, you should use LoadDataFile(path) first.");
            }
        }
        public void UpdateObject<TObject>(string path,string objectKey,TObject obj){
            if(ObjectFiles.ContainsKey(path)){
                ObjectFiles[path].UpdateObject(objectKey,obj);
            }else{
                throw new Exception("File not loaded, you should use LoadObjectFile(key) first.");
            }
        }
        public void UpdateData<TData>(string path,string dataKey,TData data,int id){
            if(DataBaseFiles.ContainsKey(path)){
                DataBaseFiles[path].UpdateData(dataKey,data,id);
            }else{
                throw new Exception("File not loaded, you should use LoadDataFile(path) first.");
            }
        }
        public void RemoveObject<TObject>(string path,string objectKey){
            if(ObjectFiles.ContainsKey(path)){
                ObjectFiles[path].RemoveObject<TObject>(objectKey);
            }else{
                throw new Exception("File not loaded, you should use LoadObjectFile(key) first.");
            }
        }
        public void RemoveData(string path,string dataKey,int id){
            if(DataBaseFiles.ContainsKey(path)){
                DataBaseFiles[path].RemoveData(dataKey,id);
            }else{
                throw new Exception("File not loaded, you should use LoadDataFile(path) first.");
            }
        }

        #endregion

        #region Default Json Operation
        public void SetObjectToJson<TObject>(TObject obj){
            ObjectFiles["SaveData/DefaultJsonSave.json"].SetObject(typeof(TObject).ToString(),obj);
        }
        public TObject GetObjectFromJson<TObject>() where TObject : new(){
            return ObjectFiles["SaveData/DefaultJsonSave.json"].GetObject<TObject>(typeof(TObject).ToString());
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