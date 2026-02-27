using Godot;
using System;

public partial class monk_little : CharacterBody2D
{
    [Export] public float Gravity = 900f;

    // --- 状态变量 ---
    public bool unlockState = false; // 与 main.cs 中的 CheckUnlockAllowence 对应
    public bool isDragging = false;
    public Vector2 draggingoffset;

    // --- 节点引用 ---
    private AnimatedSprite2D _sprite;
    private Button _unlockButton;
    private Area2D _dragArea;
    private Timer _gongdeTimer; // 建议在场景里添加一个 Timer 节点并命名为 GongdeTimer
    private Node _stateMachine;
    private CustomSignals _customSignal;

    // --- 颜色配置 ---
    private readonly Color LockedColor = new Color(0.1f, 0.1f, 0.1f, 1.0f);
    private readonly Color UnlockedColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

    public override void _Ready()
    {
        // 节点初始化
        _customSignal = GetNode<CustomSignals>("/root/CustomSignals");
        _sprite = GetNode<AnimatedSprite2D>("Sprite");
        _unlockButton = GetNode<Button>("UnlockButton");
        _dragArea = GetNode<Area2D>("DragArea2D");
        _stateMachine = GetNode<Node>("StateMachine");
        
        // 获取或创建计时器
        if (HasNode("GongdeTimer"))
        {
            _gongdeTimer = GetNode<Timer>("GongdeTimer");
        }
        else
        {
            // 如果场景里没画 Timer，我们代码生成一个
            _gongdeTimer = new Timer();
            _gongdeTimer.Name = "GongdeTimer";
            _gongdeTimer.WaitTime = 1.0f; // 每秒产生一次功德
            AddChild(_gongdeTimer);
        }

        // 绑定事件
        _unlockButton.Pressed += Unlock;
        _dragArea.InputEvent += DragInteraction;
        _gongdeTimer.Timeout += OnGongdeTimerTimeout;

        // 初始状态：变黑且隐藏按钮
        _sprite.Modulate = LockedColor;
        _unlockButton.Scale = Vector2.Zero;
    }

    // 由 main.cs 调用：显示解锁按钮
    public void ShowUnlockButton()
    {
        // 只有未解锁且按钮还没弹出来时才执行 Tween
        if (unlockState || _unlockButton.Scale.X > 0.5f) return;

        GetTree().CreateTween().TweenProperty(_unlockButton, "scale", Vector2.One, 0.2f)
            .SetTrans(Tween.TransitionType.Back)
            .SetEase(Tween.EaseType.Out);
    }

    // 点击按钮触发解锁
    public void Unlock()
    {
        if (unlockState) return;

        unlockState = true;
        
        // 表现效果
        TurnOnPrayAction();
        _unlockButton.Disabled = true;
        _unlockButton.Hide();

        // 逻辑启动
        if (_stateMachine.HasMethod("ready_start"))
        {
            _stateMachine.Call("ready_start");
        }
        
        _gongdeTimer.Start();

        // 通知主系统扣钱
        _customSignal.EmitSignal(nameof(_customSignal.MonkLittleUnlock));
    }

    private void OnGongdeTimerTimeout()
    {
        // 自动调用主场景的功德增加方法
        var mainScene = GetTree().CurrentScene as main;
        if (mainScene != null)
        {
            mainScene.AddGongde(1); // 使用我们重写后的 AddGongde 方法
        }
    }

    private void TurnOnPrayAction()
    {
        _sprite.Modulate = UnlockedColor;
        _sprite.Play();
        // 确保动画循环
        if (_sprite.SpriteFrames != null)
        {
            _sprite.SpriteFrames.SetAnimationLoop(_sprite.Animation, true);
        }
    }

    // 拖拽交互逻辑
    private void DragInteraction(Node viewport, InputEvent @event, long shape_idx)
    {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == MouseButton.Left)
        {
            if (mouseEvent.Pressed)
            {
                // 1. 立即计算偏移并标记状态，防止逻辑延迟
                isDragging = true;
                draggingoffset = GetGlobalMousePosition() - GlobalPosition;
                Gravity = 0f;
                Velocity = Vector2.Zero;

                // 2. 通知主场景注册当前拖拽对象
                _customSignal.EmitSignal(nameof(_customSignal.MonkRegisteredToDND), this);
            }
            else if (isDragging)
            {
                isDragging = false;
                Gravity = 900f;
                
                // 3. 通知主场景尝试放置
                _customSignal.EmitSignal(nameof(_customSignal.ReleaseMonkRegisteredToDND));
            }
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (isDragging)
        {
            // 拖拽时直接更新位置
            GlobalPosition = GetGlobalMousePosition() - draggingoffset;
            return;
        }

        // 正常物理：重力与移动
        Vector2 vel = Velocity;
        if (!IsOnFloor())
        {
            vel.Y += Gravity * (float)delta;
        }
        else
        {
            vel.Y = 0;
        }

        Velocity = vel;
        MoveAndSlide();
    }
}