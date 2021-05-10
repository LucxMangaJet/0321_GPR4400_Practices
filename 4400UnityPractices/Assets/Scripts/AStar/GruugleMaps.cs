using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GruugleMaps : MonoBehaviour
{
    [SerializeField] MapCity start, destination;

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

        //For example visualization
        List<MapCity> shortestRoute = new List<MapCity>();
        shortestRoute.Add(start);
        shortestRoute.Add(destination);

        //Implement AStar

        return shortestRoute;
    }

}
