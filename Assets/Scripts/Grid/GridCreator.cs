using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class GridCreator : MonoBehaviour {

	public static GridCreator Instance;
	
	public enum TileType {
		Empty,
		Wall,
		Path,
		Closed,
		Open
	}
	
	[Header("World dimensions")] public int MapWidth, MapHeight;

	[Header("Tile Settings")]
	public GameObject TilePrefab;

	public Color EmptyColor;
	public Color WallColor;
	public Color PathColor;
	public Color ClosedSetColor;
	public Color OpenSetColor;

	private Transform        _gridParent;
	private SpriteRenderer[] _tiles;

	public void ColorTileAt(int x, int y, Color color) {
		_tiles[x + y * MapHeight].color = color;
	}
	
	public void ColorTileAt(int x, int y, TileType type) {
		_tiles[x + y * MapHeight].color = GetColorByType(type);
	}

	public void GenerateMapFromSaveData(MapData data) {
		MapWidth = data.mapWidth;
		MapHeight = data.mapHeight;
		MapManager.GetInstance().UpdateMapData();
		GenerateGrid();

		for (int x = 0; x < MapWidth; x++) {
			for (int y = 0; y < MapHeight; y++) {
				MapManager.GetInstance().TileAt(x, y).SetTraversability(data.TraversabilityMap[x + y * MapHeight]);
			}
		}
		
		Drawer.Instance.DrawActorTargetPair();
		SetCameraPosition();
	}
	
	private void Awake() {
		if (Instance != null) {
			Destroy(Instance.gameObject);
		}

		Instance = this;
		GenerateGrid();
		SetCameraPosition();
	}

	private void SetCameraPosition() {
		Camera.main.transform.localPosition = new Vector3(MapWidth / 2f, MapHeight / 2f, -100);
		Camera.main.orthographicSize = MapWidth / 2f + 2;
	}
	
	private void GenerateGrid() {
		if (_gridParent != null) {
			DestroyImmediate(_gridParent.gameObject);
		}

		_gridParent = (new GameObject("Grid")).transform;			
		_tiles = new SpriteRenderer[MapWidth * MapHeight];
		
		for (int x = 0; x < MapWidth; x++) {
			for (int y = 0; y < MapHeight; y++) {
				_tiles[x + y * MapHeight] = Instantiate(TilePrefab, new Vector3(x, y, 0), Quaternion.identity, _gridParent).GetComponent<SpriteRenderer>();
				ColorTileAt(x, y, TileType.Empty);
			}
		}
	}
	
	private Color GetColorByType(TileType type) {
		switch (type) {
			case TileType.Empty:
				return EmptyColor;
			
			case TileType.Wall:
				return WallColor;
			
			case TileType.Path:
				return PathColor;
			
			case TileType.Closed:
				return ClosedSetColor;
			
			case TileType.Open:
				return OpenSetColor;
			
			default:
				return Color.clear;
		}
	}
}
