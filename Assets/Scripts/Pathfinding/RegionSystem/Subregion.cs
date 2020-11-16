using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Subregion {

    public HashSet<Node> nodes                  { get; protected set; } = new HashSet<Node>();
    public List<Subregion>  neighbouringSubregions { get; protected set; } = new List<Subregion>();

    public Region region { get; protected set; }

    public void SetRegion(Region region) => this.region = region;

    //Pathfinding
    public Subregion ParentSubregion;

    private Subregion _parent;
    
    public int       gCost, hCost;
    public int       fCost => gCost + hCost;

    public Subregion child;
    public int avergX,     avergY;
    //

    public void CalculateAverageCoordinates() {
        //Finding node with minimum distance to all other nodes
        int minSqrDistance = Int32.MaxValue;
        foreach (var node in nodes) {
            int currSqrDistance = 0;
            foreach (var border in nodes) {
                int dX = (border.X >= node.X) ? border.X - node.X : node.X - border.X;
                int dY = (border.Y >= node.Y) ? border.Y - node.Y : node.Y - border.Y;
                currSqrDistance += (int) (Mathf.Pow(dX, 2) + Mathf.Pow(dY, 2));
            }

            if (currSqrDistance < minSqrDistance) {
                minSqrDistance = currSqrDistance;
                avergX = node.X;
                avergY = node.Y;
            }
        }
    }
    
    public void AddNode(Node node) {
        nodes.Add(node);
        node.subregion = this;
    }

    public void RemoveNode(Node node) {
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
        foreach (Node node in nodes) {
            foreach (Node neighbour in node.GetNeighbours()) {
                if (neighbour.subregion != this && neighbour.subregion != null) {
                    AddNeighbour(neighbour.subregion);
                }
            }
        }
    }
}