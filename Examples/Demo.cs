using Godot;
using System;

public partial class Demo : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MainMenu.inst.Show();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
