﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : Singleton<MapManager> {

	public Tile TileAt(int x, int y) => Pathfinder.IsPositionViable(x, y) ? _grid[x, y] : null;

	public Vector2Int ActorPosition;
	public Vector2Int TargetPosition;
	
	private Tile[,] _grid;
	
	private void Start() {
		UpdateMapData();
	}

	public void UpdateMapData() {
		CreateGrid();
		Pathfinder.Reset();
		Pathfinder.Initialize(GridCreator.Instance.MapWidth, GridCreator.Instance.MapHeight, _grid);
	}

	public void SetActorPosition(Vector2Int position) {
		Drawer.Instance.ClearActorTargetPair();
		ActorPosition = position;
		Drawer.Instance.DrawActorTargetPair();
	}

	public void SetTargetPosition(Vector2Int position) {
		Drawer.Instance.ClearActorTargetPair();
		TargetPosition = position;
		Drawer.Instance.DrawActorTargetPair();
	}

	private void CreateGrid() {
		_grid = new Tile[GridCreator.Instance.MapWidth, GridCreator.Instance.MapHeight];
		for (int x = 0; x < GridCreator.Instance.MapWidth; x++) {
			for (int y = 0; y < GridCreator.Instance.MapHeight; y++) {
				_grid[x, y] = new Tile(x, y, true);
			}
		}
	}
}
