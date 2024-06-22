using Godot;
using System;
using System.Collections.Generic;

namespace EGFramework{
    /// <summary>
    /// In Godot engine, the async method return cannot operate the godot main thread such as Screen Trees Node.
    /// The protocol schedule provide a main thread to check the message in protocol tools 's response message.
    /// If you have more idea ,please issue to me, thanks.
    /// </summary>
    public partial class EGProtocolSchedule : Node, IModule,IEGFramework
    {
        public Dictionary<Type,IProtocolReceived> ProtocolTools = new Dictionary<Type, IProtocolReceived>();
        public void Init()
        {

        }

        public override void _Process(double delta)
        {
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
    public static class CanGetEGProtocolExtension{
        public static EGProtocolSchedule EGProtocolSchedule(this Node self){
            return self.NodeModule<EGProtocolSchedule>();
        }
        public static void EGEnabledProtocolTools(this Node self){
            self.NodeModule<EGProtocolSchedule>().EnabledAllTools();
        }
        public static void EGEnabledProtocolTool<TProtocolReceived>(this Node self) where TProtocolReceived : class, IModule,IProtocolReceived,new(){
            self.NodeModule<EGProtocolSchedule>().EnabledTool<TProtocolReceived>();
        }

    }
}

