using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;
using System.Configuration;

namespace EGFramework
{
    /// <summary>
    /// CSV Save tools, support read and write CSV file.The dataKey param is not use.please use "" or any string.
    /// </summary>
    public class EGCsvSave : IEGSaveData, IEGSave, IEGSaveReadOnly
    {
        public bool IsReadOnly { get; set; }
        public Encoding StringEncoding { set; get; } = Encoding.UTF8;
        private string DefaultPath { set; get; }
        private List<string[]> CsvDataBlock { get; set; }
        private Dictionary<string,int> CsvDataHeader = new Dictionary<string,int>();
        public IOCContainer TypeDataContainer = new IOCContainer();
        private string ReadText { set; get; }


        public void InitSave(string path)
        {
            ReadDataBlock(path);
        }

        public void InitReadOnly(string data)
        {
            ReadText = data;
            if(ReadText != null || ReadText != ""){
                CsvDataBlock = GetCSVDataBlockFromText(ReadText,out CsvDataHeader);
            }
        }

        public void InitReadOnly(byte[] data)
        {
            ReadText = StringEncoding.GetString(data);
            if(ReadText != null || ReadText != ""){
                CsvDataBlock = GetCSVDataBlockFromText(ReadText,out CsvDataHeader);
            }
        }

        public void ReadDataBlock(string path){
            DefaultPath = path;
            try
            {
                if (!File.Exists(path))
                {
                    FileStream newCsvFile = File.Create(path);
                    CsvDataBlock = new List<string[]>();
                    newCsvFile.Close();
                    newCsvFile.Dispose();
                    return;
                }
                FileStream fileStream = new FileStream(path,FileMode.Open);
                byte[] buffer = new byte[fileStream.Length];
                fileStream.Read(buffer, 0, (int)fileStream.Length);
                fileStream.Close();
                fileStream.Dispose();
                ReadText = StringEncoding.GetString(buffer);
            }
            catch (System.Exception e)
            {
                EG.Print("e:" + e);
                throw;
            }
            if(ReadText != null || ReadText != ""){
                CsvDataBlock = GetCSVDataBlockFromText(ReadText,out CsvDataHeader);
            }
        }

        public void WriteDataBlock(string path){
            try
            {
                FileStream fileStream = File.Create(path);
                string writeText = "";
                string headerText = "";
                foreach(string headStr in CsvDataHeader.Keys){
                    headerText+=headStr + ",";
                }
                headerText = headerText.Remove(headerText.Length-1,1);
                writeText = headerText + "\n";
                foreach(string[] lineData in CsvDataBlock){
                    string lineText = "";
                    foreach(string singleData in lineData){
                        lineText += singleData + ",";
                    }
                    lineText = lineText.Remove(lineText.Length-1,1);
                    writeText += lineText + "\n";
                }
                writeText = writeText.Remove(writeText.Length-1,1);
                byte[] data = StringEncoding.GetBytes(writeText);
                fileStream.Write(data,0,data.Length);
                fileStream.Close();
                fileStream.Dispose();
            }
            catch (System.Exception e)
            {
                EG.Print("e:" + e);
                throw;
            }
        }

        public List<string[]> GetCSVDataBlockFromText(string text,out Dictionary<string,int> header){
            List<string[]> csvBlock = new List<string[]>();
            string[] lineData = text.Split('\n');
            string[] headerStr = lineData[0].Split(',');
            header = new Dictionary<string,int>();
            for (int i = 0; i < headerStr.Length; i++)
            {
                header.Add(headerStr[i],i);
            }
            for (int lineID = 0; lineID < lineData.Length; lineID++)
            {
                if (lineID!=0){
                    csvBlock.Add(lineData[lineID].Split(','));
                }
            }
            return csvBlock;
        }

        public string[] ReadLine(int id){
            if(CsvDataBlock.Count()>0){
                return CsvDataBlock[id];
            }else{
                return null;
            }
        }

        public void WriteLine(string[] lineData){
            CsvDataBlock.Add(lineData);
            this.WriteDataBlock(DefaultPath);
        }

