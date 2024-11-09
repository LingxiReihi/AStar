using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAStar : MonoBehaviour
{
    private BaseGrid _baseGrid = new();

    public Transform mapStart;
    public Transform mapEnd;
    public Transform start;
    public Transform end;
    public LayerMask layers;
    public List<AStarNode> path;

    public void OnStart()
    {
        _baseGrid.Init(mapStart.position, mapEnd.position, layers, 0.5f);
        StartCoroutine(GetPath());
    }

    private IEnumerator GetPath()
    {
        List<AStarNode> path = AStarManager.Instance.FindPath(start.position, end.position, _baseGrid);
        yield return null;
        this.path = path;
        yield return null;
        foreach (var node in path)
        {
            Debug.Log(node.worldPos);
            yield return new WaitForSeconds(1);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(_baseGrid.startPos + new Vector3(_baseGrid.gridSize.x, 0, _baseGrid.gridSize.y) / 2, new Vector3(_baseGrid.gridSize.x, 1, _baseGrid.gridSize.y));
        if (_baseGrid.grids == null)
        {
            return;
        }
        foreach (var node in _baseGrid.grids)
        {
            Gizmos.color = node.type == E_Node_Type.Obstacle ? Color.red : Color.white;
            Gizmos.DrawCube(node.worldPos, Vector3.one * (_baseGrid.Extent - 0.1f));
        }


        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(_baseGrid.GetByWorldPositionNode(start.position).worldPos, Vector3.one * (_baseGrid.Extent - 0.1f));
        Gizmos.DrawCube(_baseGrid.GetByWorldPositionNode(end.position).worldPos, Vector3.one * (_baseGrid.Extent - 0.1f));

        if (path != null)
        {
            Gizmos.color = Color.green;
            foreach (var node in path)
            {
                Gizmos.DrawCube(node.worldPos, Vector3.one * (_baseGrid.Extent - 0.1f));
            }
        }
    }
}