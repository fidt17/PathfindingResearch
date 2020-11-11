﻿using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Pathfinder {

    public static int MapWidth, MapHeight;
    public static bool IsPositionViable(int x, int y) => x >= 0 && y >= 0 && x < MapWidth && y < MapHeight;
    
    public static void Initialize(int mapWidth, int mapHeight, ITraversable[,] tileGrid) {
        MapWidth = mapWidth;
        MapHeight = mapHeight;
        
        PathGrid.CreateGrid(tileGrid);
        RegionSystem.Initialize();
    }

    public static void Reset() {
        RegionSystem.Reset();
    }
    
    #region A* + Subregion search
    
    public static List<PathNode> GetPath(Vector2Int startPosition, Vector2Int targetPosition) {
        var startNode = PathGrid.NodeAt(startPosition.x, startPosition.y);
        var targetNode = PathGrid.NodeAt(targetPosition.x, targetPosition.y);
        var path = AStarSearch.GetPath(startNode, targetNode);
        return path;
    } 
    
    public static void TestTime(Vector2Int startPosition, Vector2Int targetPosition, int testCount) {
        var startNode = PathGrid.NodeAt(startPosition.x, startPosition.y);
        var targetNode = PathGrid.NodeAt(targetPosition.x, targetPosition.y);

        string AlgoName = "A* Region HashSet";
        float T = 0;
        for (int i = 0; i < testCount; i++) {
            float startTime = Time.realtimeSinceStartup;
            AStarSearch.GetPath(startNode, targetNode);
            T += (Time.realtimeSinceStartup - startTime) * 1000 / testCount;
        }
        Debug.Log($"{AlgoName}: {T} ms.");
    } 
    
    #endregion

    #region A* without subregion search
    
    public static List<PathNode> GetPathWithoutRegionSearch(Vector2Int startPosition, Vector2Int targetPosition) {
        var startNode = PathGrid.NodeAt(startPosition.x, startPosition.y);
        var targetNode = PathGrid.NodeAt(targetPosition.x, targetPosition.y);
        var path = AStarSearch.GetPathWithoutRegionSearch(startNode, targetNode);
        return path;
    } 
    
    public static void TestTimeWithoutRegionSearch(Vector2Int startPosition, Vector2Int targetPosition, int testCount) {
        var startNode = PathGrid.NodeAt(startPosition.x, startPosition.y);
        var targetNode = PathGrid.NodeAt(targetPosition.x, targetPosition.y);

        string AlgoName = "A* no regions";
        float T = 0;
        for (int i = 0; i < testCount; i++) {
            float startTime = Time.realtimeSinceStartup;
            AStarSearch.GetPathWithoutRegionSearch(startNode, targetNode);
            T += (Time.realtimeSinceStartup - startTime) * 1000 / testCount;
        }
        Debug.Log($"{AlgoName}: {T} ms.");
    }
    
    #endregion
}