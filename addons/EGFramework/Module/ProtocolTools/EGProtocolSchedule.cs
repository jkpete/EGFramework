using Godot;
using System;
using System.Collections.Generic;

namespace EGFramework{
    public class EGProtocolSchedule : IEGFramework
    {
        public Dictionary<Type,IProtocolReceived> ProtocolTools = new Dictionary<Type, IProtocolReceived>();

        public void CheckedProcess(){
            foreach(IProtocolReceived tool in ProtocolTools.Values){
                if(tool.GetReceivedMsg().Count>0){
                    this.GetModule<EGMessage>().OnDataReceived.Invoke(tool.GetReceivedMsg().Dequeue());
                }
            }
        }
        public void EnabledAllTools(){
            this.EnabledTool<EGTCPClient>();
            this.EnabledTool<EGTCPServer>();
            this.EnabledTool<EGUDP>();
            this.EnabledTool<EGSerialPort>();
            this.EnabledTool<EGFileStream>();

        }
        public void EnabledTool<TProtocolReceived>() where TProtocolReceived : class, IModule,IProtocolReceived,new(){
            if(!ProtocolTools.ContainsKey(typeof(TProtocolReceived))){
                ProtocolTools.Add(typeof(TProtocolReceived),this.GetModule<TProtocolReceived>());
            }
            
        }
        
        public void DisabledAllTools(){
            ProtocolTools.Clear();
        }

        public void DisabledTool<TProtocolReceived>() where TProtocolReceived : class, IModule,IProtocolReceived,new(){
            if(ProtocolTools.ContainsKey(typeof(TProtocolReceived))){
                ProtocolTools.Remove(typeof(TProtocolReceived));
            }
        }

        public IArchitecture GetArchitecture()
        {
            return EGArchitectureImplement.Interface;
        }

    }
}

