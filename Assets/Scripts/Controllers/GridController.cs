using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 셀 안에 있는 오브젝트를 관리
/// </summary>
class Cell
{ 
    // 오브젝트를 종류별 구분 관리해도 좋을 듯
    public HashSet<GameObject> Objects { get; } = new HashSet<GameObject>();
}

/// <summary>
/// 오브젝트를 그리드 영역을 통해 관리
/// </summary>
public class GridController : BaseController
{
    Grid _grid;
    Dictionary<Vector3Int, Cell> _cells = new Dictionary<Vector3Int, Cell>();

    public override bool Init()
    {
        base.Init();

        _grid = gameObject.GetOrAddComponent<Grid>();
        return true;
    }

    public void Add(GameObject go)
    { 
        var cellPos = _grid.WorldToCell(go.transform.position);
        var cell = GetCell(cellPos);
        if (cell == null)
            return;

        cell.Objects.Add(go);
    }

    public void Remove(GameObject go)
    {
        var cellPos = _grid.WorldToCell(go.transform.position);
        var cell = GetCell(cellPos);
        if (cell == null)
            return;

        cell.Objects.Remove(go);
    }

    Cell GetCell(Vector3Int cellPos)
    { 
        Cell cell = null;

        // 처음 방문하는 셀인 경우
        if (!_cells.TryGetValue(cellPos, out cell))
        {
            cell = new Cell();
            _cells.Add(cellPos, cell);
        }

        return cell;
    }

    public List<GameObject> GatherObjects(Vector3 pos, float radius)
    { 
        List<GameObject> objects = new List<GameObject>();

        Vector3Int left = _grid.WorldToCell(pos + new Vector3(-radius, 0.0f));
        Vector3Int right = _grid.WorldToCell(pos + new Vector3(radius, 0.0f));
        Vector3Int top = _grid.WorldToCell(pos + new Vector3(0.0f, radius));
        Vector3Int bottom = _grid.WorldToCell(pos + new Vector3(0.0f, -radius));

        int minX = left.x; 
        int minY = bottom.y;

        int maxX = right.x; 
        int maxY = top.y;

        for (int x = minX; x <= maxX; ++x)
        {
            for (int y = minY; y <= maxY; ++y)
            {
                // 등록되지 않은 구역. 즉, 오브젝트가 존재하지 않은 구역
                if (!_cells.ContainsKey(new Vector3Int(x, y, 0)))
                    continue;

                objects.AddRange(_cells[new Vector3Int(x, y, 0)].Objects);
            }
        }

        return objects;
    }
}
