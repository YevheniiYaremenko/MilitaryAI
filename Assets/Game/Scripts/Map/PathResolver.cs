using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace App.Map
{
    public class PathNode
    {
        public Waypoint waypoint;
        public PathNode lastNode;
        public float lenghtToStart;
        public float remainingLenght;
        //full lenght from start to end throught this way
        public float ExpectedLenght { get { return remainingLenght + lenghtToStart; } }
    }

    public static class PathResolver
    {
        public static List<Waypoint> FindPath(List<Waypoint> graph, Waypoint startPoint, Waypoint endPoint)
        {
            List<PathNode> openNodes = new List<PathNode>();
            List<PathNode> closedNodes = new List<PathNode>();

            PathNode startNode = new PathNode()
            {
                waypoint = startPoint,
                lastNode = null,
                lenghtToStart = 0,
                remainingLenght = RemainingLenght(startPoint, endPoint)
            };

            openNodes.Add(startNode);

            while (openNodes.Count>0)
            {
                var currentNode = openNodes.OrderBy(
                    node => node.ExpectedLenght).First();
                if (currentNode.waypoint==endPoint)
                {
                    return GetPath(currentNode);
                }

                openNodes.Remove(currentNode);
                closedNodes.Add(currentNode);

                var neighbours = GetNeighbours(currentNode, endPoint);
                if (neighbours.Count==0)
                {
                    continue;
                }
                foreach (var neighbourNode in neighbours)
                {
                    if (closedNodes.Count(node=>node.waypoint==neighbourNode.waypoint)>0)
                    {
                        continue;
                    }
                    var openNode = openNodes.FirstOrDefault(node => node.waypoint == neighbourNode.waypoint);
                    if (openNode==null)
                    {
                        openNodes.Add(neighbourNode);
                    }
                    else if (openNode.lenghtToStart>neighbourNode.lenghtToStart)
                    {
                        openNode.lastNode = currentNode;
                        openNode.lenghtToStart = neighbourNode.lenghtToStart;
                    }
                }
            }
            return null;
        }

        static float RemainingLenght(Waypoint a1, Waypoint a2)
        {
            return (a1.Position - a2.Position).magnitude;
        }

        static List<Waypoint> GetPath(PathNode node)
        {
            var result = new List<Waypoint>();
            var currentNode = node;
            while (currentNode.lastNode != null)
            {
                result.Add(currentNode.waypoint);
                currentNode = currentNode.lastNode;
            }
            result.Reverse();
            return result;
        }

        static List<PathNode> GetNeighbours(PathNode currentNode, Waypoint endPoint)
        {
            List<PathNode> neighbours = new List<PathNode>();
            if (currentNode.waypoint.Relations.Count>0)
            {
                foreach(var neighbour in currentNode.waypoint.Relations)
                {
                    neighbours.Add( new PathNode()
                    {
                        waypoint = neighbour,
                        lastNode = currentNode,
                        lenghtToStart = currentNode.lenghtToStart + (currentNode.waypoint.Position - neighbour.Position).magnitude,
                        remainingLenght = RemainingLenght(neighbour, endPoint)
                    });
                }
            }
            return neighbours;
        }
    }
}
