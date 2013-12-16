using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Bronk
{
	class PathfindingToBase : IMessageListener
	{
        private const int MOVE_COST = 1;
        private Bronk.Pathfinding.Node[,] _nodes;
        private int _sizeX;
        private int _sizeY;
        private Bronk.Pathfinding.Node _homeBaseNode;
        private GameWorldData _blocks;

        private List<Bronk.Pathfinding.Node> _openList = new List<Bronk.Pathfinding.Node>();

        /// <summary>
        /// Basically this algorithm is there for always keeping an update of how much the travel-cost is from each node to pathfind to base
        /// </summary>
        /// <param name="sizeX"></param>
        /// <param name="sizeY"></param>
        /// <param name="blocks"></param>
        public PathfindingToBase(int sizeX, int sizeY, GameWorldData blocks) {
            _sizeX = sizeX;
            _sizeY = sizeY;
            _blocks = blocks;
            _nodes = new Bronk.Pathfinding.Node[_sizeX, _sizeY];
            MessageManager.AddListener(this);
        }

        public void onMessage(IMessage message) {
            if (message is BlockMinedMessage) {
                var msg = message as BlockMinedMessage;
                Debug.Log("Pathfinding BlockMined blockID=" + msg.BlockID);
                var updatedNode = nodeFromID(msg.BlockID);

                var neighbours = FindNonBlockedNeighbours(updatedNode);
                var bestNeighbour = neighbours[0];
                for (int i = 1; i < neighbours.Count; ++i ) {
                    var currNode = neighbours[i];
                    if (currNode.g_costFromStart < bestNeighbour.g_costFromStart) {
                        bestNeighbour = currNode;
                    }
                }

                updatedNode.g_costFromStart = bestNeighbour.g_costFromStart + MOVE_COST;
                updatedNode.h_heuristicToFinish = Math.Abs(_homeBaseNode.x - updatedNode.x) + Math.Abs(_homeBaseNode.y - updatedNode.y);
                updatedNode.f_totalCost = updatedNode.g_costFromStart + updatedNode.h_heuristicToFinish;
                updatedNode.parent = bestNeighbour;
                //if we have more than one neighbour that means that we might have found a new cheaper way and need to update graph
                if (neighbours.Count > 1) {
                    MessageManager.QueueMessage(new ScheduleGraphUpdateMessage(msg.BlockID, neighbours));
                }

            }
        }

        public void updateGraphBasedOnNode(int blockID, List<Pathfinding.Node> blockNeighbours) {
            var updatedNode = nodeFromID(blockID);
            var changedNodes = new List<Pathfinding.Node>(4);

            foreach (var node in blockNeighbours)
            {
                if (node.g_costFromStart > updatedNode.g_costFromStart + MOVE_COST) {
                    node.g_costFromStart = updatedNode.g_costFromStart + MOVE_COST;
                    node.parent = updatedNode;
                    node.h_heuristicToFinish = Math.Abs(_homeBaseNode.x - node.x) + Math.Abs(_homeBaseNode.y - node.y);
                    node.f_totalCost = node.g_costFromStart + node.h_heuristicToFinish;
                    changedNodes.Add(node);
                }
            }
            foreach (var node in changedNodes) {
                updateGraphBasedOnNode(node.blockID, FindNonBlockedNeighbours(node));
            }
        }

        private Bronk.Pathfinding.Node nodeFromID(int blockID) {
            return _nodes[blockID % _sizeX, (int)(blockID / _sizeX)];
        }

        private void resetNodes() {
            int i = 0;
            for (int y = 0; y < _sizeY; ++y) {
                for (int x = 0; x < _sizeX; ++x) {
                    _nodes[x, y] = new Bronk.Pathfinding.Node() {
                        blockID = i++,
                        x = x,
                        y = y,
                        finished = false,
                        f_totalCost = int.MaxValue,
                        g_costFromStart = int.MaxValue,
                        h_heuristicToFinish = int.MaxValue,
                        parent = null,
                        inOpen = false,
                    };
                }
            }
        }

        public void init(int baseBlockID) {
            resetNodes();
            _homeBaseNode = _nodes[baseBlockID % _sizeX, (int)(baseBlockID/_sizeX)];

            /** Basic Algorithm for this kind of pathfinder
             * Iterate through all elements that are accessible from home base
             * Update them with cheapest cost to traverse to homebase
             * Same as for A* to traverse from each point towards homebase - but in the reverse direction
             */

            _homeBaseNode.g_costFromStart = 0;
            _homeBaseNode.h_heuristicToFinish = 0;
            _homeBaseNode.f_totalCost = 0;

            _openList.Add(_homeBaseNode);

            while (_openList.Count > 0) {
                Bronk.Pathfinding.Node node = null;

                int bestIndex = 0;
                node = _openList[bestIndex];
                node.finished = true;
                _openList.RemoveAt(bestIndex);
                addAdjacentNodes(_openList, node);
            }
        }

        private void addAdjacentNodes(List<Bronk.Pathfinding.Node> openList, Bronk.Pathfinding.Node node) {
            var addedNodes = FindNonBlockedNeighbours(node);

            //update every node
            foreach (var tar in addedNodes) {
                if(tar.finished)
                    continue;

                //1
                tryUpdateNodeWith(tar, node);
                //2
                if (tar.inOpen == false) {
                    tar.inOpen = true;
                    openList.Add(tar);
                }
            }
        }

        private List<Pathfinding.Node> FindNonBlockedNeighbours(Bronk.Pathfinding.Node node) {
            var addedNodes = new List<Pathfinding.Node>(4);
            //right
            if (node.x + 1 < _sizeX) {
                var tar = _nodes[node.x + 1, node.y];
                if (tar == _homeBaseNode || !isNodeBlocked(tar)) {
                    addedNodes.Add(tar);
                }
            }
            //left
            if (node.x - 1 >= 0) {
                var tar = _nodes[node.x - 1, node.y];
                if (tar == _homeBaseNode || !isNodeBlocked(tar)) {
                    addedNodes.Add(tar);
                }
            }
            //top
            if (node.y - 1 >= 0) {
                var tar = _nodes[node.x, node.y - 1];
                if (tar == _homeBaseNode || !isNodeBlocked(tar)) {
                    addedNodes.Add(tar);
                }
            }
            //bottom
            if (node.y + 1 < _sizeY) {
                var tar = _nodes[node.x, node.y + 1];
                if (tar == _homeBaseNode || !isNodeBlocked(node)) {
                    addedNodes.Add(tar);
                }
            }

            return addedNodes;
        }

        public bool isNodeBlocked(Bronk.Pathfinding.Node node) {
            return _blocks.GetBlockType(node.blockID) != GameWorld.BlockType.DirtGround;
        }

        private void tryUpdateNodeWith(Bronk.Pathfinding.Node updatedNode, Bronk.Pathfinding.Node parentNode) {
            //if it isn’t on the open list - Make the current square the parent of this square. Record the F, G, and H costs of the square. 
            if (updatedNode.inOpen == false) {
                if (parentNode.g_costFromStart + MOVE_COST < updatedNode.g_costFromStart) {
                    updatedNode.g_costFromStart = parentNode.g_costFromStart + MOVE_COST;
                    updatedNode.parent = parentNode;
                }

                updatedNode.h_heuristicToFinish = Math.Abs(_homeBaseNode.x - updatedNode.x) + Math.Abs(_homeBaseNode.y - updatedNode.y);
                updatedNode.f_totalCost = updatedNode.g_costFromStart + updatedNode.h_heuristicToFinish;
            } else {
                //If it is on the open list already, check to see if this path to that square is better, using G cost as the measure. 
                //A lower G cost means that this is a better path. If so, change the parent of the square to the current square, and recalculate the G and F scores of the square. 
                //If you are keeping your open list sorted by F score, you may need to resort the list to account for the change.
                if (parentNode.g_costFromStart + MOVE_COST < updatedNode.g_costFromStart) {
                    updatedNode.g_costFromStart = parentNode.g_costFromStart + MOVE_COST;
                    updatedNode.f_totalCost = updatedNode.g_costFromStart + updatedNode.h_heuristicToFinish;
                    updatedNode.parent = parentNode;
                }
            }
        }

        public List<Pathfinding.Node> pathfindToHomebaseFrom(int currentBlockID) {
            var output = new List<Pathfinding.Node>();
            var node = _nodes[currentBlockID % _sizeX, (int)(currentBlockID/_sizeX)];

            if (node.parent != null) {
                //we have a valid path from this block to home base
                while (node != _homeBaseNode) {
                    output.Add(node);
                    node = node.parent;
                }
                output.Add(_homeBaseNode);
            } else {
                //this block is "not reachable" from base move the ant to a nearby block
                if (node.blockID == _homeBaseNode.blockID) {
                    //("Trying to move to home position from homeposition...");
                    //throw new Exception("from homebase to homebase-error");
                    output.Add(node);
                } else {
                    var neighbours = FindNonBlockedNeighbours(node);
                    if (neighbours.Count > 0) {
                        int bestIndex = 0;
                        int bestCost = neighbours[0].g_costFromStart;
                        for (int i = 1; i < neighbours.Count; ++i) {
                            var neighbour = neighbours[i];
                            if (neighbour.g_costFromStart < bestCost) {
                                bestIndex = i;
                                bestCost = neighbours[i].g_costFromStart;
                            }
                        }
                        
                        node = neighbours[bestIndex];
                        while (node != _homeBaseNode) {
                            output.Add(node);
                            node = node.parent;
                        }
                        output.Add(_homeBaseNode);
                    }
                }
            }

            return output.Count > 0 ? output : null;
        }
    }
}
