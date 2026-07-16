using System;

namespace EGFramework
{
    #region AbstractClass
    
    public class BaseJsonResponse : IResponse
    {
        private string ExceptionMsg;
        public virtual string ToProtocolData()
        {
            return "";
        }
        public virtual bool TrySetData(string json,byte[] bytes)
        {
            try
            {
                return true;
            }
            catch (Exception e)
            {
                ExceptionMsg = e.ToString();
                //PrintErr(ExceptionMsg);
                return false;
            }
        }
    }
    
    public class StringRequest : IRequest
    {
        private string RequestStr;
        public StringRequest() {
            RequestStr = "No message";
        }
        public StringRequest(string str) {
            RequestStr = str;
        }

        public byte[] ToProtocolByteData()
        {
            return null;
        }

        public string ToProtocolData()
        {
            return RequestStr;
        }
    }

    public class ByteRequest : IRequest
    {
        private byte[] Data;
        public ByteRequest(byte[] data) {
            Data = data;
        }

        public byte[] ToProtocolByteData()
        {
            return Data;
        }

        public string ToProtocolData()
        {
            return null;
        }
    }
    #endregion
}