using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MapHandler : MonoBehaviour
{
    [SerializeField] float maxDistance;
    [SerializeField] bool visualize;

    Dictionary<MapCity, List<MapCity>> neighboursMap = new Dictionary<MapCity, List<MapCity>>();



    //This is not performant at all! It is done in this way soly to allow editing everything in the editor
    private void Update()
    {
        MapCity[] cities = FindObjectsOfType<MapCity>();

        for (int a = 0; a < cities.Length; a++)
        {
            MapCity cityA = cities[a];
            List<MapCity> neighbours = new List<MapCity>();

            for (int b = 0; b < cities.Length; b++)
            {
                MapCity cityB = cities[b];
                float dist = GetDistanceBetween(cityA, cityB);
                if (dist < maxDistance)
                    neighbours.Add(cityB);
            }
            neighboursMap[cityA] = neighbours;
        }

    }


    public float GetDistanceBetween(MapCity a, MapCity b)
    {
        return (b.transform.position - a.transform.position).magnitude;
    }

    public List<MapCity> GetNeighboursOf(MapCity city)
    {
        if (neighboursMap.ContainsKey(city))
        {
            return neighboursMap[city];
        }
        throw new System.ArgumentException("Requested information on a city that has not been registered.");

    }

    private void OnDrawGizmos()
    {
        if (!visualize)
            return;

        foreach (var element in neighboursMap)
        {
            MapCity key = element.Key;
            foreach (var neighbour in element.Value)
            {
                float s = 1.3f - GetDistanceBetween(key, neighbour) / maxDistance;
                Gizmos.color = new Color(s, s, s, 1);
                Gizmos.DrawLine(key.transform.position, neighbour.transform.position);
            }
        }
    }

}

