using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BaseGrid
{
    /// <summary>
    /// 存放所有节点信息的数组
    /// </summary>
    public AStarNode[,] grids;
    /// <summary>
    /// 格子地图左下角坐标
    /// </summary>
    public Vector3 startPos;
    /// <summary>
    /// 格子的大小
    /// </summary>
    public Vector2 gridSize;
    /// <summary>
    /// 节点检测半径
    /// </summary>
    public float halfExtent;
    /// <summary>
    /// 节点检测层级
    /// </summary>
    public LayerMask layer;
    public float Extent => halfExtent * 2;
    /// <summary>
    /// 节点横坐标数量
    /// </summary>
    protected int _gridCountX;
    /// <summary>
    /// 节点纵坐标数量
    /// </summary>
    protected int _gridCountY;

    /// <summary>
    /// 初始化格子
    /// </summary>
    /// <param name="startPos">地图起点（对角线）</param>
    /// <param name="endPos">地图终点（对角线）</param>
    public virtual void Init(Vector3 startPos, Vector3 endPos, LayerMask layer, float halfExtent = 0.5f)
    {
        gridSize = new Vector2(Mathf.Abs(endPos.x - startPos.x), Mathf.Abs(endPos.z - startPos.z));
        this.halfExtent = halfExtent;

        _gridCountX = Mathf.RoundToInt(gridSize.x / Extent);
        _gridCountY = Mathf.RoundToInt(gridSize.y / Extent);
        grids = new AStarNode[_gridCountX, _gridCountY];
        this.layer = layer;

        // 获取两点的最小分量作为地图左上角
        this.startPos = new Vector3(
            Mathf.Min(startPos.x, endPos.x),
            Mathf.Min(startPos.y, endPos.y),
            Mathf.Min(startPos.z, endPos.z)
        );

        CreateGrid();
    }

    /// <summary>
    /// 创建格子数组并进行设置
    /// </summary>
    protected virtual void CreateGrid()
    {
        for (int i = 0; i < _gridCountX; i++)
        {
            for (int j = 0; j < _gridCountY; j++)
            {
                Vector3 worldPos = startPos;
                // 计算该点在世界坐标中的位置
                worldPos.x += i * Extent + halfExtent;
                worldPos.z += j * Extent + halfExtent;
                // 默认为可以行走的路径
                E_Node_Type type = E_Node_Type.Walkable;
                // 如果检测到不可以行走则设置对应的类型
                if (Physics.CheckSphere(worldPos, halfExtent, layer))
                {
                    type = E_Node_Type.Obstacle;
                }
                // 将节点记录到数组中
                grids[i, j] = new AStarNode(i, j, worldPos, type);
            }
        }
    }

    /// <summary>
    /// 通过坐标获取对应格子
    /// </summary>
    /// <param name="pos">格子坐标</param>
    /// <returns>格子对象</returns>
    public virtual AStarNode GetByWorldPositionNode(Vector3 pos)
    {
        int posInGridX = (int)((pos.x - startPos.x) / Extent);
        int posInGridY = (int)((pos.z - startPos.z) / Extent);
        if (posInGridX >= 0 && posInGridX < _gridCountX && posInGridY >= 0 && posInGridY < _gridCountY)
        {
            if (grids[posInGridX, posInGridY] != null || grids[posInGridX, posInGridY].type == E_Node_Type.Obstacle)
            {
                return grids[posInGridX, posInGridY];
            }
        }
        return null;
    }

    /// <summary>
    /// 获取节点周围的相邻节点
    /// </summary>
    /// <param name="node">中心节点</param>
    /// <returns>相邻节点列表</returns>
    public virtual List<AStarNode> GetNeighbors(AStarNode node)
    {
        Stack<AStarNode> neighbors = new();
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                {
                    continue;
                }

                int x = node.gridX + i;
                int y = node.gridY + j;

                if (x >= 0 && y >= 0 && x < _gridCountX && y < _gridCountY)
                {
                    neighbors.Push(grids[x, y]);
                }
            }
        }
        return neighbors.ToList<AStarNode>();
    }
}