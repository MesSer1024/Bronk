using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bronk
{
	class CubesSelectedMessage : IMessage
	{
        private string _id;
        private List<CubeLogic> _cubes;
        private int _action;

        public CubesSelectedMessage(string id, List<CubeLogic> cubes, int actionToPerformPlaceholder)
        {
            _id = id;
            _cubes = cubes;
            _action = actionToPerformPlaceholder;
        }

        public string getGroup()
        {
            return MessageManager.GameMessage;
        }

        public string getId()
        {
            return _id;
        }

        public List<CubeLogic> getCubes()
        {
            return _cubes;
        }

        /// <summary>
        /// What action to perform with the cube, can probably not be an int in the end...
        /// </summary>
        /// <returns></returns>
        public int getAction()
        {
            return _action;
        }
    }
}
