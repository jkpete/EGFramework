using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Renci.SshNet;

namespace EGFramework{
    public class EGSsh : IModule, IProtocolSend, IProtocolReceived
    {
        public Dictionary<string,SshClient> SshClientDevices { set; get; } = new Dictionary<string, SshClient>();

        public string ErrorLogs { set; get; }

        public Encoding StringEncoding { set; get; } = Encoding.UTF8;

        public Queue<ResponseMsg> ResponseMsgs { set; get; } = new Queue<ResponseMsg>();

        public int TimeOutDelay = 5000;
        public void Init()
        {
            this.EGRegisterSendAction(request=>{
                if(request.protocolType == ProtocolType.SSHClient){
                    if(request.req.ToProtocolData() != null && request.req.ToProtocolData() != ""){
                        this.SendStringData(request.sender,request.req.ToProtocolData());
                    }else if(request.req.ToProtocolByteData() != null && request.req.ToProtocolByteData().Length > 0){
                        this.SendByteData(request.sender,request.req.ToProtocolByteData());
                    }
                }
            });
        }

        /// <summary>
        /// Connect Ssh Server by using username and passwd
        /// </summary>
        public async Task<bool> ConnectSsh(string host,string username,string password){
            try{
                //Reconnect
                if(SshClientDevices.ContainsKey(host)){
                    if(SshClientDevices[host].IsConnected){
                        return true;
                    }else{
                        CancellationTokenSource sourceReconnect = new CancellationTokenSource();
                        CancellationToken tokenReconnect = sourceReconnect.Token;
                        tokenReconnect.Register(() => EG.Print("Ssh connect timeout!"));
                        await SshClientDevices[host].ConnectAsync(tokenReconnect);
                        if(!SshClientDevices[host].IsConnected){
                            return false;
                        }else{
                            return true;
                        }
                    }
                }
                //First Connect
                SshClient client = new SshClient(host, username, password);
                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;
                token.Register(() => EG.Print("Ssh connect timeout!"));
                source.CancelAfter(TimeOutDelay);
                await client.ConnectAsync(token);
                if(!client.IsConnected){
                    return false;
                }else{
                    SshClientDevices.Add(host,client);
                }
                return true;
            }
            catch(Exception e){
                ErrorLogs = "[ssh connect error]" + e.ToString();
                return false;
            }
        }

        /// <summary>
        /// Connect Ssh Server by using private key file
        /// </summary>
        public async Task<bool> ConnectSsh(string host,string username,PrivateKeyFile keyFile){
            try{
                //Reconnect
                if(SshClientDevices.ContainsKey(host)){
                    if(SshClientDevices[host].IsConnected){
                        return true;
                    }else{
                        CancellationTokenSource sourceReconnect = new CancellationTokenSource();
                        CancellationToken tokenReconnect = sourceReconnect.Token;
                        tokenReconnect.Register(() => Godot.GD.Print("Ssh connect timeout!"));
                        await SshClientDevices[host].ConnectAsync(tokenReconnect);
                        if(!SshClientDevices[host].IsConnected){
                            return false;
                        }else{
                            return true;
                        }
                    }
                }
                //First Connect
                SshClient client = new SshClient(host, username, keyFile);
                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;
                token.Register(() => Godot.GD.Print("Ssh connect timeout!"));
                source.CancelAfter(TimeOutDelay);
                await client.ConnectAsync(token);
                if(!client.IsConnected){
                    return false;
                }else{
                    SshClientDevices.Add(host,client);
                }
                return true;
            }
            catch(Exception e){
                ErrorLogs = "[ssh connect error]" + e.ToString();
                return false;
            }
        }


        public void DisconnectSsh(string host){
            if(SshClientDevices.ContainsKey(host)){
                if (SshClientDevices[host].IsConnected)
                {
                    SshClientDevices[host].Disconnect();
                    SshClientDevices[host].Dispose();
                    SshClientDevices.Remove(host);
                }
            }
        }

        public Queue<ResponseMsg> GetReceivedMsg()
        {
            return ResponseMsgs;
        }
        
        public void SendByteData(string destination, byte[] data)
        {
            if(SshClientDevices.ContainsKey(destination)){
                if (SshClientDevices[destination].IsConnected)
                {
                    SshCommand cmd = SshClientDevices[destination].RunCommand(StringEncoding.GetString(data));
                    ResponseMsg receivedMsgs = new ResponseMsg(cmd.Result,StringEncoding.GetBytes(cmd.Result),destination, ProtocolType.SSHClient);
                    ResponseMsgs.Enqueue(receivedMsgs);
                }
            }
        }

        public void SendStringData(string destination, string data)
        {
            if(SshClientDevices.ContainsKey(destination)){
                if (SshClientDevices[destination].IsConnected)
                {
                    SshCommand cmd = SshClientDevices[destination].RunCommand(data);
                    ResponseMsg receivedMsgs = new ResponseMsg(cmd.Result,StringEncoding.GetBytes(cmd.Result),destination, ProtocolType.SSHClient);
                    ResponseMsgs.Enqueue(receivedMsgs);
                }
            }
        }

        public void SetEncoding(Encoding textEncoding)
        {
            this.StringEncoding = textEncoding;
        }

        public IArchitecture GetArchitecture()
        {
            return EGArchitectureImplement.Interface;
            //throw new System.NotImplementedException();
        }
    }

    public static class CanGetEGSShClientExtension{
        public static EGSsh EGSsh(this IEGFramework self){
            return self.GetModule<EGSsh>();
        }

        public static SshClient EGGetSshClient(this IEGFramework self,string host){
            return self.GetModule<EGSsh>().SshClientDevices[host];
        }
    }

}