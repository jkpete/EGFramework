using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EGFramework
{
    /// Pointer max length is 4294967295.
    /// Data protocol:
    /// [XX XX XX XX](Pointer length) [XX XX XX XX]...[XX XX XX XX](Pointers) [DataBlock 1]...[DataBlock N](Data)
    /// 1. The first (four byte) is the length of the data pointers (uint type).
    /// 2. The pointer List is the position to the data block.
    /// 3. The data block is the data you want to save.
    public interface IEGByteObject{
        byte[] GetBytes();
        void SetBytes(byte[] byteData);
    }
    [Obsolete("This class is not comlpete, please not use it!")]
    public class EGByteObjectSave : IEGSave,IEGSaveObject
    {
        public Encoding StringEncoding { set; get; } = Encoding.ASCII;
        private string DefaultPath { set; get; }
        private uint PointerLength { get; set; }
        private uint[] Pointer { get; set; }
        private Dictionary<uint,byte[]> Data { get; set; }
        private byte[] _Data;

        public void ReadDataBlock(string path){
            DefaultPath = path;
            try
            {
                FileStream fileStream = new FileStream(path,FileMode.OpenOrCreate);
                byte[] buffer = new byte[fileStream.Length];
                fileStream.Read(buffer, 0, (int)fileStream.Length);
                fileStream.Close();
                fileStream.Dispose();
                _Data = buffer;
            }
            catch (System.Exception e)
            {
                EG.Print("e:" + e);
                throw;
            }
        }
        public void WriteDataBlock(string path){
            try
            {
                FileStream fileStream = File.Create(path);
                fileStream.Write(_Data,0,_Data.Length);
                fileStream.Close();
                fileStream.Dispose();
            }
            catch (System.Exception e)
            {
                EG.Print("e:" + e);
                throw;
            }
        }

        public void InitSaveFile(string path)
        {
            ReadDataBlock(path);
        }

        /// <summary>
        /// Set object to the file,the pointer 
        /// </summary>
        /// <param name="objectKey"></param>
        /// <param name="obj"></param>
        /// <typeparam name="TObject"></typeparam>
        public void SetObject<TObject>(string objectKey , TObject obj)
        {
            if(typeof(TObject).GetInterfaces().Contains(typeof(IEGByteObject))){
                _Data = ((IEGByteObject)obj).GetBytes();
            }else{
                throw new Exception("This byte class cannot be serialized! you should implement IRequest first!");
            }
            WriteDataBlock(DefaultPath);
        }

        public TObject GetObject<TObject>(string objectKey) where TObject : new()
        {
            if(typeof(TObject).GetInterfaces().Contains(typeof(IEGByteObject))){
                TObject result = new TObject();
                ((IEGByteObject)result).SetBytes(_Data);
                return result;
            }else{
                throw new Exception("This byte class cannot be serialized! you should implement IRequest first!");
            }
        }

        public void RemoveObject<TObject>(string objectKey)
        {
            throw new NotImplementedException();
        }

        public void AddObject<TObject>(string objectKey, TObject obj)
        {
            throw new NotImplementedException();
        }

        public void UpdateObject<TObject>(string objectKey, TObject obj)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetKeys()
        {
            throw new NotImplementedException();
        }

    }

    public interface IEGByteInit{
        void Init(byte[] data);
    }
}