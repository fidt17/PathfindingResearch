using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public static class AStarSearch {
    
    public delegate void OnAddToPath(Node node);
    public static event OnAddToPath HandleAddToPath;

    public delegate void OnAddToClosedSet(Node node);
    public static event OnAddToClosedSet HandleAddToClosedSet;
    
    public static List<Node> GetPath(Node startNode, Node targetNode) {
        if (startNode == targetNode) {
            return new List<Node>();
        }
        
        if (startNode.Region != targetNode.Region) {
            return null;
        }

        List<Subregion> subregions = AStarSubregionSearch.GetPath(startNode.subregion, targetNode.subregion);
        Node[] possibleNodes = new Node[Pathfinder.MapWidth * Pathfinder.MapHeight];
        foreach (Subregion s in subregions) {
            foreach (Node n in s.nodes) {
                possibleNodes[n.X + n.Y * Pathfinder.MapHeight] = n;
            }
        }
        
        Heap<Node> openSet = new Heap<Node>(Pathfinder.MapWidth * Pathfinder.MapHeight);
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0) {
            Node currentNode = openSet.RemoveFirst();
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

    public static List<Node> GetPathWithoutRegionSearch(Node startNode, Node targetNode) {
        if (startNode == targetNode) {
            return new List<Node>();
        }
        
        if (startNode.Region != targetNode.Region) {
            return null;
        }

        Heap<Node> openSet = new Heap<Node>(Pathfinder.MapWidth * Pathfinder.MapHeight);
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0) {
            Node currentNode = openSet.RemoveFirst();
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

    private static List<Node> GetNeighbours(Node node) {
		List<Node> neighbours = new List<Node>();
		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {

				if( x == 0 && y == 0 ) {
					continue;
                }

				int checkX = node.X + x;
				int checkY = node.Y + y;
                Node n = PathGrid.NodeAt(checkX, checkY);
                if (n != null && n.IsTraversable) {
					neighbours.Add(n);
                }
			}
		}
		return neighbours;
	}

    private static int GetDistance(Node A, Node B) {
        int distX = Mathf.Abs(A.X - B.X);
        int distY = Mathf.Abs(A.Y - B.Y);
        if(distX > distY) {
            return 14 * distY + 10 * (distX - distY);
        }
        return 14 * distX + 10 * (distY - distX);
    }

    private static int Heuristic(Node A, Node B) {
        // Octile
        int dx = Mathf.Abs(A.X - B.X);
        int dy = Mathf.Abs(A.Y - B.Y);
        return 10 * (dx + dy) + (14 - 2 * 10) * Mathf.Min(dx, dy);
        
        //Manhattan
        return Mathf.Abs(A.X - B.X) + Mathf.Abs(A.Y - B.Y);
    }

    private static List<Node> RetracePath(Node startNode, Node currentNode) {
        List<Node> path = new List<Node>();
        do {
            path.Add(currentNode);
            currentNode = currentNode.parent;
            HandleAddToPath?.Invoke(currentNode);
        } while (currentNode != startNode);

        path.Reverse();
        return path;
    }
}