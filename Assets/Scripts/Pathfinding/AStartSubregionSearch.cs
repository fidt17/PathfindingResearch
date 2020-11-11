﻿using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;

public static class AStarSubregionSearch {

    public static List<Subregion> GetPath(Subregion startSubregion, Subregion targetSubregion) {
        
        if (startSubregion.region != targetSubregion.region) {
            return null;
        }

        if (startSubregion == targetSubregion) {
            List<Subregion> result = new List<Subregion>();
            result.Add(startSubregion);
            return result;
        }

        List<Subregion> openSet = new List<Subregion>();
        List<Subregion> closedSet = new List<Subregion>();
        openSet.Add(startSubregion);
        while (openSet.Count > 0) {
            Subregion currentSubregion = openSet[0];
            for (int i = 1; i < openSet.Count; i++) {
                if ((openSet[i].fCost <= currentSubregion.fCost)
                    && (openSet[i].hCost < currentSubregion.hCost)) {
                    currentSubregion = openSet[i];
                }
            }

            openSet.Remove(currentSubregion);
            closedSet.Add(currentSubregion);
            if (currentSubregion == targetSubregion) {
                return RetracePath(startSubregion, targetSubregion);
            }

            foreach(Subregion neighbour in currentSubregion.neighbouringSubregions) {
                if(closedSet.Contains(neighbour)) {
                    continue;
                }

                int newMovementCostToNeighbour = currentSubregion.gCost + GetDistance(currentSubregion, neighbour);
                if(newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetSubregion);
                    neighbour.parent = currentSubregion;

                    if (!openSet.Contains(neighbour)) {
                        openSet.Add(neighbour);
                    }
                }
            }
        }

        return null;
    }

    private static int GetDistance(Subregion subA, Subregion subB) {

        PathNode A = PathGrid.NodeAt(subA.avergX, subA.avergY);
        PathNode B = PathGrid.NodeAt(subB.avergX, subB.avergY);
        
        int distX = Mathf.Abs(A.X - B.X);
        int distY = Mathf.Abs(A.Y - B.Y);

        if(distX > distY) {
            return 14 * distY + 10 * (distX - distY);
        }

        return 14 * distX + 10 * (distY - distX);
    }

    private static List<Subregion> RetracePath(Subregion startSubregion, Subregion endSubregion) {
        List<Subregion> path = new List<Subregion>();
        Subregion currentSubregion = endSubregion;
        while (currentSubregion != startSubregion) {
            path.Add(currentSubregion);
            currentSubregion = currentSubregion.parent;
        }
        path.Add(startSubregion);

        path.Reverse();
        return path;
    }
}