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
        private IEnumerable<string[]> CsvDataBlock { get; set; }
        private string[] CsvDataHeader;
        private string ReadText { set; get; }
        public void InitSaveFile(string path)
        {
            
        }

        public void ReadDataBlock(string path){
            try
            {
                FileStream fileStream = new FileStream(path,FileMode.Open);
                byte[] buffer = new byte[fileStream.Length];
                fileStream.Read(buffer, 0, (int)fileStream.Length);
                fileStream.Close();
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
        public IEnumerable<string[]> GetCSVDataBlockFromText(string text,out string[] header){
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