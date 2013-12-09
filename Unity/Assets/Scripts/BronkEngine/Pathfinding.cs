﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Bronk
{
    public class Pathfinding
    {
        private const int MOVE_COST = 1;
        private Node[,] _nodes;
        private int _sizeX;
        private int _sizeY;
        private Node endNode;
		private GameWorldData _blocks;

		private List<Node> _openList = new List<Node>();
		private int _pathfindCounter;
		private int _pathfindID;

        public class Node
        {
            public int x;
            public int y;
            public Node parent;
            public int f_totalCost;
            public int g_costFromStart;
            public int h_heuristicToFinish;
			public int blockID;
            public bool inOpen;
            public bool finished;
			public int pathfindID;
        }

        /// <summary>
        /// Create a pathfinding graph that is possible to use for finding out collisions, [0,0] = top left, [sizeX,sizeY] = bottom right
        /// Uses CubeData to find out if a tile is walkable or not, expecting tiles to be ordered as (x=0,y=0, x=1,y=0, x=2,y=0) ...
        /// </summary>
        /// <param name="sizeX"></param>
        /// <param name="sizeY"></param>
        /// <param name="cubes"></param>
		public Pathfinding(int sizeX, int sizeY, GameWorldData blocks)
        {
            _sizeX = sizeX;
            _sizeY = sizeY;
			_blocks = blocks;

            _nodes = new Node[_sizeX, _sizeY];
            int i = 0;
            //init nodes
            for (int y = 0; y < _sizeY; ++y)
            {
                for (int x = 0; x < _sizeX; ++x)
                {
                    _nodes[x, y] = new Node()
                    {
						blockID = i++,
                        x = x,
                        y = y,
                        finished = false,
                        f_totalCost = int.MaxValue,
                        g_costFromStart = int.MaxValue
                    };
                }
            }
            
        }

		private void MaybeResetNode(Node node)
		{
			if (node.pathfindID != _pathfindID)
			{
				node.finished = false;
				node.f_totalCost = int.MaxValue;
				node.g_costFromStart = int.MaxValue;
				node.inOpen = false;
				node.h_heuristicToFinish = 0;
				node.parent = null;
				node.pathfindID = _pathfindID;
			}
		}

		public bool isNodeBlocked(Node node) {
			return _blocks.GetBlockType(node.blockID) != GameWorld.BlockType.DirtGround;
		}

        /// <summary>
        /// Expecting a vector containing x & y-values, z does not matter... Will include final node in response if a path is found, no matter if it is blocked or not
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public List<Node> findPath(Vector3 start, Vector3 end)
        {
			_pathfindID = _pathfindCounter;
			_pathfindCounter++;
			endNode = _nodes[(int)start.x, (int)start.y];
			var startNode = _nodes[(int)end.x, (int)end.y];

			MaybeResetNode (startNode);
			MaybeResetNode (endNode);

            bool reachedEnd = false;

            startNode.g_costFromStart = 0;
            startNode.h_heuristicToFinish = Math.Abs(endNode.x - startNode.x) + Math.Abs(endNode.y - startNode.y);
            startNode.f_totalCost = startNode.g_costFromStart + startNode.h_heuristicToFinish;

            _openList.Add(startNode);

            int totalIterations = 0;

            while (_openList.Count > 0)
            {
                totalIterations++;
                Node node = null;

                //find next item to iterate over
                int bestIndex = 0;
                int bestCost = _openList[0].f_totalCost;
                for (int i = 1; i < _openList.Count; i++)
                {
                    node = _openList[i];
                    if (!node.finished)
                    {
                        if (node.f_totalCost < bestCost)
                        {
                            bestIndex = i;
                            bestCost = node.f_totalCost;
                        }
                    }
                }
                node = _openList[bestIndex];
                node.finished = true;
				_openList.RemoveAt(bestIndex);
                //--

                if (node == endNode)
                {
                    reachedEnd = true;
                    break;
                }
                addAdjacentNodes(ref _openList, node, endNode);
            }

			_openList.Clear ();
            //build final path
            if (reachedEnd || endNode.parent != null)
            {
                var output = new List<Node>();
                var node = endNode;
                while (node.parent != null)
                {
					output.Add(node);
                    node = node.parent;
                }
				output.Add(startNode);
                return output;
            }
            else
            {
                return null;
            }
        }


        private bool isValid(Node node)
        {
            if (node.finished)
                return false;
			if (isNodeBlocked(node))
                return false;
            return true;
        }

        private void tryUpdateNodeWith(Node tar, Node node)
        {
            //if it isn’t on the open list, add it to the open list. Make the current square the parent of this square. Record the F, G, and H costs of the square. 
            if (tar.inOpen == false)
            {
                if (node.g_costFromStart + MOVE_COST < tar.g_costFromStart)
                {
                    tar.g_costFromStart = node.g_costFromStart + MOVE_COST;
                    tar.parent = node;
                }

                tar.h_heuristicToFinish = Math.Abs(endNode.x - tar.x) + Math.Abs(endNode.y - tar.y);
                tar.f_totalCost = tar.g_costFromStart + tar.h_heuristicToFinish;
            }
            else
            {
                //If it is on the open list already, check to see if this path to that square is better, using G cost as the measure. 
                //A lower G cost means that this is a better path. If so, change the parent of the square to the current square, and recalculate the G and F scores of the square. 
                //If you are keeping your open list sorted by F score, you may need to resort the list to account for the change.
                if (node.g_costFromStart + MOVE_COST < tar.g_costFromStart)
                {
                    tar.g_costFromStart = node.g_costFromStart + MOVE_COST;
                    tar.f_totalCost = tar.g_costFromStart + tar.h_heuristicToFinish;
                    tar.parent = node;
                }
            }
        }

        private void addAdjacentNodes(ref List<Node> openList, Node node, Node targetNode)
        {
            //right
            if (node.x + 1 < _sizeX)
            {
                var tar = _nodes[node.x + 1, node.y];
				MaybeResetNode (tar);
                if (tar == targetNode || isValid(tar))
                {
                    //1
                    tryUpdateNodeWith(tar, node);
                    //2
                    if (tar.inOpen == false)
                    {
                        tar.inOpen = true;
                        openList.Add(tar);
                    }
                }
            }
            //left
            if (node.x - 1 >= 0)
            {
				var tar = _nodes[node.x - 1, node.y];
				MaybeResetNode (tar);
                if (tar == targetNode || isValid(tar))
                {
                    //1
                    tryUpdateNodeWith(tar, node);
                    //2
                    if (tar.inOpen == false)
                    {
                        tar.inOpen = true;
                        openList.Add(tar);
                    }
                }
            }
            //top
            if (node.y - 1 >= 0)
            {
				var tar = _nodes[node.x, node.y - 1];
				MaybeResetNode (tar);
                if (tar == targetNode || isValid(tar))
                {
                    //1
                    tryUpdateNodeWith(tar, node);
                    //2
                    if (tar.inOpen == false)
                    {
                        tar.inOpen = true;
                        openList.Add(tar);
                    }
                }
            }
            //bottom
            if (node.y + 1 < _sizeY)
            {
				var tar = _nodes[node.x, node.y + 1];
				MaybeResetNode (tar);
                if (tar == targetNode || isValid(tar))
                {
                    //1
                    tryUpdateNodeWith(tar, node);
                    //2
                    if (tar.inOpen == false)
                    {
                        tar.inOpen = true;
                        openList.Add(tar);
                    }
                }
            }
        }

        public void printGraph(List<Node> targetNodes)
        {
            StringBuilder sb = new StringBuilder();
            if (targetNodes == null)
            {
                for (var y = 0; y < _sizeY; ++y)
                {
                    for (int x = 0; x < _sizeX; x++)
                    {
                        var c = _nodes[x, y];
						sb.Append(isNodeBlocked(c) ? "x" : "-");
                    }
                    sb.Append(Environment.NewLine);
                }
            }
            else
            {
                for (var y = 0; y < _sizeY; ++y)
                {
                    for (int x = 0; x < _sizeX; x++)
                    {
                        var c = _nodes[x, y];
                        if (targetNodes.Contains(c))
                        {
                            sb.Append("o");
                        }
                        else
                        {
							sb.Append(isNodeBlocked(c) ? "x" : "-");
                        }
                    }
                    sb.Append(Environment.NewLine);
                }
            }
            Console.Write(sb);
        }
    }
}
