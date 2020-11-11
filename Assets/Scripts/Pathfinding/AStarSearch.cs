using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public static class AStarSearch {
    
    public delegate void OnAddToPath(PathNode node);
    public static event OnAddToPath HandleAddToPath;

    public delegate void OnAddToClosedSet(PathNode node);
    public static event OnAddToClosedSet HandleAddToClosedSet;
    
    public static List<PathNode> GetPath(PathNode startNode, PathNode targetNode) {
        if (startNode == targetNode) {
            return new List<PathNode>();
        }
        
        if (startNode.Region != targetNode.Region) {
            return null;
        }

        List<Subregion> subregions = AStarSubregionSearch.GetPath(startNode.subregion, targetNode.subregion);
        PathNode[] possibleNodes = new PathNode[Pathfinder.MapWidth * Pathfinder.MapHeight];
        foreach (Subregion s in subregions) {
            foreach (PathNode n in s.nodes) {
                possibleNodes[n.X + n.Y * Pathfinder.MapHeight] = n;
            }
        }
        
        Heap<PathNode> openSet = new Heap<PathNode>(Pathfinder.MapWidth * Pathfinder.MapHeight);
        HashSet<PathNode> closedSet = new HashSet<PathNode>();
        openSet.Add(startNode);

        while (openSet.Count > 0) {
            PathNode currentNode = openSet.RemoveFirst();
            closedSet.Add(currentNode);
            HandleAddToClosedSet?.Invoke(currentNode);
            
            if (currentNode == targetNode) {
                var path = RetracePath(startNode, targetNode);
                return path;
            }

            foreach(var neighbour in GetNeighbours(currentNode)) {
                if (closedSet.Contains(neighbour) || possibleNodes[neighbour.X + neighbour.Y * Pathfinder.MapHeight] == null) {
                    continue;
                }
                
                int newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (openSet.Contains(neighbour)) {
                    if (newCostToNeighbour < neighbour.gCost) {
                        neighbour.gCost = newCostToNeighbour;
                        neighbour.hCost = Heuristic(neighbour, targetNode);
                        neighbour.parent = currentNode;
                        openSet.UpdateItem(neighbour);
                    }
                } else {
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = Heuristic(neighbour, targetNode);
                    neighbour.parent = currentNode;
                    
                    openSet.Add(neighbour);
                }
            }
        }

        return null;
    }

    public static List<PathNode> GetPathWithoutRegionSearch(PathNode startNode, PathNode targetNode) {
        if (startNode == targetNode) {
            return new List<PathNode>();
        }
        
        if (startNode.Region != targetNode.Region) {
            return null;
        }

        Heap<PathNode> openSet = new Heap<PathNode>(Pathfinder.MapWidth * Pathfinder.MapHeight);
        HashSet<PathNode> closedSet = new HashSet<PathNode>();
        openSet.Add(startNode);

        while (openSet.Count > 0) {
            PathNode currentNode = openSet.RemoveFirst();
            closedSet.Add(currentNode);
            HandleAddToClosedSet?.Invoke(currentNode);
            
            if (currentNode == targetNode) {
                var path = RetracePath(startNode, targetNode);
                return path;
            }

            foreach(var neighbour in GetNeighbours(currentNode)) {
                if (closedSet.Contains(neighbour)) {
                    continue;
                }
                
                int newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (openSet.Contains(neighbour)) {
                    if (newCostToNeighbour < neighbour.gCost) {
                        neighbour.gCost = newCostToNeighbour;
                        neighbour.hCost = Heuristic(neighbour, targetNode);
                        neighbour.parent = currentNode;
                        openSet.UpdateItem(neighbour);
                    }
                } else {
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = Heuristic(neighbour, targetNode);
                    neighbour.parent = currentNode;
                    
                    openSet.Add(neighbour);
                }
            }
        }

        return null;
    }

    private static List<PathNode> GetNeighbours(PathNode node) {
		List<PathNode> neighbours = new List<PathNode>();
		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {

				if( x == 0 && y == 0 ) {
					continue;
                }

				int checkX = node.X + x;
				int checkY = node.Y + y;
                PathNode n = PathGrid.NodeAt(checkX, checkY);
                if (n != null && n.isTraversable) {
					neighbours.Add(n);
                }
			}
		}
		return neighbours;
	}

    private static int GetDistance(PathNode A, PathNode B) {
        int distX = Mathf.Abs(A.X - B.X);
        int distY = Mathf.Abs(A.Y - B.Y);
        if(distX > distY) {
            return 14 * distY + 10 * (distX - distY);
        }
        return 14 * distX + 10 * (distY - distX);
    }

    private static int Heuristic(PathNode A, PathNode B) {
        // Octile
        int dx = Mathf.Abs(A.X - B.X);
        int dy = Mathf.Abs(A.Y - B.Y);
        return 10 * (dx + dy) + (14 - 2 * 10) * Mathf.Min(dx, dy);
        
        //Manhattan
        return Mathf.Abs(A.X - B.X) + Mathf.Abs(A.Y - B.Y);
    }

    private static List<PathNode> RetracePath(PathNode startNode, PathNode currentNode) {
        List<PathNode> path = new List<PathNode>();
        do {
            path.Add(currentNode);
            currentNode = currentNode.parent;
            HandleAddToPath?.Invoke(currentNode);
        } while (currentNode != startNode);

        path.Reverse();
        return path;
    }
}