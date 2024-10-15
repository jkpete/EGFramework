using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace EGFramework
{
    [Obsolete("this idea can be replaced by EGFileStream")]
    public class EGByteSave : IEGSave,IEGSaveObject
    {
        public Encoding StringEncoding { set; get; } = Encoding.ASCII;
        private string DefaultPath { set; get; }
        private byte[] Data { get; set; }

        public void ReadDataBlock(string path){
            DefaultPath = path;
            try
            {
                FileStream fileStream = new FileStream(path,FileMode.OpenOrCreate);
                byte[] buffer = new byte[fileStream.Length];
                fileStream.Read(buffer, 0, (int)fileStream.Length);
                fileStream.Close();
                fileStream.Dispose();
                Data = buffer;
            }
            catch (System.Exception e)
            {
                Godot.GD.Print("e:" + e);
                throw;
            }
        }
        public void WriteDataBlock(string path){
            try
            {
                FileStream fileStream = File.Create(path);
                fileStream.Write(Data,0,Data.Length);
                fileStream.Close();
                fileStream.Dispose();
            }
            catch (System.Exception e)
            {
                Godot.GD.Print("e:" + e);
                throw;
            }
        }

        public void InitSaveFile(string path)
        {
            ReadDataBlock(path);
        }

        public void SetObject<TObject>(string objectKey , TObject obj)
        {
            if(typeof(TObject).GetInterfaces().Contains(typeof(IRequest))){
                Data = ((IRequest)obj).ToProtocolByteData();
            }else{
                throw new Exception("This byte class cannot be serialized! you should implement IRequest first!");
            }
            WriteDataBlock(DefaultPath);
        }

        public TObject GetObject<TObject>(string objectKey) where TObject : new()
        {
            if(typeof(TObject).GetInterfaces().Contains(typeof(IResponse))){
                TObject result = new TObject();
                ((IResponse)result).TrySetData(StringEncoding.GetString(Data),Data);
                return result;
            }else{
                throw new Exception("This byte class cannot be serialized! you should implement IRequest first!");
            }
        }
    }

    public interface IEGByteInit{
        void Init(byte[] data);
    }
}