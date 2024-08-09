using Godot;
using System;

public partial class ActionCamera : Node3D
{
	[Export]
	public float Speed { get; set; } = 4;
	private Vector3 _targetRotation = Vector3.Zero;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsActionPressed("camera_up"))
		{
			_targetRotation.X -= Speed*(float)delta;
		}
		if (Input.IsActionPressed("camera_down"))
		{
			_targetRotation.X += Speed*(float)delta;
		}
		if (Input.IsActionPressed("camera_left"))
		{
			_targetRotation.Y -= Speed*(float)delta;
		}
		if (Input.IsActionPressed("camera_right"))
		{
			_targetRotation.Y += Speed*(float)delta;
		}
		if (_targetRotation.X>30*Mathf.Pi/180)
		{
			_targetRotation.X = 30*Mathf.Pi/180;
		}
		if (_targetRotation.X<-30*Mathf.Pi/180){
			_targetRotation.X = -30*Mathf.Pi/180;
		}
		this.Rotation = this.Rotation.Lerp(_targetRotation,Speed*(float)delta) ;
    }
}
