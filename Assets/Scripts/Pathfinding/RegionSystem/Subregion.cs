using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Subregion {

    public HashSet<PathNode> nodes                  { get; protected set; } = new HashSet<PathNode>();
    public List<Subregion>  neighbouringSubregions { get; protected set; } = new List<Subregion>();

    public Region region { get; protected set; }

    public void SetRegion(Region region) => this.region = region;

    //Pathfinding
    public Subregion parent;
    public int       gCost, hCost;
    public int       fCost => gCost + hCost;

    public int avergX, avergY;
    //

    public void CalculateAverageCoordinates() {
        float x = 0;
        float y = 0;
        foreach (var node in nodes) {
            x += node.X;
            y += node.Y;
        }
        
        avergX = Mathf.RoundToInt(x / nodes.Count);
        avergY = Mathf.RoundToInt(y / nodes.Count);
    }
    
    public void AddNode(PathNode node) {
        nodes.Add(node);
        node.subregion = this;
    }

    public void RemoveNode(PathNode node) {
        nodes.Remove(node);
        node.subregion = null;
    }

    public void AddNeighbour(Subregion neighbour) {
        if (neighbouringSubregions.Contains(neighbour)) {
            return;
        }
        neighbouringSubregions.Add(neighbour);
        neighbour.AddNeighbour(this);
    }

    public void RemoveNeighbour(Subregion neighbour) {
        neighbouringSubregions.Remove(neighbour);
        neighbour.neighbouringSubregions.Remove(this);
    }

    public void Reset() {
        foreach (var node in nodes) {
            node.subregion = null;
        }
        nodes.Clear();
        /*
        for (int i = nodes.Count - 1; i >= 0; i--) {
            RemoveNode(nodes[i]);
        }
        */

        for (int i = neighbouringSubregions.Count - 1; i >= 0; i--) {
            RemoveNeighbour(neighbouringSubregions[i]);
        }

        region?.RemoveSubregion(this);

        SubregionSystem.RemoveSubregion(this);
    }

    public void FindNeighbours() {
        foreach (PathNode node in nodes) {
            foreach (PathNode neighbour in node.GetNeighbours()) {
                if (neighbour.subregion != this && neighbour.subregion != null) {
                    AddNeighbour(neighbour.subregion);
                }
            }
        }
    }
}