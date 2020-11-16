﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node> {

    public int X, Y;
    
    public Region Region => subregion?.region;
    public Subregion subregion;

    public bool IsTraversable;
    public int  gCost, hCost, rCost;
    public int  fCost => gCost + hCost + rCost;
    public Node parent;

    public int HeapIndex { get; set; }

    public Node(int x, int y, ITraversable tile) {
        X = x;
        Y = y;
        IsTraversable = tile.IsTraversable;
        tile.OnTraversabilityChange += HandleTraversabilityChange;
    }

    public void HandleTraversabilityChange(object source, EventArgs e) {
        if (e is Tile.TileArgs args) {
            IsTraversable = args.isTraversable;
            RegionSystem.UpdateSystemAt(X, Y);
        }
    }

    public int CompareTo(Node nodeToCompare) {
        
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
    
    public List<Node> GetNeighbours() {
        List<Node> neighbours = new List<Node>() {
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
}