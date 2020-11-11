using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SubregionSystem {

    public static List<Subregion> subregions { get; private set; } = new List<Subregion>();
    private const int _subregionSize = 10;

    public static void Reset() {
        subregions.Clear();
    }
    
    public static void CreateSubregions() {
        for (int x = 0; x < Pathfinder.MapWidth; x += 10) {
            for (int y = 0; y < Pathfinder.MapHeight; y += 10) {
                CreateSubregionAt(x, y);
            }
        }

        foreach(Subregion subregion in subregions) {
            subregion.FindNeighbours();
        }
    }

    public static void UpdateSubregionAt(int X, int Y) {
        foreach (Subregion newSubregion in CreateSubregionAt(X, Y)) {
          newSubregion.FindNeighbours();
        }
    }

    public static void RemoveSubregion(Subregion subregion) => subregions.Remove(subregion);

    private static List<Subregion> CreateSubregionAt(int X, int Y) {
        int subregionStartX = ((int) (X / _subregionSize)) * _subregionSize;
        int subregionStartY = ((int) (Y / _subregionSize)) * _subregionSize;

        //Deleting any subregions that existed on 10x10 chunk
        for (int x = subregionStartX; x < subregionStartX + _subregionSize; x++) {
            for (int y = subregionStartY; y < subregionStartY + _subregionSize; y++) {
                PathGrid.NodeAt(x, y)?.subregion?.Reset();
            }
        }

        //Creating new subregions on named chunk
        List<Subregion> createdSubregions = new List<Subregion>();
        for (int x = subregionStartX; x < subregionStartX + _subregionSize; x++) {
            for (int y = subregionStartY; y < subregionStartY + _subregionSize; y++) {
                Subregion newSubregion = FillSubregionFrom(PathGrid.NodeAt(x, y));
                if (newSubregion != null) {
                    newSubregion.CalculateAverageCoordinates();
                    createdSubregions.Add(newSubregion);
                }
            }
        }
        return createdSubregions;
    }

    private static Subregion FillSubregionFrom(Node node) {
        if (node == null || node.subregion != null || node.IsTraversable == false) {
            return null;
        }
        
        Subregion subregion = new Subregion();
        List<Node> openNodes = new List<Node>();
        openNodes.Add(node);
        do {
        } while(NextWaveIteration(ref openNodes, ref subregion));
        subregions.Add(subregion);
        return subregion;
    }

    private static bool NextWaveIteration(ref List<Node> openSet, ref Subregion subregion) {
        for (int i = openSet.Count - 1; i >= 0; i--) {
            if (openSet[i].subregion != null) {
                openSet.RemoveAt(i);
            }
        }

        if (openSet.Count == 0) {
            return false;
        }

        foreach (Node neighbour in openSet[0].GetNeighbours()) {
            if (!neighbour.IsTraversable || neighbour.subregion != null
                || !IsInsideArea(neighbour.X, neighbour.Y, ((int) (openSet[0].X / _subregionSize)) * _subregionSize, ((int) (openSet[0].Y / _subregionSize)) * _subregionSize)) {
                continue;
            }
            openSet.Add(neighbour);
        }

        subregion.AddNode(openSet[0]);
        openSet.RemoveAt(0);

        return true;
	}

    private static bool IsInsideArea(int x, int y, int subX, int subY) => (x >= subX && y >= subY && x < subX + _subregionSize && y < subY + _subregionSize);
}