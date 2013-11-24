using System;
using System.Collections.Generic;
using UnityEngine;

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
            for (var i = 0; i < 4; ++i)
            {
                var ant = AI.createAnt();
                ant.Position = new Vector3(World.StartArea.x + 3 + i, 0, World.StartArea.y + 2 + i);
            }
        }

        public static void update(float delta)
        {
            AI.update(delta);
        }
    }
}

