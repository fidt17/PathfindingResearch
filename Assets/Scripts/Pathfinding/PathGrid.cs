﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PathGrid {

    private static PathNode[,] _nodes;
    private static bool _isInitialized = false;

    public static PathNode NodeAt(int x, int y) {
        if (_isInitialized == false) {
            return null;
        }

        if (Pathfinder.IsPositionViable(x, y)) {
            return _nodes[x, y];
        }
        return null;
    }

    public static void CreateGrid(ITraversable[,] tileGrid) {
        _isInitialized = true;
        _nodes = new PathNode[Pathfinder.MapWidth, Pathfinder.MapHeight];
        for (int x = 0; x < Pathfinder.MapWidth; x++) {
            for (int y = 0; y < Pathfinder.MapHeight; y++) {
                _nodes[x, y] = new PathNode(x, y, tileGrid[x, y]);
            }
        }
    }
}