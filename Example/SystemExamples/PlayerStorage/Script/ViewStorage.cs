using Godot;
using System;
using EGFramework;

public partial class ViewStorage : Node,IEGFramework
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// this.GetModule<EGByteSave>().SaveToFile("nihao");
		Variant result = this.GetModule<EGByteSave>().LoadFromFile();
		GD.Print(result);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
