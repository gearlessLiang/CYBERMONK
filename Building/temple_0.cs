using Godot;
using System;
using System.ComponentModel;

public partial class temple_0 : Node2D
{

	
	// public TextureButton upgradeButton;
	// public TextureButton workButton;
	// public TextureButton unselectButton;
	public TextureButton temple1Button;
	public TextureButton temple2Button;
	public TextureButton temple3Button;
	public Node2D buttons;

	public Area2D templeArea;

	[Signal]
	public delegate void ChangeBuildingEventHandler(int building_code);




	//单独制作一个initial方便重新调用ready
	public override void _Ready()
    {
		
		buttons = GetNode<Node2D>("Buttons");
		// downgradeButton = GetNode<TextureButton>("Buttons/DowngradeButton");
		// upgradeButton = GetNode<TextureButton>("Buttons/UpgradeButton");
		// workButton = GetNode<TextureButton>("Buttons/WorkButton");
		// unselectButton = GetNode<TextureButton>("Buttons/UnselectButton");
		temple1Button = GetNode<TextureButton>("Buttons/Temple1Button");
		temple2Button = GetNode<TextureButton>("Buttons/Temple2Button");
		temple3Button = GetNode<TextureButton>("Buttons/Temple3Button");

		templeArea = GetNode<Area2D>("Area2D");
		templeArea.InputEvent += ClickBuilding;

		temple1Button.Pressed += ClickTemple1;
		temple2Button.Pressed += ClickTemple2;
		temple3Button.Pressed += ClickTemple3;

		Initialize();
    }

	// Called when the node enters the scene tree for the first time.
	public void Initialize()
	{


		buttons.Scale = Vector2.Zero;
	}



	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void ClickBuilding(Node vivewport, InputEvent @event, long shape_idx)
	{
		if (@event is InputEventMouseButton mouseEvent)
		{
			if (mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
			{
				UnfoldButtonList();
			}
		}

	}


	public void ClickTemple1()
    {
		EmitSignal(nameof(ChangeBuilding), 1);
    }

	public void ClickTemple2()
    {
		EmitSignal(nameof(ChangeBuilding), 2);
    }

	public void ClickTemple3()
    {
		EmitSignal(nameof(ChangeBuilding), 3);
    }



	public void UnfoldButtonList()
	{
		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(buttons, "scale", Vector2.One * 0.25f, 0.2);
	}
	

	public void FoldButtonList()
    {
		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(buttons, "scale", Vector2.Zero, 0.2);
    }
}
