using System.Collections.Generic;
using System.Text;
using System.IO;

namespace EGFramework{
    public class EGFileStream : IEGFramework, IModule,IProtocolSend,IProtocolReceived
    {
        public Encoding StringEncoding { set; get; } = Encoding.UTF8;

        public Queue<ResponseMsg> ResponseMsgs { set; get; } = new Queue<ResponseMsg>();

        public void Init()
        {
            this.EGRegisterSendAction(request=>{
                if(request.protocolType == ProtocolType.FileStream){
                    if(request.req.ToProtocolData() != "" && request.req.ToProtocolData() != null){
                        this.SendStringData(request.sender,request.req.ToProtocolData());
                    }
                    if(request.req.ToProtocolByteData().Length > 0 && request.req.ToProtocolByteData() != null){
                        this.SendByteData(request.sender,request.req.ToProtocolByteData());
                    }
                }
            });
        }

        public IArchitecture GetArchitecture()
        {
            return EGArchitectureImplement.Interface;
        }

        public async void SendByteData(string destination, byte[] data)
        {
            string path = destination;
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                FileStream fileStream = File.Create(path);
                await fileStream.WriteAsync(data,0,data.Length);
                fileStream.Close();
                await fileStream.DisposeAsync();
            }
            catch (System.Exception)
            {
                throw;
            }
            //throw new System.NotImplementedException();
        }

        public void SendStringData(string destination, string data)
        {
            SendByteData(destination,StringEncoding.GetBytes(data));
            //throw new System.NotImplementedException();
        }

        /// <summary>
        /// Read file by FileStream and return received a byte data or string data from file
        /// </summary>
        /// <param name="path">File Path</param>
        public async void ReadFromFile(string path){
            try
            {
                FileStream fileStream = new FileStream(path,FileMode.Open);
                byte[] buffer = new byte[fileStream.Length];
                await fileStream.ReadAsync(buffer, 0, (int)fileStream.Length);
                fileStream.Close();
                await fileStream.DisposeAsync();
                string data = StringEncoding.GetString(buffer);
                ResponseMsg receivedMsgs = new ResponseMsg(data,buffer,path, ProtocolType.FileStream);
                ResponseMsgs.Enqueue(receivedMsgs);
            }
            catch (System.Exception e)
            {
                EG.Print("e:" + e);
                throw;
            }
        }

        public void SetEncoding(Encoding textEncoding)
        {
            this.StringEncoding = textEncoding;
        }

        public Queue<ResponseMsg> GetReceivedMsg()
        {
            return this.ResponseMsgs;
        }
    }
    public static class CanGetEGFileStreamExtension{
        public static EGFileStream EGFileStream(this IEGFramework self){
            return self.GetModule<EGFileStream>();
        }

        public static void EGReadFromFile(this IEGFramework self,string path){
            self.GetModule<EGFileStream>().ReadFromFile(path);
        }
    }
}