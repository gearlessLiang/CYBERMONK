using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class main : Node2D
{
    // --- 节点引用 ---
	private CustomSignals _customSignal;
    private Label _gongdeLabel;
    
    public Node2D BuildingSystem; // 必须是 public
    public Node2D MonkSystem;     // 必须是 public

    // --- 数据变量 ---
    public int gongdeBalance = 0;
    public monk_little registered_monk;

    [Export(PropertyHint.File, "*.csv")]
    public string appointed_csv_path = "";

    public override void _Ready()
    {
        // 这里的路径要和你场景树的名字完全一致
        BuildingSystem = GetNode<Node2D>("BuildingSystem");
        MonkSystem = GetNode<Node2D>("MonkSystem");
        _gongdeLabel = GetNode<Label>("CanvasLayer/Label"); 
        
        // ... 其余逻辑不变 ...
        _customSignal = GetNode<CustomSignals>("/root/CustomSignals");
        _customSignal.ClickMuyu += (msg) => AddGongde(1);
        _customSignal.MonkLittleUnlock += OnMonkUnlocked;
        _customSignal.MonkRegisteredToDND += (monk) => registered_monk = monk;
        _customSignal.ReleaseMonkRegisteredToDND += OnReleaseMonk;

        LoadAndReadCSV(appointed_csv_path);
        UpdateUI();
    }

    // 功德增加核心逻辑
    public void AddGongde(int amount)
    {
        gongdeBalance += amount;
        UpdateUI();
        
        // 每次功德变动，检查所有和尚是否达到解锁条件
        CheckAllMonksUnlockAllowance();
    }

    private void UpdateUI()
    {
        if (IsInstanceValid(_gongdeLabel))
        {
            _gongdeLabel.Text = gongdeBalance.ToString();
        }
    }

	private void CheckAllMonksUnlockAllowance()
    {
        if (!IsInstanceValid(MonkSystem)) return;

        foreach (Node child in MonkSystem.GetChildren())
        {
            if (child is monk_little monk && IsInstanceValid(monk))
            {
                if (!monk.unlockState && gongdeBalance >= 10)
                {
                    monk.ShowUnlockButton();
                }
            }
        }
    }

    private void OnMonkUnlocked()
    {
        gongdeBalance -= 10;
        UpdateUI();
        GD.Print("主系统确认：功德扣除，解锁完成。");
    }

    // --- CSV 读取 (优化版) ---
    public void LoadAndReadCSV(string path)
    {
        if (string.IsNullOrEmpty(path) || !FileAccess.FileExists(path))
        {
            GD.PrintErr("CSV 路径无效！");
            return;
        }

        using var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
        int lineCount = 0;
        while (!file.EofReached())
        {
            string line = file.GetLine();
            if (lineCount == 2) // 获取第三行 (Index 2)
            {
                string[] cols = line.Split(',');
                if (cols.Length > 1 && int.TryParse(cols[1].Trim(), out int val))
                {
                    gongdeBalance = val;
                }
            }
            lineCount++;
        }
    }

    // --- 拖拽放置逻辑 (重构版) ---
    private void OnReleaseMonk()
    {
        if (!IsInstanceValid(registered_monk)) return;

        Vector2 dropPos = registered_monk.GlobalPosition;
        Node2D targetBuilding = FindClosestBuilding(dropPos, 80f);

        if (targetBuilding != null)
        {
            if (targetBuilding.HasMethod("monk_dragged_in"))
            {
                targetBuilding.Call("monk_dragged_in", registered_monk);
                // 这里不需要在这里 Hide，由建筑脚本处理
            }
            targetBuilding.Scale *= 1.05f;
        }

        registered_monk = null;
    }

    private Node2D FindClosestBuilding(Vector2 pos, float maxDist)
    {
        Node2D closest = null;
        float minDist = maxDist;

        foreach (Node child in BuildingSystem.GetChildren())
        {
            if (child is Node2D b)
            {
                float d = pos.DistanceTo(b.GlobalPosition);
                if (d < minDist)
                {
                    minDist = d;
                    closest = b;
                }
            }
        }
        return closest;
    }
}