using System;
using System.Collections.Generic;

namespace Bronk
{
	public static class Game
	{
        public static GameWorld World { get; private set; }
        public static AIMain AI { get; private set; }

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

        public static List<CubeLogic> getCubesBetween(CubeLogic cube1, CubeLogic cube2)
        {
            var items = new List<CubeLogic>();

            return items;
        }

        public static void init()
        {
            Game.World = new GameWorld();
            Game.World.init();
            AI = new AIMain();
            AI.createAnt();
        }

        public static void update(float delta)
        {
            AI.update(delta);
        }
    }
}

