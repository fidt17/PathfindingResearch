﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : ITraversable {
	
	public event EventHandler OnTraversabilityChange;
	public class TileArgs : EventArgs {
		public bool isTraversable;
	}

	public bool IsTraversable { get; private set; } = true;

	public int X, Y;
	
	public Tile(int x, int y, bool isTraversable) {
		X = x;
		Y = y;
		SetTraversability(isTraversable);
	}
	
	public void SetTraversability(bool value) {
		if (IsTraversable != value) {
			IsTraversable = value;
			OnTraversabilityChange?.Invoke(this, new TileArgs() { isTraversable = IsTraversable});
			
			//GFX
			GridCreator.Instance.ColorTileAt(X, Y, (value) ? GridCreator.TileType.Empty : GridCreator.TileType.Wall);
		}
	}
}
