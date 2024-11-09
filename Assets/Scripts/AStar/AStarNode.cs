using UnityEngine;

/// <summary>
/// 地图结点信息(格子类)
/// </summary>
public class AStarNode
{
    /// <summary>
    /// 节点在数组中的坐标x
    /// </summary>
    public int gridX;
    /// <summary>
    /// 节点在数组中的坐标y
    /// </summary>
    public int gridY;
    /// <summary>
    /// 节点在世界坐标中的坐标
    /// </summary>
    public Vector3 worldPos;
    /// <summary>
    /// 节点到终点的预估消耗
    /// </summary>
    public float h;
    /// <summary>
    /// 起点到当前节点的消耗
    /// </summary>
    public float g;
    /// <summary>
    /// 该路径预估总消耗
    /// </summary>
    public float F { get { return g + h; } }
    /// <summary>
    /// 节点的父节点
    /// </summary>
    public AStarNode parent;
    /// <summary>
    /// 该节点的类型
    /// </summary>
    public E_Node_Type type;

    /// <summary>
    /// 初始化节点函数
    /// </summary>
    /// <param name="gridX">节点在数组中的坐标x</param>
    /// <param name="gridY">节点在数组中的坐标y</param>
    /// <param name="worldPos">节点的世界坐标</param>
    /// <param name="type">节点的类型</param>
    public AStarNode(int gridX, int gridY, Vector3 worldPos, E_Node_Type type)
    {
        this.gridX = gridX;
        this.gridY = gridY;
        this.type = type;
        this.worldPos = worldPos;
    }
}
