using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathfindingTest
{
    class Pathfinding
    {
        #region class dummies
        private const int SIZE_X = 62;
        private const int SIZE_Y = 25;

        public class Vector3 {
            public float x;
            public float y;
            public float z;

            public Vector3(int p, int p_2, int p_3)
            {
                // TODO: Complete member initialization
                this.x = p;
                this.y = p_2;
                this.z = p_3;
            }
        }

        #endregion

        private const int MOVE_COST = 1;
        private Node[,] _nodes;
        private int _sizeX;
        private int _sizeY;
        private Node endNode;

        public class Node
        {
            public int x;
            public int y;
            public int f_totalCost;
            public int g_costFromStart;
            public int h_heuristicToFinish;
            public Node parent;
            public bool inOpen;
            public bool finished;
            public bool isBlocked;
        }

        public Pathfinding(int sizeX, int sizeY)
        {
            _sizeX = SIZE_X;
            _sizeY = SIZE_Y;

            _nodes = new Node[_sizeX, _sizeY];

            //init nodes
            {
                for (int y = 0; y < _sizeY; ++y)
                {
                    for (int x = 0; x < _sizeX; ++x)
                    {
                        _nodes[x, y] = new Node()
                        {
                            x = x,
                            y=y,
                            finished=false,
                            f_totalCost = int.MaxValue,
                            g_costFromStart = int.MaxValue
                        };

                        if(y == 3 && x > 0) {
                            var node = _nodes[x, y];
                            node.isBlocked = true;
                        }
                        else if (y == 5 && x < _sizeX - 1)
                        {
                            var node = _nodes[x, y];
                            node.isBlocked = true;
                        }

                    }
                }
            }
        }

        public List<Node> findPath(Vector3 start, Vector3 end)
        {
            var startNode = _nodes[(int)start.x, (int)start.y];
            endNode = _nodes[(int)end.x, (int)end.y];

            bool reachedEnd = false;

            startNode.g_costFromStart = 0;
            startNode.h_heuristicToFinish = Math.Abs(endNode.x - startNode.x) + Math.Abs(endNode.y - startNode.y);
            startNode.f_totalCost = startNode.g_costFromStart + startNode.h_heuristicToFinish;

            var openList = new List<Node>();
            openList.Add(startNode);

            int totalIterations = 0;

            while (openList.Count > 0)
            {
                totalIterations++;
                Node node = null;

                //find next item to iterate over
                int bestIndex = 0;
                int bestCost = openList[0].f_totalCost;
                for (int i = 1; i < openList.Count; i++)
                {
                    node = openList[i];
                    if (!node.finished)
                    {
                        if (node.f_totalCost < bestCost)
                        {
                            bestIndex = i;
                            bestCost = node.f_totalCost;
                        }
                    }
                }
                node = openList[bestIndex];
                node.finished = true;
                openList.Remove(node);
                //--

                if (node == endNode)
                {   
                    reachedEnd = true;
                    break;
                }
                addAdjacentNodes(ref openList, node);
            }

            Console.WriteLine("Total Iterations {0}", totalIterations);
            //build final path
            if (reachedEnd || endNode.parent != null)
            {
                var output = new List<Node>();
                var node = endNode;
                while (node.parent != null)
                {
                    output.Insert(0, node);
                    node = node.parent;
                }
                output.Insert(0, startNode);
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
            if (node.isBlocked)
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

        private void addAdjacentNodes(ref List<Node> openList, Node node)
        {
            //right
            if (node.x + 1 < _sizeX)
            {
                var tar = _nodes[node.x + 1, node.y];
                if (isValid(tar))
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
                if (isValid(tar))
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
                if (isValid(tar))
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
                if (isValid(tar))
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
                        var c = _nodes[x,y];
                        sb.Append(c.isBlocked ? "x" : "-");
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
                            sb.Append(c.isBlocked ? "x" : "-");
                        }
                    }
                    sb.Append(Environment.NewLine);
                }
            }
            Console.Write(sb);
        }
    }
}
