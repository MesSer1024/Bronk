using System;
using System.Collections.Generic;

namespace Bronk
{
	public static class Game
	{
        internal static GameWorld World { get; private set; }
        internal static AIMain AI { get; private set; }

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

            var items = new List<CubeLogic>();

        return items;
    }

        }	}
}

