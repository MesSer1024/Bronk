using System;

namespace Bronk
{
	public static class Game
	{
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
	}
}

