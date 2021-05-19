using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class IslandGenerator : MonoBehaviour
{
    [SerializeField] TerrainData terrainData;
    [SerializeField] bool regenerate;
    [SerializeField] bool generateContinuously;

    [Header("Heighmap")]
    [SerializeField] float scale;
    [SerializeField] Vector2 offset;

    [SerializeField] float detailScale;
    [SerializeField] Vector2 detailOffset;
    [SerializeField] float detailIntensity;


    [Header("Alphamap")]
    [SerializeField] float sandTransitionStrength = 10;
    [SerializeField] float rockMinAngle = 25;
    [SerializeField] float rockStrengthDivisor = 30;

    [Range(0, 1)]
    [SerializeField] float sandEnd;

    private void OnEnable()
    {
        UnityEditor.EditorApplication.update += OnUpdate;
    }

    private void OnUpdate()
    {
        if (regenerate || generateContinuously)
        {
            regenerate = false;
            GenerateTerrain();
        }
    }

    private void OnDisable()
    {
        UnityEditor.EditorApplication.update -= OnUpdate;
    }

    private void GenerateTerrain()
    {
        if (!terrainData) return;

        float[,] heights = new float[terrainData.heightmapResolution, terrainData.heightmapResolution];
        float[,,] textures = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, 3];

        float halfHeight = terrainData.heightmapResolution / 2;
        Vector2 midPoint = new Vector2(halfHeight, halfHeight);

        for (int x = 0; x < heights.GetLength(0); x++)
        {
            for (int y = 0; y < heights.GetLength(1); y++)
            {
                float percentX = (float)x / heights.GetLength(0);
                float percentY = (float)y / heights.GetLength(1);

                float noise = Mathf.PerlinNoise(percentX * scale + offset.x, percentY * scale + offset.y);
                float detailNoise = Mathf.PerlinNoise(percentX * detailScale + detailOffset.x, percentY * detailScale + detailOffset.y) * detailIntensity;

                float dist = 1 - Vector2.Distance(midPoint, new Vector2(x, y)) / (halfHeight);

                float h = Mathf.Clamp01((noise + detailNoise) * dist);
                SetAlphamapValues(textures, x, y, percentX, percentY, h);

                heights[x, y] = h;
            }
        }

        terrainData.SetAlphamaps(0, 0, textures);
        terrainData.SetHeights(0, 0, heights);
    }

    private void SetAlphamapValues(float[,,] textures, int x, int y, float percentX, float percentY, float h)
    {
        if (x < textures.GetLength(0) && y < textures.GetLength(1))
        {

            bool fullSand = h < sandEnd;

            if (fullSand)
            {
                textures[x, y, 2] = 1;
            }
            else
            {
                float sandRemaining = Mathf.Clamp01(1 - (h - sandEnd) * sandTransitionStrength);
                textures[x, y, 2] = sandRemaining;

                //show rock texture based on steepness;
                var angle = terrainData.GetSteepness(percentX, percentY);

                float steepness = Mathf.Clamp01((angle - rockMinAngle) / rockStrengthDivisor);

                textures[x, y, 1] = steepness * (1 - sandRemaining);
                textures[x, y, 0] = (1 - steepness) * (1 - sandRemaining);
            }
        }
    }
}
