using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace EGFramework
{
    public class EGByteSave : IEGSave,IEGSaveObject
    {
        public Encoding StringEncoding { set; get; } = Encoding.ASCII;
        private string DefaultPath { set; get; }
        private byte[] Data { get; set; }

        public void ReadDataBlock(string path){
            DefaultPath = path;
            try
            {
                FileStream fileStream = new FileStream(path,FileMode.Open);
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
            // throw new NotImplementedException();
        }

        public TObject GetObject<TObject>(string objectKey) where TObject : new()
        {
            throw new NotImplementedException();
        }

        public int GetDataLength(Type type){
            
            switch(type){

                default: return 0 ;
            }
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ByteParamAttribute: Attribute{
        public int _pointer { set; get; }
        public int _length { set; get; }
        public ByteParamAttribute(int pointer,int length = 0){
            this._pointer = pointer;
            this._length = length;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class ByteClassAttribute:Attribute{
        public int _length { set; get; }
        public ByteClassAttribute(int length){
            this._length = length;
        }
    }

    public interface IEGByteInit{
        void Init(byte[] data);
    }
}