using System.Collections.Generic;
using UnityEngine;

public class AStarPathFinder
{
    public class Node 
    {
        public Vector3Int pos;
        public int gCost;
        public int hCost;
        public int fCost => gCost + hCost;
        public Node parent;

        public Node(Vector3Int pos) { this.pos = pos; }
    }

    public static List<Vector3Int> FindPath(Vector3Int start, Vector3Int goal, HashSet<Vector3Int> walkableTiles, int moveRange) 
    {
        var open = new List<Node>();
        var closed = new HashSet<Vector3Int>();

        Node startNode = new Node(start) { gCost = 0, hCost = Heuristic(start, goal) };
        open.Add(startNode);

        while (open.Count > 0) 
        {
            Node current = open[0];
            foreach (var node in open) 
            {
                if (node.fCost < current.fCost) current = node;
            }

            open.Remove(current);
            closed.Add(current.pos);

            if (current.pos == goal) 
            {
                if (current.gCost > moveRange) 
                {
                    //이동력 부족
                    return null;
                }
                return ReconstructPath(current);
            }

            foreach (var nodePos in GetNearNode(current.pos)) 
            {
                if (!walkableTiles.Contains(nodePos) || closed.Contains(nodePos)) continue;

                //이동 코스트 계산
                int heightCost = 0;
                if (nodePos.y > current.pos.y) { heightCost = nodePos.y - current.pos.y; }
                int tentativeG = current.gCost + 1 + heightCost;
                //이동력 초과시
                if (tentativeG > moveRange) continue;

                Node nearNode = open.Find(n => n.pos == nodePos);
                if (nearNode == null)
                {
                    nearNode = new Node(nodePos);
                    nearNode.gCost = tentativeG;
                    nearNode.hCost = Heuristic(nodePos, goal);
                    nearNode.parent = current;
                    open.Add(nearNode);
                }
                else if (tentativeG < nearNode.gCost) 
                {
                    nearNode.gCost = tentativeG;
                    nearNode.parent = current;
                }
            }
        }

        return null;
    }

    private static int Heuristic(Vector3Int a, Vector3Int b) 
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.z - b.z);
    }

    private static List<Vector3Int> ReconstructPath(Node node) 
    {
        List<Vector3Int> path = new List<Vector3Int>();
        while (node != null) 
        {
            path.Add(node.pos);
            node = node.parent;
        }
        path.Reverse();
        return path;
    }

    private static List<Vector3Int> GetNearNode(Vector3Int pos) 
    {
        List<Vector3Int> nearNodes = new List<Vector3Int>();

        Vector3Int[] dirs = {
            new Vector3Int(1, 0, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(0, 0, 1),
            new Vector3Int(0, 0, -1) };

        foreach (var dir in dirs) 
        {
            int nx = pos.x + dir.x;
            int nz = pos.z + dir.z; 

            TileBase tile = StageManager.Instance.GetTopTileAt(nx, nz);
            if (tile != null && tile.isWalkable) 
            {
                nearNodes.Add(tile.GetPosition());
            }
        } 
        return nearNodes;
    }

    //BFS
    public static HashSet<Vector3Int> GetReachableTiles(Vector3Int start, int moveRange, HashSet<Vector3Int> walkableTiles) 
    {
        var reachable = new HashSet<Vector3Int>();
        var frontier = new Queue<(Vector3Int pos, int cost)>();
        frontier.Enqueue((start, 0));

        while (frontier.Count > 0) 
        {
            var (pos, cost) = frontier.Dequeue();
            //행동력 부족하면
            if (cost > moveRange) continue;
            //지나갈 수 없는 경우
            if (pos != start && !walkableTiles.Contains(pos)) continue;
            //이미 넣었으면
            if (reachable.Contains(pos)) continue;

            reachable.Add(pos);

            foreach (var near in GetNearNode(pos)) 
            {
                int heightCost = 0;
                if (near.y > pos.y) { heightCost = near.y - pos.y; }
                frontier.Enqueue((near, cost + 1 + heightCost));
            }
        }

        return reachable;
    }
}
