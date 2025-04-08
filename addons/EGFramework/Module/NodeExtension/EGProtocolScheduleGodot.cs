using Godot;

namespace EGFramework{
    /// <summary>
    /// In Godot engine, the async method return cannot operate the godot main thread such as Screen Trees Node.
    /// The protocol schedule provide a main thread to check the message in protocol tools 's response message.
    /// If you have more idea ,please issue to me, thanks.
    /// </summary>
    public partial class EGProtocolScheduleGodot : Node, IModule, IEGFramework
    {
        public EGProtocolSchedule ProtocolSchedule { set; get; } = new EGProtocolSchedule();
        public override void _Process(double delta)
        {
            ProtocolSchedule.CheckedProcess();
        }
        public void Init()
        {

        }
        public IArchitecture GetArchitecture()
        {
            return EGArchitectureImplement.Interface;
        }
    }

    public static class CanGetEGProtocolInGodotExtension{
        public static EGProtocolSchedule EGProtocolSchedule(this Node self){
            return self.NodeModule<EGProtocolScheduleGodot>().ProtocolSchedule;
        }
        public static void EGEnabledProtocolTools(this Node self){
            self.NodeModule<EGProtocolScheduleGodot>().ProtocolSchedule.EnabledAllTools();
        }
        public static void EGEnabledProtocolTool<TProtocolReceived>(this Node self) where TProtocolReceived : class, IModule,IProtocolReceived,new(){
            self.NodeModule<EGProtocolScheduleGodot>().ProtocolSchedule.EnabledTool<TProtocolReceived>();
        }
    }
}