        public void SetData<TData>(string dataKey, TData data, object id)
        {
            if(IsReadOnly){
                throw new Exception("This file is readonly! can't set any data to file.");
            }
            bool IsAdd = false;
            int dataID = 0;
            if(id.GetType()==typeof(int)){
                dataID = (int)id;
            }else if(int.TryParse(id.ToString() ,out dataID)){
                throw new Exception("Id cannot be convert to int!");
            }
            if(dataID>=CsvDataBlock.Count() || dataID < 0){
                IsAdd = true;
            }
            string[] csvSet = new string[CsvDataHeader.Keys.Count()];
            foreach(PropertyInfo property in data.GetType().GetProperties()){
                CsvParamAttribute csvParam = property.GetCustomAttribute<CsvParamAttribute>();
                if(csvParam != null && CsvDataHeader.ContainsKey(csvParam._name)){
                    csvSet[CsvDataHeader[csvParam._name]] = property.GetValue(data).ToString();
                }
            }
            if(IsAdd){
                CsvDataBlock.Add(csvSet);
            }else{
                CsvDataBlock[dataID] = csvSet;
            }
            this.WriteDataBlock(DefaultPath);
        }

        public TData GetData<TData>(string dataKey, object id) where TData : new()
        {
            TData data = new TData();
            int dataID = 0;
            if(id.GetType()==typeof(int)){
                dataID = (int)id;
            }else if(int.TryParse(id.ToString() ,out dataID)){
                throw new Exception("Id cannot be convert to int!");
            }
            if(dataID>=CsvDataBlock.Count()){
                throw new IndexOutOfRangeException("Parameter index is out of range.");
            }
            foreach(PropertyInfo property in data.GetType().GetProperties()){
                CsvParamAttribute csvParam = property.GetCustomAttribute<CsvParamAttribute>();
                if(csvParam != null && CsvDataHeader.ContainsKey(csvParam._name)){
                    string valueStr = CsvDataBlock[dataID][CsvDataHeader[csvParam._name]];
                    if(property.PropertyType==typeof(string)){
                        property.SetValue(data,valueStr);
                    }else{
                        property.SetValue(data,Convert.ChangeType(valueStr,property.PropertyType));
                    }
                }
            }
            return data;
        }
        public IEnumerable<TData> GetAll<TData>(string dataKey) where TData : new()
        {
            // throw new NotImplementedException();
            List<TData> DataList = new List<TData>();
            PropertyInfo[] properties = typeof(TData).GetProperties();
            for (int dataID = 0; dataID < CsvDataBlock.Count(); dataID++){
                TData data = new TData();
                foreach(PropertyInfo property in properties){
                    CsvParamAttribute csvParam = property.GetCustomAttribute<CsvParamAttribute>();
                    if(csvParam != null && CsvDataHeader.ContainsKey(csvParam._name)){
                        string valueStr = CsvDataBlock[dataID][CsvDataHeader[csvParam._name]];
                        if(property.PropertyType==typeof(string)){
                            property.SetValue(data,valueStr);
                        }else{
                            property.SetValue(data,Convert.ChangeType(valueStr,property.PropertyType));
                        }
                    }
                }
                DataList.Add(data);
            }
            TypeDataContainer.Register(DataList);
            return DataList;
        }

        public IEnumerable<TData> GetPage<TData>(string dataKey, int pageIndex, int pageSize) where TData : new()
        {
            if(pageIndex <= 0){
                pageIndex = 1;
            }
            int startPointer = (pageIndex - 1) * pageSize;
            List<TData> DataList = new List<TData>();
            PropertyInfo[] properties = typeof(TData).GetProperties();
            for (int dataID = startPointer; dataID < startPointer+pageSize; dataID++){
                TData data = new TData();
                foreach(PropertyInfo property in properties){
                    CsvParamAttribute csvParam = property.GetCustomAttribute<CsvParamAttribute>();
                    if(csvParam != null && CsvDataHeader.ContainsKey(csvParam._name)){
                        string valueStr = CsvDataBlock[dataID][CsvDataHeader[csvParam._name]];
                        if(property.PropertyType==typeof(string)){
                            property.SetValue(data,valueStr);
                        }else{
                            property.SetValue(data,Convert.ChangeType(valueStr,property.PropertyType));
                        }
                    }
                }
                DataList.Add(data);
            }
            TypeDataContainer.Register(DataList);
            return DataList;
        }
        
