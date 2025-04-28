using Godot;
using System;

namespace EGFramework.Examples.ModbusDebugTool{
    public partial class ViewMessage : Control
    {
        public TextEdit MessageContainer { set; get; }
        public override void _Ready()
        {
            this.Visible = false;
            MessageContainer = this.GetNode<TextEdit>("MessageContainer");
        }
        public void OnClose(){
            this.Visible = false;
        }
        public void Clear(){
            MessageContainer.Text = "";
        }
        public void AppendMessage(string msg){
            MessageContainer.Text += msg + "\n";
        }
    }

    public static class ViewMessageExtension{
        public static ViewMessage ViewMessage(this Node self){
            return self.GetTree().CurrentScene.GetNode<ViewMessage>("Message");
        }
        public static void AppendMessage(this Node self,string msg){
            self.GetTree().CurrentScene.GetNode<ViewMessage>("Message").AppendMessage(msg);
        } 
    }
}

