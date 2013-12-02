using UnityEngine;
using System.Collections.Generic;

namespace Bronk
{
	public class DigJob 
	{
		public int BlockID;
		public List<Ant> AssignedAnts = new List<Ant>();
		public DigJob(int blockID)
		{
			BlockID = blockID;
		}
	}
}