        public IEnumerable<TData> FindData<TData>(string dataKey, Expression<Func<TData, bool>> expression) where TData : new()
        {
            List<TData> sourceList;
            if(TypeDataContainer.self.ContainsKey(typeof(List<TData>))){
                sourceList = TypeDataContainer.Get<List<TData>>();
            }else{
                sourceList = (List<TData>)GetAll<TData>(dataKey);
            }
            return sourceList.Where(expression.Compile());
        }
        
        public IEnumerable<TData> FindData<TData>(string dataKey, string columnName, string keyWords) where TData : new()
        {
            // throw new NotImplementedException();
            List<TData> DataList = new List<TData>();
            PropertyInfo[] properties = typeof(TData).GetProperties();
            if (!CsvDataHeader.ContainsKey(columnName))
            {
                EG.Print("Column not found!");
                return null;
            }
            for (int dataID = 0; dataID < CsvDataBlock.Count(); dataID++)
            {
                TData data = new TData();
                string compareStr = CsvDataBlock[dataID][CsvDataHeader[columnName]];
                if (!compareStr.Contains(keyWords))
                {
                    continue;
                }
                foreach (PropertyInfo property in properties)
                {
                    CsvParamAttribute csvParam = property.GetCustomAttribute<CsvParamAttribute>();
                    if (csvParam != null && CsvDataHeader.ContainsKey(csvParam._name))
                    {
                        string valueStr = CsvDataBlock[dataID][CsvDataHeader[csvParam._name]];
                        if (property.PropertyType == typeof(string))
                        {
                            property.SetValue(data, valueStr);
                        }
                        else
                        {
                            property.SetValue(data, Convert.ChangeType(valueStr, property.PropertyType));
                        }
                    }
                }
                DataList.Add(data);
            }
            return DataList;
        }

        public void AddData<TData>(string dataKey, TData data)
        {
            if (IsReadOnly)
            {
                throw new Exception("This file is readonly! can't set any data to file.");
            }
            string[] csvSet = new string[CsvDataHeader.Keys.Count()];
            foreach (PropertyInfo property in data.GetType().GetProperties())
            {
                CsvParamAttribute csvParam = property.GetCustomAttribute<CsvParamAttribute>();
                if (csvParam != null && CsvDataHeader.ContainsKey(csvParam._name))
                {
                    csvSet[CsvDataHeader[csvParam._name]] = property.GetValue(data).ToString();
                }
            }
            CsvDataBlock.Add(csvSet);
            this.WriteDataBlock(DefaultPath);
        }

        public void AddData<TData>(string dataKey, IEnumerable<TData> dataSet)
        {
            if(IsReadOnly){
                throw new Exception("This file is readonly! can't set any data to file.");
            }
            foreach(TData data in dataSet){
                string[] csvSet = new string[CsvDataHeader.Keys.Count()];
                foreach(PropertyInfo property in data.GetType().GetProperties()){
                    CsvParamAttribute csvParam = property.GetCustomAttribute<CsvParamAttribute>();
                    if(csvParam != null && CsvDataHeader.ContainsKey(csvParam._name)){
                        csvSet[CsvDataHeader[csvParam._name]] = property.GetValue(data).ToString();
                    }
                }
                CsvDataBlock.Add(csvSet);
            }
            this.WriteDataBlock(DefaultPath);
        }

        public void AddData(string datakey, Dictionary<string, object> data)
        {
            if(IsReadOnly){
                throw new Exception("This file is readonly! can't set any data to file.");
            }
            string[] csvSet = new string[CsvDataHeader.Keys.Count()];
            foreach(KeyValuePair<string,object> param in data){
                if(CsvDataHeader.ContainsKey(param.Key)){
                    csvSet[CsvDataHeader[param.Key]] = param.Value.ToString();
                }
            }
            CsvDataBlock.Add(csvSet);
            this.WriteDataBlock(DefaultPath);
        }

