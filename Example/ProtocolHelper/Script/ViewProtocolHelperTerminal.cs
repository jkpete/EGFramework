using EGFramework;
using Godot;
using System;
using System.Collections.Generic;

namespace EGFramework.ProtocolHelper
{
    public partial class ViewProtocolHelperTerminal : RichTextLabel
    {
        public Dictionary<long, ResponseMsg> MessageMappings { set; get; } = new Dictionary<long, ResponseMsg>();
        public override void _Ready()
        {

        }

        public void ClearArea()
        {

        }
        public void RefreshProtocol()
        {

        }

    }
}