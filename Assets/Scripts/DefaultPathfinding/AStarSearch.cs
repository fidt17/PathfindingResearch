using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace DefaultPathfinding {
    public static class AStarSearch {
        public delegate void OnAddToPath(Node node);
        public static event OnAddToPath HandleAddToPath;

        public delegate void OnAddToClosedSet(Node node);
        public static event OnAddToClosedSet HandleAddToClosedSet;

        public static List<Node> GetPath(Node startNode, Node targetNode) {
            List<Node> openSet = new List<Node>();
            List<Node> closedSet = new List<Node>();
            openSet.Add(startNode);
            while (openSet.Count > 0) {
                Node currentNode = openSet[0];
                for (int i = 1; i < openSet.Count; i++) {
                    if (openSet[i].fCost <= currentNode.fCost) {
                        currentNode = openSet[i];
                    }
                }
                closedSet.Add(currentNode);
                openSet.Remove(currentNode);
                HandleAddToClosedSet?.Invoke(currentNode);

                if (currentNode == targetNode) {
                    var path = RetracePath(startNode, targetNode);
                    return path;
                }

                foreach (var neighbour in GetNeighbours(currentNode)) {
                    if (closedSet.Contains(neighbour)) {
                        continue;
                    }

                    int newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                    bool isInOpenSet = openSet.Contains(neighbour);
                    if (newCostToNeighbour < neighbour.gCost || !isInOpenSet) {
                        neighbour.gCost = newCostToNeighbour;
                        neighbour.hCost = Heuristic(neighbour, targetNode);
                        neighbour.rCost = 0;
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