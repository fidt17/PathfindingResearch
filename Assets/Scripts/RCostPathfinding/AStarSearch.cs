using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace RCostPathfinding {

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

            List<Subregion> subregionsToErase = AStarSubregionSearch.GetPath(startNode.subregion, targetNode.subregion);
            Stack<Subregion> subregions = new Stack<Subregion>(subregionsToErase);
            
            Heap<Node> openSet = new Heap<Node>(Pathfinder.MapWidth * Pathfinder.MapHeight);
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0) {
                Node currentNode;
                do {
                    //debug
                    if (openSet.Count == 0) {
                        foreach (var subregion in subregionsToErase) {
                            subregion.child = null;
                        }
                        return null;
                    }
                    
                    currentNode = openSet.RemoveFirst();
                    if (subregions.Count == 0 || currentNode.subregion == subregions.Peek()) {
                        break;
                    }
                } while (true);
                
                closedSet.Add(currentNode);
                HandleAddToClosedSet?.Invoke(currentNode);
                
                if (currentNode == targetNode) {
                    var path = RetracePath(startNode, targetNode);

                    foreach (var subregion in subregionsToErase) {
                        subregion.child = null;
                    }
                    
                    return path;
                }

                foreach (var neighbour in GetNeighbours(currentNode)) {
                    if (closedSet.Contains(neighbour)) {
                        continue;
                    }
                    
                    if (subregions.Count != 0 && subregions.Peek().child == neighbour.subregion) {
                        subregions.Pop();
                    }
                    
                    if (closedSet.Contains(neighbour) || (subregions.Count != 0 && !subregions.Peek().nodes.Contains(neighbour))) {
                        continue;
                    }

                    int newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                    bool isInOpenSet = openSet.Contains(neighbour);
                    if (newCostToNeighbour < neighbour.gCost || !isInOpenSet) {
                        neighbour.gCost = newCostToNeighbour;
                        neighbour.hCost = Heuristic(neighbour, targetNode);

                        if (neighbour.subregion.child != null) {
                            neighbour.rCost = GetDistance(neighbour, PathGrid.NodeAt(neighbour.subregion.child.avergX, neighbour.subregion.child.avergY));
                        }
                        else {
                            neighbour.rCost = 0;
                        }
                        
                        neighbour.parent = currentNode;

                        if (!isInOpenSet) {
                            openSet.Add(neighbour);
                        }
                    }
                }
            }
            
            foreach (var subregion in subregionsToErase) {
                subregion.child = null;
            }

            return null;
        }
        
        public static List<Node> GetPathNew(Node startNode, Node targetNode) {
            if (startNode == targetNode) {
                return new List<Node>();
            }

            if (startNode.Region != targetNode.Region) {
                return null;
            }

            Stack<Subregion> subregions = new Stack<Subregion>(AStarSubregionSearch.GetPath(startNode.subregion, targetNode.subregion));
            
            Heap<Node> openSet = new Heap<Node>(Pathfinder.MapWidth * Pathfinder.MapHeight);
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0) {
                Node currentNode;
                do {
                    currentNode = openSet.RemoveFirst();
                    if (subregions.Count == 0 || currentNode.subregion == subregions.Peek()) {
                        break;
                    }
                } while (true);
                
                closedSet.Add(currentNode);
                HandleAddToClosedSet?.Invoke(currentNode);
                
                if (currentNode == targetNode) {
                    return RetracePath(startNode, targetNode);
                }

                foreach (var neighbour in GetNeighbours(currentNode)) {
                    if (closedSet.Contains(neighbour)) {
                        continue;
                    }
                    
                    if (subregions.Count != 0 && subregions.Peek().child == neighbour.subregion) {
                        subregions.Peek().child = null;
                        subregions.Pop();
                    }
                    
                    if (closedSet.Contains(neighbour) || (subregions.Count != 0 && !subregions.Peek().nodes.Contains(neighbour))) {
                        continue;
                    }

                    int newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                    bool isInOpenSet = openSet.Contains(neighbour);
                    if (newCostToNeighbour < neighbour.gCost || !isInOpenSet) {
                        neighbour.gCost = newCostToNeighbour;
                        neighbour.hCost = Heuristic(neighbour, targetNode);

                        if (neighbour.subregion.child != null) {
                            neighbour.rCost = GetDistance(neighbour, PathGrid.NodeAt(neighbour.subregion.child.avergX, neighbour.subregion.child.avergY));
                        }
                        else {
                            neighbour.rCost = 0;
                        }
                        
                        neighbour.parent = currentNode;

                        if (!isInOpenSet) {
                            openSet.Add(neighbour);
                        }
                    }
                }
            }

            return null;
        }

        private static List<Node> GetNeighbours(Node node) {
            List<Node> neighbours = new List<Node>();
            for (int x = -1; x <= 1; x++) {
                for (int y = -1; y <= 1; y++) {

                    if (x == 0 && y == 0) {
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
            if (distX > distY) {
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
}