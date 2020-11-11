﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode : IHeapItem<PathNode> {

    public int X;
    public int Y;
    
    public Region Region => subregion?.region;
    public Subregion subregion;
    public PathNode parent;

    public bool isTraversable;
    public int  gCost, hCost;
    public int  heapIndex;
    public int  fCost => gCost + hCost;

    public PathNode(int x, int y, ITraversable tile) {
        X = x;
        Y = y;
        isTraversable = tile.IsTraversable;
        tile.OnTraversabilityChange += HandleTraversabilityChange;
    }

    public void HandleTraversabilityChange(object source, EventArgs e) {
        if (e is Tile.TileArgs args) {
            isTraversable = args.isTraversable;
            RegionSystem.UpdateSystemAt(X, Y);
        }
    }

    public int CompareTo(PathNode nodeToCompare) {
        
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare == 0) {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;
        /*
        int compare = hCost.CompareTo(nodeToCompare.hCost);
        if (compare == 0) {
            compare = fCost.CompareTo(nodeToCompare.fCost);
        }
        return -compare;
        */
        
    }
    
    public List<PathNode> GetNeighbours() {
        List<PathNode> neighbours = new List<PathNode>() {
            PathGrid.NodeAt(X    , Y + 1),
            PathGrid.NodeAt(X        + 1, Y),
            PathGrid.NodeAt(X    , Y - 1),
            PathGrid.NodeAt(X        - 1, Y)
        };

        for (int i = neighbours.Count - 1; i >= 0; i--) {
            if (neighbours[i] == null) {
                neighbours.RemoveAt(i);
            }
        }

        return neighbours;
    }

    public int HeapIndex {
        get {
            return heapIndex;
        }
        set {
            heapIndex = value;
        }
    }
}