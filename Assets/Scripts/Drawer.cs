﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drawer : MonoBehaviour {
    
    public static Drawer Instance;
    
    public static Color GetRandomColor(float alpha) => new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), Mathf.Clamp(alpha, 0f, 1f));

    private void Awake() {
        if (Instance != null) {
            Destroy(Instance.gameObject);
        }

        Instance = this;
    }
    
    private void Update() {
        if (Input.GetKeyDown(KeyCode.L)) {
            StartCoroutine(DrawSubRegions());
        }
    }

    public void DrawActorTargetPair() {
        GridCreator.Instance.ColorTileAt(MapManager.GetInstance().ActorPosition.x, MapManager.GetInstance().ActorPosition.y, Color.green);
        GridCreator.Instance.ColorTileAt(MapManager.GetInstance().TargetPosition.x, MapManager.GetInstance().TargetPosition.y, Color.red);
    }
    
    public void ClearActorTargetPair() {
        GridCreator.Instance.ColorTileAt(MapManager.GetInstance().ActorPosition.x, MapManager.GetInstance().ActorPosition.y, GridCreator.TileType.Empty);
        GridCreator.Instance.ColorTileAt(MapManager.GetInstance().TargetPosition.x, MapManager.GetInstance().TargetPosition.y, GridCreator.TileType.Empty);
    }
    
    public void ClearAll() {
        foreach (var r in RegionSystem.regions) {
            foreach (var n in r.GetNodes()) {
                GridCreator.Instance.ColorTileAt(n.X, n.Y, GridCreator.TileType.Empty);
            }
        }

        DrawActorTargetPair();
    } 

    public IEnumerator DrawNodes(List<Node> nodes, GridCreator.TileType type) {
        foreach (var node in nodes) {
            GridCreator.Instance.ColorTileAt(node.X, node.Y, type);
            yield return null;
        }
    }
    
    private bool _isDrawingRegions = false;
    public IEnumerator DrawRegions() {
        if (_isDrawingRegions) {
            yield break;
        }

        _isDrawingRegions = true;
        
        HashSet<Node> nodesToReset = new HashSet<Node>();
        foreach (var r in RegionSystem.regions) {
            var regionColor = GetRandomColor(1);
            foreach (var n in r.GetNodes()) {
                GridCreator.Instance.ColorTileAt(n.X, n.Y, regionColor);
                nodesToReset.Add(n);
            }
        }

        while (InputManager.GetInstance().IsDrawingRegions) {
            yield return null;
        }
        
        foreach (var pathNode in nodesToReset) {
            GridCreator.Instance.ColorTileAt(pathNode.X, pathNode.Y,
                                             (pathNode.IsTraversable)
                                                 ? GridCreator.TileType.Empty
                                                 : GridCreator.TileType.Closed);
        }

        _isDrawingRegions = false;
    }
    
    private IEnumerator DrawSubRegions() {
        if (_isDrawingRegions) {
            yield break;
        }

        _isDrawingRegions = true;

        foreach (var r in RegionSystem.regions) {
            foreach (var sr in r.subregions) {
                var regionColor = GetRandomColor(1);
                foreach (var n in sr.nodes) {
                    GridCreator.Instance.ColorTileAt(n.X, n.Y, regionColor);
                }
            }
        }

        while (Input.GetKey(KeyCode.L)) {
            yield return null;
        }
        
        foreach (var r in RegionSystem.regions) {
            foreach (var sr in r.subregions) {
                var regionColor = GetRandomColor(1);
                foreach (var n in sr.nodes) {
                    GridCreator.Instance.ColorTileAt(n.X, n.Y, GridCreator.TileType.Empty);
                }
            }
        }

        _isDrawingRegions = false;
    }
}
