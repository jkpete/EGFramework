using Godot;
using System;

namespace EGFramework.Examples.Action3D{
	public partial class ActionPlayer : CharacterBody3D
	{
		[Export]
    	public int Speed { get; set; } = 14;
		[Export]
    	public int FallAcceleration { get; set; } = 75;

    	private Vector3 _targetVelocity = Vector3.Zero;

		public Node3D CameraPivot { set; get; }
		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			CameraPivot = this.GetNode<Node3D>("../Pivot");
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
		}

        public override void _PhysicsProcess(double delta)
        {
			// We create a local variable to store the input direction.
			var direction = Vector3.Zero;

			direction.X = Input.GetAxis("move_left", "move_right");
			direction.Z = Input.GetAxis("move_forward", "move_back");
			

			if (direction.Length() > 0)
			{
				this.Rotation = new Vector3(0,CameraPivot.Rotation.Y,0);
				direction = direction.Normalized();
				direction = direction.Rotated(Vector3.Up, CameraPivot.Rotation.Y);
			}
			// direction.Rotated(Vector3.Up, CameraPivot.Rotation.Y);
			// Ground velocity
			_targetVelocity.X = direction.X * Speed;
			_targetVelocity.Z = direction.Z * Speed;

			if (!IsOnFloor()) // If in the air, fall towards the floor. Literally gravity
			{
				// GD.Print("Fallen!");
				_targetVelocity.Y -= FallAcceleration * (float)delta;
			}else{
				if (Input.IsActionPressed("jump"))
				{
					_targetVelocity.Y += 10.0f;
				}
			}

			this.Velocity = _targetVelocity;
			MoveAndSlide();
			CameraPivot.Position = this.Position;
        }
    }
}
