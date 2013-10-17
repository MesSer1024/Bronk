using System;
using System.Collections.Generic;

namespace Bronk
{
	public static class Game
	{
        internal static GameWorld World { get; set; }

		public enum States 
		{
			Frontend
			, Loading
			, Playing
			, InGameMenu
			, Finished
		}
		
		public static States state {
			get;
			set;
		}

    public static List<CubeLogic> getCubesBetween(CubeLogic cube1, CubeLogic cube2) {
        var items = new List<CubeLogic>();

        return items;
    }

	}
}

