#if TOOLS
using Godot;
using System;

namespace EGFramework.Example.SystemExamples.PlayerStorage
{
	[Tool]
	public partial class ToolItemImporter : EditorPlugin
	{
		PackedScene MainPanel = ResourceLoader.Load<PackedScene>("res://addons/Tools/ItemImporter/ToolItemImporter.tscn");
    	Control MainPanelInstance;
		public override void _EnterTree()
		{
			// Initialization of the plugin goes here.
			MainPanelInstance = (Control)MainPanel.Instantiate();
			// Add the main panel to the editor's main viewport.
			EditorInterface.Singleton.GetEditorMainScreen().AddChild(MainPanelInstance);
			// Hide the main panel. Very much required.
			_MakeVisible(false);
		}

		public override void _ExitTree()
		{
			// Clean-up of the plugin goes here.
			if (MainPanelInstance != null)
			{
				MainPanelInstance.QueueFree();
			}
		}
		public override bool _HasMainScreen()
		{
			return true;
		}

		public override void _MakeVisible(bool visible)
		{
			if (MainPanelInstance != null)
			{
				MainPanelInstance.Visible = visible;
			}
		}

		public override string _GetPluginName()
		{
			return "Tool Item Importer";
		}

		public override Texture2D _GetPluginIcon()
		{
			return EditorInterface.Singleton.GetEditorTheme().GetIcon("Node", "EditorIcons");
		}
	}
}
#endif