        public void AddGroup(string datakey, List<Dictionary<string, object>> dataGroup)
        {
            if (IsReadOnly)
            {
                throw new Exception("This file is readonly! can't set any data to file.");
            }
            foreach (Dictionary<string, object> data in dataGroup)
            {
                if (CsvDataHeader.Count == 0)
                {
                    int id = 0;
                    foreach (string key in data.Keys)
                    {
                        CsvDataHeader.Add(key, id);
                        id++;
                    }
                }
                string[] csvSet = new string[CsvDataHeader.Keys.Count()];
                foreach (KeyValuePair<string, object> param in data)
                {
                    if (CsvDataHeader.ContainsKey(param.Key))
                    {
                        csvSet[CsvDataHeader[param.Key]] = param.Value.ToString();
                    }
                }
                CsvDataBlock.Add(csvSet);
            }
            this.WriteDataBlock(DefaultPath);
        }

        public int RemoveData(string dataKey, object id)
        {
            if (IsReadOnly)
            {
                throw new Exception("This file is readonly! can't set any data to file.");
            }
            bool IsAdd = false;
            int dataID = 0;
            if (id.GetType() == typeof(int))
            {
                dataID = (int)id;
            }
            else if (int.TryParse(id.ToString(), out dataID))
            {
                throw new Exception("Id cannot be convert to int!");
            }
            if (dataID >= CsvDataBlock.Count() || dataID < 0)
            {
                IsAdd = true;
            }
            if (IsAdd)
            {
                return 0;
            }
            else
            {
                CsvDataBlock.RemoveAt(dataID);
            }
            this.WriteDataBlock(DefaultPath);
            return 1;
        }

        public void UpdateData<TData>(string dataKey, TData data, object id)
        {
            if(IsReadOnly){
                throw new Exception("This file is readonly! can't set any data to file.");
            }
            bool IsAdd = false;
            int dataID = 0;
            if(id.GetType()==typeof(int)){
                dataID = (int)id;
            }else if(int.TryParse(id.ToString() ,out dataID)){
                throw new Exception("Id cannot be convert to int!");
            }
            if(dataID>=CsvDataBlock.Count() || dataID < 0){
                IsAdd = true;
            }
            string[] csvSet = new string[CsvDataHeader.Keys.Count()];
            foreach(PropertyInfo property in data.GetType().GetProperties()){
                CsvParamAttribute csvParam = property.GetCustomAttribute<CsvParamAttribute>();
                if(csvParam != null && CsvDataHeader.ContainsKey(csvParam._name)){
                    csvSet[CsvDataHeader[csvParam._name]] = property.GetValue(data).ToString();
                }
            }
            if(!IsAdd){
                CsvDataBlock[dataID] = csvSet;
            }else{
                throw new Exception("Data not found!");
            }
            this.WriteDataBlock(DefaultPath);
        }

        
        public void UpdateData(string dataKey, Dictionary<string, object> data, object id)
        {
            if(IsReadOnly){
                throw new Exception("This file is readonly! can't set any data to file.");
            }
            bool IsAdd = false;
            int dataID = 0;
            if(id.GetType()==typeof(int)){
                dataID = (int)id;
            }else if(int.TryParse(id.ToString() ,out dataID)){
                throw new Exception("Id cannot be convert to int!");
            }
            if(dataID>=CsvDataBlock.Count() || dataID < 0){
                IsAdd = true;
            }
            string[] csvSet = new string[CsvDataHeader.Keys.Count()];
            foreach(KeyValuePair<string,object> param in data){
                if(CsvDataHeader.ContainsKey(param.Key)){
                    csvSet[CsvDataHeader[param.Key]] = param.Value.ToString();
                }
            }
            if(!IsAdd){
                CsvDataBlock[dataID] = csvSet;
            }else{
                throw new Exception("Data not found!");
            }
            this.WriteDataBlock(DefaultPath);
        }

        public IEnumerable<string> GetKeys()
        {
            return CsvDataHeader.Keys;
        }

        public bool ContainsKey(string dataKey)
        {
            return CsvDataHeader.ContainsKey(dataKey);
        }

        public bool ContainsData(string dataKey, object id)
        {
            return CsvDataBlock.Count() > 0 && id.GetType() == typeof(int) && (int)id < CsvDataBlock.Count();
        }

        public int GetDataCount(string dataKey)
        {
            return CsvDataBlock.Count();
        }

    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class CsvParamAttribute: Attribute{
        public string _name { set; get; }
        public CsvParamAttribute(string name){
            this._name = name;
        }
    }
}