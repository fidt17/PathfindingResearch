﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    private Vector2Int CursorToMapPosition() => ToGridPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    private Vector2Int ToGridPosition(Vector2 value) => new Vector2Int((int) (value.x + 0.5f), (int) (value.y + 0.5f));
    private Tile _prevTile;

    private delegate void MouseClickHandler(Vector2Int cursorPositionOnGrid);
    private event MouseClickHandler OnMouseClick;
    
    #region Setup buttons
    
    private void BuildWallAt(Vector2Int position) {
        Tile t = MapManager.GetInstance().TileAt(position.x, position.y);
        if (t != null && MapManager.GetInstance().ActorPosition != position && MapManager.GetInstance().TargetPosition != position) {
            t.SetTraversability(false);
        }
    }

    private void DemolishWallAt(Vector2Int position) {
        Tile t = MapManager.GetInstance().TileAt(position.x, position.y);
        t?.SetTraversability(true);
    }

    private void SetActorPosition(Vector2Int position) {
        Tile t = MapManager.GetInstance().TileAt(position.x, position.y);
        if (t != null && t.IsTraversable && MapManager.GetInstance().TargetPosition != position) {
            MapManager.GetInstance().SetActorPosition(position);
        }
    }
    
    private void SetTargetPosition(Vector2Int position) {
        Tile t = MapManager.GetInstance().TileAt(position.x, position.y);
        if (t != null && t.IsTraversable && MapManager.GetInstance().ActorPosition != position) {
            MapManager.GetInstance().SetTargetPosition(position);
        }
    }
    
    public void EnterBuildState()    => OnMouseClick = BuildWallAt;
    public void EnterDemolishState() => OnMouseClick = DemolishWallAt;
    public void EnterActorState()    => OnMouseClick = SetActorPosition;
    public void EnterTargetState()   => OnMouseClick = SetTargetPosition;
    
    #endregion
    
    #region Pathfinding related buttons

    public bool IsDrawingRegions = false;
    public void DrawRegions() {
        IsDrawingRegions = !IsDrawingRegions;
        if (IsDrawingRegions) {
            StartCoroutine(Drawer.Instance.DrawRegions());
        }
    }
    
    #endregion
    
    private void Update() {

        if (Input.GetMouseButton(0)) {
            OnMouseClick?.Invoke(CursorToMapPosition());
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            StartCoroutine(FindAndDrawPath());
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            StartCoroutine(FindAndDrawPathWithoutRegions());
        }
    }

    private List<PathNode> _closedList;
    private List<PathNode> _path;

    private void AddToClosedList(PathNode node) => _closedList.Add(node);
    private void AddToPath(PathNode node) => _path.Add(node);

    private bool _isDrawingPath = false;
    private IEnumerator FindAndDrawPath() {
        if (_isDrawingPath) {
            yield break;
        }

        _isDrawingPath = true;
        
        _closedList = new List<PathNode>();
        _path = new List<PathNode>();
        
        Pathfinder.TestTime(MapManager.GetInstance().ActorPosition, MapManager.GetInstance().TargetPosition, 10);
        
        AStarSearch.HandleAddToPath      += AddToPath;
        AStarSearch.HandleAddToClosedSet += AddToClosedList;
        
        Pathfinder.GetPath(MapManager.GetInstance().ActorPosition, MapManager.GetInstance().TargetPosition);

        Drawer.Instance.ClearAll();
        yield return StartCoroutine(Drawer.Instance.DrawNodes(_closedList, GridCreator.TileType.Closed));
        Debug.Log("Path Length: " + _path.Count);
        yield return StartCoroutine(Drawer.Instance.DrawNodes(_path, GridCreator.TileType.Path));
        
        AStarSearch.HandleAddToPath      -= AddToPath;
        AStarSearch.HandleAddToClosedSet -= AddToClosedList;

        _isDrawingPath = false;
    }
    
    private IEnumerator FindAndDrawPathWithoutRegions() {
        if (_isDrawingPath) {
            yield break;
        }

        _isDrawingPath = true;
        
        _closedList = new List<PathNode>();
        _path = new List<PathNode>();
        
        Pathfinder.TestTimeWithoutRegionSearch(MapManager.GetInstance().ActorPosition, MapManager.GetInstance().TargetPosition, 10);
        
        AStarSearch.HandleAddToPath      += AddToPath;
        AStarSearch.HandleAddToClosedSet += AddToClosedList;
        
        Pathfinder.GetPathWithoutRegionSearch(MapManager.GetInstance().ActorPosition, MapManager.GetInstance().TargetPosition);

        Drawer.Instance.ClearAll();
        yield return StartCoroutine(Drawer.Instance.DrawNodes(_closedList, GridCreator.TileType.Closed));
        yield return StartCoroutine(Drawer.Instance.DrawNodes(_path, GridCreator.TileType.Path));
        
        AStarSearch.HandleAddToPath      -= AddToPath;
        AStarSearch.HandleAddToClosedSet -= AddToClosedList;

        _isDrawingPath = false;
    }
}
