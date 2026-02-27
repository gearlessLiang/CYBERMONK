using Godot;
using System;
using Godot.Collections;

public partial class building_place : Node2D
{
    // 使用数组或字典管理建筑，比单独定义变量更易于扩展
    private Node2D[] _temples = new Node2D[4];
    private Node2D _buildingsContainer;
    
    public int active_building_num = 0;

    public override void _Ready()
    {
        // 1. 初始化引用
        _buildingsContainer = GetNode<Node2D>("Building");
        
        // 自动获取所有子建筑并绑定信号
        for (int i = 0; i < 4; i++)
        {
            string path = $"Building/Temple{i}";
            if (HasNode(path))
            {
                _temples[i] = GetNode<Node2D>(path);
                
                // 绑定建筑切换信号
                // 注意：这里假设你的 temple 脚本都定义了 ChangeBuilding 信号
                _temples[i].Connect("ChangeBuilding", Callable.From<int>(change_building));
            }
        }

        // 2. 初始状态：只显示 Temple0
        change_building(0);
    }

    public void change_building(int building_code)
    {
        active_building_num = building_code;

        // 遍历所有建筑，匹配 code 的显示，其他的隐藏
        for (int i = 0; i < _temples.Length; i++)
        {
            if (_temples[i] == null) continue;

            if (i == building_code)
            {
                _temples[i].Show();
                // 如果建筑有 Initialize 方法，则调用它
                if (_temples[i].HasMethod("Initialize"))
                {
                    _temples[i].Call("Initialize");
                }
            }
            else
            {
                _temples[i].Hide();
            }
        }
        
        GD.Print($"建筑已切换至: Temple{building_code}");
    }

    /// <summary>
    /// 当小和尚被拖拽到此建筑位置并松开时，由 main.cs 调用
    /// </summary>
    public void monk_dragged_in(monk_little monk)
    {
        if (!IsInstanceValid(monk)) return;

        // 目前逻辑：只有 Temple3 (三级寺庙) 可以收纳和尚
        if (active_building_num == 3)
        {
            var temple3 = _temples[3] as temple_3;
            if (temple3 != null)
            {
                // 将和尚存入 temple_3 的逻辑
                temple3.GetAMonk(monk);
                GD.Print("小和尚已存入 Temple3");
            }
        }
        else
        {
            // 如果是其他等级的建筑，可以设置不接收，或者弹回原位
            GD.Print("当前等级建筑无法收纳和尚");
        }
    }
}