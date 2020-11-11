﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PathGrid {

    private static Node[,] _nodes;
    private static bool _isInitialized = false;

    public static Node NodeAt(int x, int y) {
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
        _nodes = new Node[Pathfinder.MapWidth, Pathfinder.MapHeight];
        for (int x = 0; x < Pathfinder.MapWidth; x++) {
            for (int y = 0; y < Pathfinder.MapHeight; y++) {
                _nodes[x, y] = new Node(x, y, tileGrid[x, y]);
            }
        }
    }
}