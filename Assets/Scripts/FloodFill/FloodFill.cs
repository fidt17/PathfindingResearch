using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FloodFill {
    
    public delegate void OnAddToRegion(Node n);

    public static event OnAddToRegion HandleAddNodeToRegion;

    private static readonly Color ReplacementColor = Color.white;

    public static void FillMap() {
        ResetNodesColors();
        
        for (int x = 0; x < Pathfinder.MapWidth; x++) {
            for (int y = 0; y < Pathfinder.MapHeight; y++) {
                var n = PathGrid.NodeAt(x, y);
                if (n.IsTraversable && n.rColor == ReplacementColor) {
                    FloodFillFrom(n, Drawer.GetRandomColor(1));
                } 
            }
        }
    }

    private static void FloodFillFrom(Node startNode, Color targetColor) {
        Queue<Node> nodeQueue = new Queue<Node>();
        nodeQueue.Enqueue(startNode);
        startNode.rColor = targetColor;
        HandleAddNodeToRegion?.Invoke(startNode);
        
        while (nodeQueue.Count != 0) {
            foreach (var n in nodeQueue.Dequeue().GetNeighbours()) {
                if (n.IsTraversable && n.rColor == ReplacementColor) {
                    n.rColor = targetColor;
                    nodeQueue.Enqueue(n);
                    HandleAddNodeToRegion?.Invoke(n);
                }
            }
        }
    }

    private static void ResetNodesColors() {
        for (int x = 0; x < Pathfinder.MapWidth; x++) {
            for (int y = 0; y < Pathfinder.MapHeight; y++) {
                PathGrid.NodeAt(x, y).rColor = ReplacementColor;
            }
        }
    }
}
