using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EGFramework
{
    public class EGCsvSave : IEGSaveData, IEGSave
    {
        public Encoding StringEncoding { set; get; } = Encoding.UTF8;
        private string DefaultPath { set; get; }
        private List<string[]> CsvDataBlock { get; set; }
        private string[] CsvDataHeader;
        private string ReadText { set; get; }
        public void InitSaveFile(string path)
        {
            ReadDataBlock(path);
        }

        public void ReadDataBlock(string path){
            DefaultPath = path;
            try
            {
                FileStream fileStream = new FileStream(path,FileMode.Open);
                byte[] buffer = new byte[fileStream.Length];
                fileStream.Read(buffer, 0, (int)fileStream.Length);
                fileStream.Close();
                fileStream.Dispose();
                ReadText = StringEncoding.GetString(buffer);
            }
            catch (System.Exception e)
            {
                Godot.GD.Print("e:" + e);
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
                foreach(string headers in CsvDataHeader){
                    headerText+=headers + ",";
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
                Godot.GD.Print("e:" + e);
                throw;
            }
        }

        public List<string[]> GetCSVDataBlockFromText(string text,out string[] header){
            List<string[]> csvBlock = new List<string[]>();
            string[] lineData = text.Split('\n');
            header = lineData[0].Split(',');
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
            throw new NotImplementedException();
        }
        public TData GetData<TData>(string dataKey, object id) where TData : new()
        {
            throw new NotImplementedException();
        }
        public IEnumerable<TData> GetAll<TData>(string dataKey) where TData : new()
        {
            throw new NotImplementedException();
        }
        
        public IEnumerable<TData> FindData<TData>(string dataKey, Expression<Func<TData, bool>> expression) where TData : new()
        {
            throw new NotImplementedException();
        }
    }

    public class CsvParamAttribute: Attribute{
        public CsvParamAttribute(string name){

        }
    }
}