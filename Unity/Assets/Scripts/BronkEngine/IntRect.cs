using UnityEngine;
using System.Collections;

public struct IntRect{
	public int xMin;
	public int yMax;
	public int yMin;
	public int xMax;
	public IntRect(int xmin, int xmax, int ymin, int ymax)
	{
		xMin = xmin;
		xMax = xmax;
		yMin = ymin;
		yMax = ymax;
	}
}