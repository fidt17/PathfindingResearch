using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : Singleton<SaveManager> {

	private List<MapData> maps = new List<MapData>();
	private int     currentMapIndex = 0;
	
	private void Awake() {
		PreloadAllMapData();
	}

	private void PreloadAllMapData() {
		int i = 0;
		while (File.Exists(Application.dataPath + "/Maps/map" + i + ".json")) {
			string json = File.ReadAllText(Application.dataPath + "/Maps/map" + i + ".json");
			MapData data = JsonUtility.FromJson<MapData>(json);
			maps.Add(data);
			i++;
		}
		Debug.Log($"Loaded {i} maps.");
	}

	public void DeleteCurrentMap() {
		
	}

	public void LoadNextMap() {
		if (maps.Count != 0) {
			GridCreator.Instance.GenerateMapFromSaveData(maps[currentMapIndex]);

			currentMapIndex++;
			if (currentMapIndex >= maps.Count) {
				currentMapIndex = 0;
			}
		}
		
		Debug.Log($"Loaded {currentMapIndex} map.");
	}

	public void SaveCurrentMap() {
		MapData data = new MapData();

		data.mapWidth = GridCreator.Instance.MapWidth;
		data.mapHeight = GridCreator.Instance.MapHeight;
		
		data.ActorPosition = MapManager.GetInstance().ActorPosition;
		data.TargetPosition = MapManager.GetInstance().TargetPosition;
		
		data.TraversabilityMap = new bool[GridCreator.Instance.MapWidth * GridCreator.Instance.MapHeight];
		for (int x = 0; x < GridCreator.Instance.MapWidth; x++) {
			for (int y = 0; y < GridCreator.Instance.MapHeight; y++) {
				data.TraversabilityMap[x + y * GridCreator.Instance.MapHeight] = MapManager.GetInstance().TileAt(x, y).IsTraversable;
			}
		}

		string json = JsonUtility.ToJson(data);
		string mapPath = Application.dataPath + "/Maps/" + "map" + maps.Count + ".json";
		File.WriteAllText(mapPath, json);
		
		maps.Add(data);
		
		Debug.Log($"Map {maps.Count} was saved successfully at path: " + mapPath);
	}
}

public class MapData {
	public int mapWidth, mapHeight;
	public Vector2Int ActorPosition;
	public Vector2Int TargetPosition;
	public bool[] TraversabilityMap;
}