using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A*寻路管理器
/// </summary>
public class AStarManager
{
    private static AStarManager _instance;
    public static AStarManager Instance
    {
        get
        {
            _instance ??= new AStarManager();
            return _instance;
        }
    }
    // 基础地图格子
    private BaseGrid _grid;
    // 开启列表和关闭列表
    private List<AStarNode> _openList = new();
    private HashSet<AStarNode> _closeList = new();

    /// <summary>
    /// 寻路，通过起点、终点、地图信息，返回寻得的路径
    /// </summary>
    /// <param name="startPos">起点坐标</param>
    /// <param name="endPos">终点坐标</param>
    /// <param name="grid">地图信息</param>
    /// <returns>最短路径，如果没有找到可行道路返回 null</returns>
    public List<AStarNode> FindPath(Vector3 startPos, Vector3 endPos, BaseGrid grid)
    {
        // 获取格子类
        _grid = grid;

        // 获取起点和终点节点
        AStarNode starNode = _grid.GetByWorldPositionNode(startPos);
        AStarNode endNode = _grid.GetByWorldPositionNode(endPos);

        // 判断起点和终点节点是否非法
        if (starNode == null || endNode == null)
        {
            Debug.Log("起点或终点不在地图范围内，无法寻路");
            return null;
        }

        // 清理开启列表和关闭列表
        _openList.Clear();
        _closeList.Clear();

        // 重置起点属性并将起点加入关闭列表
        starNode.g = 0;
        starNode.h = 0;
        starNode.parent = null;
        _openList.Add(starNode);

        // 重置终点的父节点
        endNode.g = 0;
        endNode.h = 0;
        endNode.parent = null;

        while (_openList.Count > 0)
        {
            // 获取最小节点
            AStarNode node = _openList[0];

            // 如果已经在终点则结束循环
            if (node.worldPos == endNode.worldPos)
                return GeneratePath(endNode);

            // 将当前点移出openList并加入closedList
            _closeList.Add(node);
            _openList.Remove(node);

            // 将该点周围的点加入openList
            AddNeighborsToOpenlist(node, endNode);
            // 对openList进行排序
            _openList.Sort(CompareOpenList);
        }

        // 当开启列表为空还没有到达终点则证明无法到达，死路，返回空路径
        return null;
    }

    /// <summary>
    /// 添加周围节点到openList，并且会更新他们的g、h值
    /// </summary>
    /// <param name="centerNode">当前节点</param>
    /// <param name="endNode">终点</param>
    protected void AddNeighborsToOpenlist(AStarNode centerNode, AStarNode endNode)
    {
        // 在传入地图中获取所有周围节点信息并依次进行操作
        foreach (var node in _grid.GetNeighbors(centerNode))
        {
            // 判断是否为障碍或该节点是否在closeList中，如果满足条件则不进行操作
            if (node.type == E_Node_Type.Obstacle || _closeList.Contains(node))
                continue;

            // 计算当前节点的新的g值
            float tempG = centerNode.g + GetDistance(centerNode, node);
            // 如果新的g值小于该节点原本的g值，或者该节点不在openList中，则进行操作
            if (tempG < node.g || !_openList.Contains(node))
            {
                //更新节点的g值和父节点
                node.g = tempG;
                node.parent = centerNode;
                // 如果该节点不在openList中，则加入openList，并更新节点的h值
                if (!_openList.Contains(node))
                {
                    node.h = Mathf.Abs(node.gridX - endNode.gridX) + Mathf.Abs(node.gridY - endNode.gridY);
                    _openList.Add(node);
                }
            }
        }
    }

    /// <summary>
    /// 获取相邻节点的实际距离（可以自行重写修改计算方式）
    /// </summary>
    /// <param name="nodeA"></param>
    /// <param name="nodeB"></param>
    /// <returns></returns>
    protected virtual float GetDistance(AStarNode nodeA, AStarNode nodeB)
    {
        // 计算两点的坐标轴差值绝对值
        int tempX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int tempY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
        // 绝对值相等则证明在对角线位置，返回1.4f
        if (tempX == tempY)
        {
            return 1.4f;
        }
        else
        {
            return 1;
        }
    }

    /// <summary>
    /// 比较两个节点相对终点的位置
    /// </summary>
    /// <param name="nodeA"></param>
    /// <param name="nodeB"></param>
    /// <returns></returns>
    protected virtual int CompareOpenList(AStarNode nodeA, AStarNode nodeB)
    {
        if (nodeA.F > nodeB.F)
        {
            return 1;
        }
        else if (nodeA.F == nodeB.F && nodeA.h > nodeB.h)
        {
            return 1;
        }
        else
        {
            return -1;
        }
    }

    /// <summary>
    /// 根据寻路终点，从终点往回找路径
    /// </summary>
    /// <param name="endNode">终点</param>
    /// <returns>路径列表</returns>
    protected virtual List<AStarNode> GeneratePath(AStarNode endNode)
    {
        if (endNode.parent != null)
        {
            // 获取路径
            var path = new List<AStarNode> { endNode };
            while (endNode.parent != null)
            {
                path.Add(endNode.parent);
                endNode = endNode.parent;
            }
            // 对顺序进行翻转，得到正确顺序路径
            path.Reverse();
            return path;
        }
        return null;
    }
}