using Godot;
using System;

public partial class temple_3 : Node2D
{
    public TextureButton downgradeButton;
    public TextureButton upgradeButton;
    public TextureButton workButton;
    public TextureButton unselectButton;

    public Node2D buttons;
    public Area2D templeArea;
    public AnimatedSprite2D Sprite;

    public int monk_num_within = 0;

    [Signal]
    public delegate void ChangeBuildingEventHandler(int building_code);

    public override void _Ready()
    {
        Sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        buttons = GetNode<Node2D>("Buttons");
        
        downgradeButton = GetNode<TextureButton>("Buttons/HBoxContainer/DowngradeButton");
        upgradeButton = GetNode<TextureButton>("Buttons/HBoxContainer/UpgradeButton");
        workButton = GetNode<TextureButton>("Buttons/HBoxContainer/WorkButton");
        unselectButton = GetNode<TextureButton>("Buttons/HBoxContainer/UnselectButton");

        templeArea = GetNode<Area2D>("Area2D");
        templeArea.InputEvent += ClickBuilding;

        unselectButton.Pressed += ClickUnselect;
        workButton.Pressed += ClickWork;
        upgradeButton.Pressed += ClickUpgrade;
        downgradeButton.Pressed += ClickDowngrade;

        Initialize();
    }

    public void Initialize()
    {
        buttons.Scale = Vector2.Zero;
    }

    public void ClickWork()
    {
        if (monk_num_within <= 0) return;

        // 1. 加载并实例化小和尚，直接转换为 monk_little 类型
        PackedScene monkScene = GD.Load<PackedScene>("res://Char/monk_little.tscn");
        monk_little monk = monkScene.Instantiate<monk_little>();

        // 2. 获取主场景并添加子节点
        main world_system = GetTree().CurrentScene as main;
        if (world_system != null)
        {
            // 确保加入到主场景专门管理和尚的 MonkSystem 节点下
            if (world_system.MonkSystem != null)
                world_system.MonkSystem.AddChild(monk);
            else
                world_system.AddChild(monk);
        }

        // 3. 设置位置 (GlobalPosition 比较稳妥)
        monk.GlobalPosition = GlobalPosition + new Vector2(50, 50);

        // 4. 【关键修复】直接调用方法，不要用 Call
        // 只要调用 Unlock，小和尚内部的 Timer 就会启动（基于我们之前改过的 monk_little 逻辑）
        monk.Unlock();

        // 5. 扣除寺庙内的数量并更新动画
        ReleaseAMonk();
        FoldButtonList();
    }

    public void GetAMonk(monk_little Monk)
    {
        if (monk_num_within >= 3) return;

        monk_num_within += 1;
        UpdateAnimation();

        if (IsInstanceValid(Monk))
            Monk.QueueFree();
    }

    public void ReleaseAMonk()
    {
        if (monk_num_within <= 0) return;
        monk_num_within -= 1;
        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        if (monk_num_within == 0) Sprite.Play("Low");
        else if (monk_num_within == 1) Sprite.Play("Median");
        else Sprite.Play("High");
    }

    // --- 界面交互 ---
    public void ClickBuilding(Node viewport, InputEvent @event, long shape_idx)
    {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
            UnfoldButtonList();
    }

    public void ClickUnselect() => FoldButtonList();
    public void ClickUpgrade() => FoldButtonList();
    public void ClickDowngrade()
    {
        EmitSignal(nameof(ChangeBuilding), 0);
        FoldButtonList();
    }

    public void UnfoldButtonList()
    {
        GetTree().CreateTween().TweenProperty(buttons, "scale", Vector2.One, 0.2);
    }

    public void FoldButtonList()
    {
        GetTree().CreateTween().TweenProperty(buttons, "scale", Vector2.Zero, 0.2);
    }
}