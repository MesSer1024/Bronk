using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bronk
{
	public static class Game
	{
        public static GameWorld World { get; private set; }
        public static AIMain AI { get; private set; }
		public static float LogicTime { get; private set; }

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
	
		static Game()
		{
			Game.World = new GameWorld();
		}

        public static void init()
        {
            Game.World.init();
			Game.World.ViewComponent.init ();
            AI = new AIMain();
        }

		public static void Start()
		{
			for (var i = 0; i < 4; ++i)
			{
				var ant = AI.createAnt();
				ant.Position = new Vector2(World.StartArea.x + 3 + i, World.StartArea.y + 2 + i);
				Game.World.ViewComponent.SetTimeline (ant.ID, ant.GetPositionTimeline ());
			}
		}

		public static void update(float delta)
        {
			LogicTime = Time.time + 0.3f; // Logic buffer time 
            MessageManager.Update();
			World.Blocks.Update (LogicTime);
            AI.update(delta);
        }
    }
}

