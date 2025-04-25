using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace EGFramework{
    public class EGProcess : IEGFramework, IModule, IProtocolSend, IProtocolReceived
    {
        public Dictionary<string,Process> Processes { set; get; } = new Dictionary<string, Process>();
        
        public string ErrorLogs { set; get; }

        public Encoding StringEncoding { set; get; } = Encoding.UTF8;

        public ConcurrentQueue<ResponseMsg> ResponseMsgs { set; get; } = new ConcurrentQueue<ResponseMsg>();

        public void Init()
        {
            this.EGRegisterSendAction(request=>{
                if(request.protocolType == ProtocolType.Process){
                    if(request.req.ToProtocolData() != null && request.req.ToProtocolData() != ""){
                        this.SendStringData(request.sender,request.req.ToProtocolData());
                    }
                    if(request.req.ToProtocolByteData() != null && request.req.ToProtocolByteData().Length > 0){
                        this.SendByteData(request.sender,request.req.ToProtocolByteData());
                    }
                }
            });
        }

        /// <summary>
        /// Connect process program with check if target program is exist.
        /// </summary>
        public void InitProcess(string processName){
            try{
                if(!Processes.ContainsKey(processName)){
                    Process process = new Process();
                    if(processName.GetStrFrontSymbol(' ') == ""){
                        process.StartInfo.FileName = processName.GetStrBehindSymbol(' ');
                    }else{
                        process.StartInfo.FileName = processName.GetStrFrontSymbol(' ');
                        process.StartInfo.Arguments = processName.GetStrBehindSymbol(' ');; // Add any arguments if needed
                    }
                    EG.Print("[open process]"+process.StartInfo.FileName+" "+process.StartInfo.Arguments);
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardInput = true;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.StartInfo.CreateNoWindow = true;
                    process.EnableRaisingEvents = true;
                    process.OutputDataReceived += (sender, e) => {
                        if (!string.IsNullOrEmpty(e.Data)) {
                            ResponseMsgs.Enqueue(new ResponseMsg { sender = processName, stringData = e.Data });
                            EG.Print("[process output]"+processName+" "+e.Data);
                        }
                    };
                    process.Exited += (sender, e) => {
                        Processes.Remove(processName);
                    };
                    Processes.Add(processName,process);
                    process.Start();
                }
            }
            catch(Exception e){
                ErrorLogs = "[open process error]" + e.ToString();
                EG.Print(ErrorLogs);
            }
        }

        public void CloseProcess(string processName){
            if(Processes.ContainsKey(processName)){
                Processes[processName].Kill();
                Processes.Remove(processName);
                EG.Print("[close process]"+processName);
            }
        }
        
        public void SetEncoding(Encoding textEncoding)
        {
            this.StringEncoding = textEncoding;
        }

        public ConcurrentQueue<ResponseMsg> GetReceivedMsg()
        {
            return this.ResponseMsgs;
        }

        public void SendByteData(string destination, byte[] data)
        {
            Processes[destination].StandardInput.WriteLine(data);
        }

        public void SendStringData(string destination, string data)
        {
            Processes[destination].StandardInput.WriteLine(data);
        }

        
        public IArchitecture GetArchitecture()
        {
            return EGArchitectureImplement.Interface;
        }
    }
    public static class CanGetEGProcessExtension{
        public static EGProcess EGProcess(this IEGFramework self){
            return self.GetModule<EGProcess>();
        }

        public static Process EGGetSshClient(this IEGFramework self,string processName){
            return self.GetModule<EGProcess>().Processes[processName];
        }
    }
}