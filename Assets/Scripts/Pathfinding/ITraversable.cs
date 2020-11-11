﻿using System;

public interface ITraversable {
	event EventHandler OnTraversabilityChange;
	
	bool IsTraversable { get; }
	
	void SetTraversability(bool value);
}