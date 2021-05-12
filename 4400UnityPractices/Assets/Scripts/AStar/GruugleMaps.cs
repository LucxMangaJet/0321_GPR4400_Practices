using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GruugleMaps : MonoBehaviour
{
    [SerializeField] MapCity start, destination;
    [SerializeField] MapHandler mapHandler;

    List<MapCity> shortestRoute;

    private void Update()
    {
        shortestRoute = FindShortestRoute();
    }

    private void OnDrawGizmos()
    {
        if (shortestRoute != null)
        {
            Gizmos.color = Color.green;

            for (int i = 0; i < shortestRoute.Count - 1; i++)
            {
                MapCity cityA = shortestRoute[i];
                MapCity cityB = shortestRoute[i + 1];

                if (cityA && cityB)
                {
                    Gizmos.DrawLine(cityA.transform.position, cityB.transform.position);
                }

            }
        }
    }


    private List<MapCity> FindShortestRoute()
    {
        if (start == null || destination == null)
            return null;

        //Implement AStar
        return AStar(start, destination, mapHandler.GetDistanceBetween);
    }

    public delegate float HeuristicDelegate(MapCity a, MapCity b);

    private List<MapCity> AStar(MapCity start, MapCity goal, HeuristicDelegate heuristic)
    {
        //Should be min-heap or priority queue, will use List for readability
        List<MapCity> openSet = new List<MapCity>() { start };


        Dictionary<MapCity, MapCity> cameFrom = new Dictionary<MapCity, MapCity>();
        Dictionary<MapCity, float> gScore = new Dictionary<MapCity, float>();
        gScore[start] = 0;

        Dictionary<MapCity, float> fScore = new Dictionary<MapCity, float>();
        fScore[start] = heuristic(start, goal);


        while (openSet.Count > 0)
        {
            MapCity current = GetLowestFScore(openSet, fScore);

            if (current == goal)
                return ReconstructPath(cameFrom, current);

            openSet.Remove(current);

            foreach (var neighbour in mapHandler.GetNeighboursOf(current))
            {
                //Here GetDistanceBetween between is used as the actual connection weigth
                float tentativeGScore = gScore[current] + mapHandler.GetDistanceBetween(current, neighbour);

                if (!gScore.ContainsKey(neighbour) || tentativeGScore < gScore[neighbour])
                {
                    cameFrom[neighbour] = current;
                    gScore[neighbour] = tentativeGScore;
                    fScore[neighbour] = tentativeGScore + heuristic(neighbour, goal);

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }

        return null;
    }

    private List<MapCity> ReconstructPath(Dictionary<MapCity, MapCity> cameFrom, MapCity current)
    {
        List<MapCity> totalPath = new List<MapCity>() { current };

        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            totalPath.Add(current);
        }

        return totalPath;
    }

    // This operation can occur in O(1) time if openSet is a min-heap or a priority queue !!
    // Here: O(n)
    private MapCity GetLowestFScore(List<MapCity> openSet, Dictionary<MapCity, float> fScore)
    {
        float min = float.MaxValue;
        MapCity minCity = null;

        foreach (var e in openSet)
        {
            if (fScore.ContainsKey(e))
            {
                float score = fScore[e];
                if (score < min)
                {
                    min = score;
                    minCity = e;
                }
            }
        }
        return minCity;
    }
}

