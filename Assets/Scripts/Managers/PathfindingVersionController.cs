using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingVersionController : MonoBehaviour {

	public float visualizationSpeed = 1f;
	
	[Header("No Upgrades")]
	[SerializeField] private TMPro.TMP_Text AStarNoUpgradesTimeValue;
	[SerializeField] private TMPro.TMP_Text AStarNoUpgradesPathLength;
	
	[Header("Data structure upgrade")]
	[SerializeField] private TMPro.TMP_Text AStarDataStructureUpgradeTimeValue;
	[SerializeField] private TMPro.TMP_Text AStarDataStructureUpgradePathLength;

	[Header("Region upgrade")]
	[SerializeField] private TMPro.TMP_Text AStarRegionUpgradeTimeValue;
	[SerializeField] private TMPro.TMP_Text AStarRegionUpgradePathLength;
	
	[Header("RCost upgrade")]
	[SerializeField] private TMPro.TMP_Text AStarRCostUpgradeTimeValue;
	[SerializeField] private TMPro.TMP_Text AStarRCostUpgradePathLength;
	
	[Header("RCost upgrade")]
	[SerializeField] private TMPro.TMP_Text AStarRCostNewUpgradeTimeValue;
	[SerializeField] private TMPro.TMP_Text AStarRCostNewUpgradePathLength;
	
	private readonly PathfindingVersion _aStarNoUpgrades           = new PathfindingVersion();
	private readonly PathfindingVersion _aStarDataStructureUpgrade = new PathfindingVersion();
	private readonly PathfindingVersion _aStarRegionUpgrade        = new PathfindingVersion();
	private readonly PathfindingVersion _aStarRCostUpgrade         = new PathfindingVersion();
	private readonly PathfindingVersion _aStarRCostNewUpgrade         = new PathfindingVersion();

	private Vector2Int Actor  => MapManager.GetInstance().ActorPosition;
	private Vector2Int Target => MapManager.GetInstance().TargetPosition;

	private void Update() {
		if (Input.GetKeyDown(KeyCode.M)) {
			//FindAStarNoUpgrades();
			FindAStarDataStructureUpgrade();
			FindAStarRegionUpgrade();
			FindAStarRCostUpgrade();
			FindAStarRCostNewUpgrade();
		}
	}
	
	#region No Upgrades
	
	public void FindAStarNoUpgrades() {
		_aStarNoUpgrades.Reset();
		DefaultPathfinding.AStarSearch.HandleAddToPath      += _aStarNoUpgrades.AddToPath;
		DefaultPathfinding.AStarSearch.HandleAddToClosedSet += _aStarNoUpgrades.AddToClosedSet;
		
		float startTime = Time.realtimeSinceStartup;
		Pathfinder.GetDefaultPathfinding(Actor, Target);
		float resultTime = (Time.realtimeSinceStartup - startTime) * 1000;
		
		AStarNoUpgradesTimeValue.text = resultTime.ToString();
		AStarNoUpgradesPathLength.text = _aStarNoUpgrades.Path.Count.ToString();
		
		DefaultPathfinding.AStarSearch.HandleAddToPath      -= _aStarNoUpgrades.AddToPath;
		DefaultPathfinding.AStarSearch.HandleAddToClosedSet -= _aStarNoUpgrades.AddToClosedSet;
	}

	public void VisualizeAStarNoUpgrades() {
		StopAllCoroutines();
		StartCoroutine(VisualizeVersion(_aStarNoUpgrades));
	}
	
	#endregion
	
	#region Data Structure Upgrade

	public void FindAStarDataStructureUpgrade() {
		_aStarDataStructureUpgrade.Reset();
		AStarSearch.HandleAddToPath      += _aStarDataStructureUpgrade.AddToPath;
		AStarSearch.HandleAddToClosedSet += _aStarDataStructureUpgrade.AddToClosedSet;
		
		float startTime = Time.realtimeSinceStartup;
		AStarSearch.GetPathWithoutRegionSearch(PathGrid.NodeAt(Actor.x, Actor.y), PathGrid.NodeAt(Target.x, Target.y));
		float resultTime = (Time.realtimeSinceStartup - startTime) * 1000;
		
		AStarDataStructureUpgradeTimeValue.text = resultTime.ToString();
		AStarDataStructureUpgradePathLength.text = _aStarDataStructureUpgrade.Path.Count.ToString();
		
		AStarSearch.HandleAddToPath      -= _aStarDataStructureUpgrade.AddToPath;
		AStarSearch.HandleAddToClosedSet -= _aStarDataStructureUpgrade.AddToClosedSet;
	}

	public void VisualizeAStarDataStructureUpgrade() {
		StopAllCoroutines();
		StartCoroutine(VisualizeVersion(_aStarDataStructureUpgrade));
	}
	
	#endregion
	
	#region Region Upgrade

	public void FindAStarRegionUpgrade() {
		_aStarRegionUpgrade.Reset();
		AStarSearch.HandleAddToPath      += _aStarRegionUpgrade.AddToPath;
		AStarSearch.HandleAddToClosedSet += _aStarRegionUpgrade.AddToClosedSet;
		
		float startTime = Time.realtimeSinceStartup;
		AStarSearch.GetPath(PathGrid.NodeAt(Actor.x, Actor.y), PathGrid.NodeAt(Target.x, Target.y));
		float resultTime = (Time.realtimeSinceStartup - startTime) * 1000;
		
		AStarRegionUpgradeTimeValue.text = resultTime.ToString();
		AStarRegionUpgradePathLength.text = _aStarRegionUpgrade.Path.Count.ToString();
		
		AStarSearch.HandleAddToPath      -= _aStarRegionUpgrade.AddToPath;
		AStarSearch.HandleAddToClosedSet -= _aStarRegionUpgrade.AddToClosedSet;
	}

	public void VisualizeAStarRegionUpgrade() {
		StopAllCoroutines();
		StartCoroutine(VisualizeVersion(_aStarRegionUpgrade));
	}
	
	#endregion
	
	#region RCost Upgrade

	public void FindAStarRCostUpgrade() {
		_aStarRCostUpgrade.Reset();
		RCostPathfinding.AStarSearch.HandleAddToPath      += _aStarRCostUpgrade.AddToPath;
		RCostPathfinding.AStarSearch.HandleAddToClosedSet += _aStarRCostUpgrade.AddToClosedSet;
		
		float startTime;
		int testCount = 50;
		float resultTime = 0;
		for (int i = 0; i < testCount; i++) {
			startTime = Time.realtimeSinceStartup;
			RCostPathfinding.AStarSearch.GetPath(PathGrid.NodeAt(Actor.x, Actor.y), PathGrid.NodeAt(Target.x, Target.y));
			resultTime += (Time.realtimeSinceStartup - startTime) * 1000 / testCount;
		}
		
		AStarRCostUpgradeTimeValue.text = resultTime.ToString();
		AStarRCostUpgradePathLength.text = _aStarRCostUpgrade.Path.Count.ToString();
		
		RCostPathfinding.AStarSearch.HandleAddToPath      -= _aStarRCostUpgrade.AddToPath;
		RCostPathfinding.AStarSearch.HandleAddToClosedSet -= _aStarRCostUpgrade.AddToClosedSet;
	}

	public void VisualizeAStarRCostUpgrade() {
		StopAllCoroutines();
		StartCoroutine(VisualizeVersion(_aStarRCostUpgrade));
	}
	
	#endregion

	#region RCost New Upgrade

	public void FindAStarRCostNewUpgrade() {
		_aStarRCostNewUpgrade.Reset();
		RCostPathfinding.AStarSearch.HandleAddToPath      += _aStarRCostNewUpgrade.AddToPath;
		RCostPathfinding.AStarSearch.HandleAddToClosedSet += _aStarRCostNewUpgrade.AddToClosedSet;
		
		float startTime;
		int testCount = 50;
		float resultTime = 0;
		for (int i = 0; i < testCount; i++) {
			startTime = Time.realtimeSinceStartup;
			RCostPathfinding.AStarSearch.GetPathNew(PathGrid.NodeAt(Actor.x, Actor.y), PathGrid.NodeAt(Target.x, Target.y));
			resultTime += (Time.realtimeSinceStartup - startTime) * 1000 / testCount;
		}
		
		AStarRCostNewUpgradeTimeValue.text = resultTime.ToString();
		AStarRCostNewUpgradePathLength.text = _aStarRCostNewUpgrade.Path.Count.ToString();
		
		RCostPathfinding.AStarSearch.HandleAddToPath      -= _aStarRCostNewUpgrade.AddToPath;
		RCostPathfinding.AStarSearch.HandleAddToClosedSet -= _aStarRCostNewUpgrade.AddToClosedSet;
	}

	public void VisualizeAStarRCostNewUpgrade() {
		StopAllCoroutines();
		StartCoroutine(VisualizeVersion(_aStarRCostNewUpgrade));
	}
	
	#endregion
	
	private IEnumerator VisualizeVersion(PathfindingVersion version) {
		Drawer.Instance.ClearAll();
		//closed set
		int frameCount = 0;
		foreach (var n in version.ClosedSet) {
			GridCreator.Instance.ColorTileAt(n.X, n.Y, GridCreator.TileType.Closed);
			frameCount++;
			if (frameCount > visualizationSpeed) {
				frameCount = 0;
				yield return null;
			} 
		}
		
		//open set
		frameCount = 0;
		foreach (var n in version.Path) {
			GridCreator.Instance.ColorTileAt(n.X, n.Y, GridCreator.TileType.Path);
			frameCount++;
			if (frameCount > visualizationSpeed) {
				frameCount = 0;
				yield return null;
			} 
		}
	}
}

public class PathfindingVersion {
	public readonly List<Node> ClosedSet = new List<Node>();
	public readonly List<Node> Path      = new List<Node>();

	public void AddToClosedSet(Node node) => ClosedSet.Add(node);
	public void AddToPath(Node node)      => Path.Add(node);

	public void Reset() {
		ClosedSet.Clear();
		Path.Clear();
	}
}
