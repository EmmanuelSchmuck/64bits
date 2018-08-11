using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TerrainManager : MonoBehaviour
{

    public Terrain terrain;
    [Header("HeightMap")]
    public Vector3 terrainSizeOverride = new Vector3(500, 100, 500);
    public AnimationCurve radialHeightFactor;
    public float hmPerlinScale = 1f;
    public float hmPerlinScale2 = 1f;
    public Vector2Int hmPerlinSeed;
    public Vector2Int hmPerlinSeed2;
    public float hmHarmonicFactor = 0.5f;

    [Header("AlphaMap")]
    public float amPerlinScale = 1f;
    public float amPerlinScale2 = 1f;
    public Vector2Int amPerlinSeed;
    public Vector2Int amPerlinSeed2;
    public float amHarmonicFactor = 0.5f;

    [Header("DetailMap")]
    public int detailDensity = 1;
    public int minAltitude = 0;
    public int maxAltitude = 100;
    public float detPerlinThreshold = 0.5f;
    public float detPerlinScale = 1f;
    public float detPerlinScale2 = 1f;
    public Vector2Int detPerlinSeed;
    public Vector2Int detPerlinSeed2;
    public float detHarmonicFactor = 0.5f;

    public bool generate;

    // Use this for initialization
    void Start()
    {

        Generate();

    }

    private void Update()
    {
        if (generate || Input.GetKeyDown(KeyCode.G)) Generate(); generate = false;
    }

    public void Generate()
    {
        GameObject player = GameObject.FindWithTag("Player");
        
        if(Application.isPlaying) player.GetComponentInChildren<CameraResolutionManager>().TriggerEvolution();


        hmPerlinSeed = new Vector2Int(Random.Range(0, 999), Random.Range(0, 999));
        hmPerlinSeed2 = new Vector2Int(Random.Range(0, 999), Random.Range(0, 999));
        amPerlinSeed = new Vector2Int(Random.Range(0, 999), Random.Range(0, 999));
        amPerlinSeed2 = new Vector2Int(Random.Range(0, 999), Random.Range(0, 999));
        detPerlinSeed = new Vector2Int(Random.Range(0, 999), Random.Range(0, 999));
        detPerlinSeed2 = new Vector2Int(Random.Range(0, 999), Random.Range(0, 999));

        Debug.Log("Generating heightmap...");

        int heightMapWidth = terrain.terrainData.heightmapWidth;
        terrain.terrainData.size = terrainSizeOverride;

        Vector2Int heightMapCenter = new Vector2Int(heightMapWidth / 2, heightMapWidth / 2);

        float[,] heightMap = new float[heightMapWidth, heightMapWidth];

        for (int x = 0; x < heightMapWidth; x++)
        {
            for (int y = 0; y < heightMapWidth; y++)
            {
                float distanceToCenter = Vector2Int.Distance(heightMapCenter, new Vector2Int(x, y)) / (heightMapWidth / 2);
                float normalizedHeight = (1 - hmHarmonicFactor) * Mathf.PerlinNoise((hmPerlinSeed.x + x) * hmPerlinScale, (hmPerlinSeed.y + y) * hmPerlinScale);
                normalizedHeight += (hmHarmonicFactor) * Mathf.PerlinNoise((hmPerlinSeed2.x + x) * hmPerlinScale2, (hmPerlinSeed2.y + y) * hmPerlinScale2);
                heightMap[x, y] = normalizedHeight * radialHeightFactor.Evaluate(distanceToCenter);
            }
        }

        terrain.terrainData.SetHeights(0, 0, heightMap);

        Debug.Log("Generating alpha map...");

        int alphaMapWidth = terrain.terrainData.alphamapWidth;
        int alphaTextureCount = 1;

        float[,,] alphaMap = new float[alphaMapWidth, alphaMapWidth, alphaTextureCount];

        for (int x = 0; x < alphaMapWidth; x++)
        {
            for (int y = 0; y < alphaMapWidth; y++)
            {
                float weight = (1 - amHarmonicFactor) * Mathf.PerlinNoise((amPerlinSeed.x + x) * amPerlinScale, (amPerlinSeed.y + y) * amPerlinScale);
                weight += (amHarmonicFactor) * Mathf.PerlinNoise((amPerlinSeed2.x + x) * amPerlinScale2, (amPerlinSeed2.y + y) * amPerlinScale2);
                alphaMap[x, y, 0] = 1f; // weight;
            }
        }

        terrain.terrainData.SetAlphamaps(0, 0, alphaMap);

        Debug.Log("Generating detail map...");

        int detailMapWidth = terrain.terrainData.detailWidth;
        int layer = 0;

        int[,] detailMap = new int[detailMapWidth, detailMapWidth];

        for (int x = 0; x < detailMapWidth; x++)
        {
            for (int y = 0; y < detailMapWidth; y++)
            {
                float detail = (1 - detHarmonicFactor) * Mathf.PerlinNoise((detPerlinSeed.x + x) * detPerlinScale, (detPerlinSeed.y + y) * detPerlinScale);
                detail += (detHarmonicFactor) * Mathf.PerlinNoise((detPerlinSeed2.x + x) * detPerlinScale2, (detPerlinSeed2.y + y) * detPerlinScale2);
                if (detail < detPerlinThreshold || heightMap[x, y] * terrainSizeOverride.y < minAltitude || heightMap[x, y] * terrainSizeOverride.y > maxAltitude) detail = 0f;
                else detail = 1f;
                detailMap[x, y] = (int)(detailDensity * detail);
            }
        }

        terrain.terrainData.SetDetailLayer(0, 0, layer, detailMap);

        Debug.Log("Placing player...");

        float playerHeight = 10f + terrain.terrainData.GetHeight(heightMapWidth / 2, heightMapWidth / 2);
        Vector3 position = terrain.transform.position + new Vector3(0.5f * terrainSizeOverride.x, playerHeight, 0.5f * terrainSizeOverride.z);
        player.transform.position = position;

    }
}
