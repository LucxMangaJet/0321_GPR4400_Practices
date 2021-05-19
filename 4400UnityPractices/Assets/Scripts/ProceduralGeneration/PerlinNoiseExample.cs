using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoiseExample : MonoBehaviour
{
    [SerializeField] TerrainData terrainData;
    [SerializeField] float scale;

    void Start()
    {
        int size = terrainData.heightmapResolution;

        float[,] heights = new float[size, size];



        for (int x = 0; x < size; ++x)
        {
            for (int y = 0; y < size; ++y)
            {
                float percentX = (float)x / size;
                float percentY = (float)y / size;

                heights[x, y] = Mathf.PerlinNoise(percentX * scale, percentY * scale);

                float distante = Vector2.Distance(new Vector2(0.5f, 0.5f), new Vector2(percentX, percentY)) * 2;

                heights[x, y] *= 1 - distante;


            }
        }

        terrainData.SetHeights(0, 0, heights);
    }

}
