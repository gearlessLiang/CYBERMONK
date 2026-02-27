using Godot;
using System;

public partial class pilgrim : CharacterBody2D
{
	public Node stateMachine;
	public AnimatedSprite2D Sprite;
	public override void _Ready()
	{
		stateMachine = GetNode<Node>("StateMachine");
		stateMachine.Call("ready_start");
		
		Sprite = GetNode<AnimatedSprite2D>("Sprite");
		Sprite.Play("walk");
	}


}
