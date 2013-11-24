using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathfindingTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var p = new Pathfinding(100,100);
            var foo = p.findPath(new Pathfinding.Vector3(1, 1, 0), new Pathfinding.Vector3(10, 10, 0));

            p.printGraph(foo);
             
            Console.ReadLine();
        }
    }
}
