﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.PostProcessing;
using System.Linq;

[ExecuteInEditMode]
public class WorldGenerator : MonoBehaviour
{
    public Transform instancedObjects;

    public Material sky;
    public GameObject temple;

    public Transform playerSpawn;

    public Light sunLight;
    public GameObject water;
    public Terrain terrain;

    public PostProcessingProfile waterPP;
    public PostProcessingProfile mainPP;


    public NPC_Navigation[] NPCs;

    [Header("Sky")]

    public float skySaturation;
    public float skyValue;


    [Header("Ruins")]
    public GameObject[] ruinsPrefabs;
    public int[] ruinAmounts;

    public float minDistanceNormalized;
    public float maxDistanceNormalized;

    [Header("Particles")]

    public ParticleSystem snow;
    public int[] snowDensities;


    [Header("Light")]
    public float lightAngleOrigin;
    public float lightAngleAmplitude;


    [Header("Colors")]

    public float hueShiftAmplitude;
    public float temperatureAmplitude;
    public float temperatureOrigin;
    public float tintAmplitude;
    public float saturationAmplitude;
    public float saturationOrigin;

    [Header("HeightMap")]

    public int terrainSizeXZ = 1000;
    public float baseAltitude = 100;
    public float altitudeAmplitude;
    public float waterHeightNormalized;
    public AnimationCurve[] radialHeightFactor;
    public float hmPerlinScaleOrigin = 1f;
    public float hmPerlinScaleAmplitude = 1f;
    public float hmPerlinScaleOrigin2 = 1f;
    public float hmPerlinScaleAmplitude2 = 1f;
    private Vector2Int hmPerlinSeed;
    private Vector2Int hmPerlinSeed2;
    public float hmHarmonicFactor = 0.5f;

    [Header("AlphaMap")]
    public float waterTextHeightOffset = 10;
    public float smoothingHeight = 20;
    public float snowAltitude = 100;

    [Header("DetailMap")]
    public int grassDensity = 1;
    public int alguaeDensity = 1;
    public float minAltitudeNormalized = 0;
    public float maxAltitudeNormalized = 100;
    public float detPerlinThreshold = 0.5f;
    public float detPerlinScale = 1f;
    public float detPerlinScale2 = 1f;
    private Vector2Int detPerlinSeed;
    private Vector2Int detPerlinSeed2;
    public float detHarmonicFactor = 0.5f;

    public bool generate;

    public bool moveTemple;
    public bool movePlayer;
    public bool moveWater;
    public bool animateCameraResolution;

    public bool modifyPP;

    public bool modifyLight;

    private AsyncOperation asyncop;

    private Vector3 terrainSizeOverride;

    public static WorldGenerator Instance;

    void Awake() => Instance = this;

    // Use this for initialization
    void Start()
    {
        StartGeneration(gameStart: true);
    }

    private void Update()
    {
        if (generate || Input.GetKeyDown(KeyCode.G)) StartGeneration(false); generate = false;
    }

    public void StartGeneration(bool gameStart = false)
    {
        StopAllCoroutines();

        StartCoroutine(Generate(gameStart));

        Debug.Log("Starting generation...");
    }

    public IEnumerator Generate(bool gameStart)
    {
        

        //instancedObjects = Instantiate(new GameObject("InstancedObjects"), this.transform).transform;

        GameObject player = GameObject.FindWithTag("Player");

        if (Application.isPlaying && animateCameraResolution)
        {
            Debug.Log("Triggering camera effect...");
            player.GetComponentInChildren<CameraResolutionManager>().TriggerEvolution(gameStart);
            if (!gameStart) yield return new WaitForSeconds(5f);

        }

        CleanUp();

        if (modifyPP)
        {
            float hueshift = hueShiftAmplitude * Random.Range(-1f, 1f);
            float saturation = saturationOrigin + saturationAmplitude * Random.Range(-1f, 1f);
            float temperature = temperatureOrigin + temperatureAmplitude * Random.Range(-1f, 1f);
            float tint = tintAmplitude * Random.Range(-1f, 1f);
            ColorGradingModel.Settings colorGradingWater = waterPP.colorGrading.settings;
            ColorGradingModel.Settings colorGradingMain = mainPP.colorGrading.settings;

            colorGradingWater.basic.hueShift = hueshift;
            colorGradingWater.basic.saturation = saturation;
            colorGradingWater.basic.temperature = temperature;
            colorGradingWater.basic.tint = tint;

            colorGradingMain.basic.hueShift = hueshift;
            colorGradingMain.basic.saturation = saturation;
            colorGradingMain.basic.temperature = temperature;
            colorGradingMain.basic.tint = tint;

            waterPP.colorGrading.settings = colorGradingWater;
            mainPP.colorGrading.settings = colorGradingMain;

        }

        if (modifyLight)
        {
            float azimuth = Random.Range(0f, 360f);
            float ascension = lightAngleOrigin + lightAngleAmplitude * Random.Range(-1f, 1f);
            sunLight.transform.rotation = Quaternion.Euler(ascension, azimuth, 0f);
        }

        Color skyColor = Random.ColorHSV(0f, 1f, skySaturation, skySaturation, skyValue, skyValue);
     
        sky.SetColor("_SkyTint", skyColor);


        hmPerlinSeed = new Vector2Int(Random.Range(0, 999), Random.Range(0, 999));
        hmPerlinSeed2 = new Vector2Int(Random.Range(0, 999), Random.Range(0, 999));
        detPerlinSeed = new Vector2Int(Random.Range(0, 999), Random.Range(0, 999));
        detPerlinSeed2 = new Vector2Int(Random.Range(0, 999), Random.Range(0, 999));

        int heightCurveIndex = Random.Range(0, radialHeightFactor.Length);


        Debug.Log("Generating heightmap...");

        int heightMapWidth = terrain.terrainData.heightmapWidth;
        terrainSizeOverride = new Vector3(terrainSizeXZ, baseAltitude, terrainSizeXZ);
        terrainSizeOverride.y += Random.Range(-1f, 1f) * altitudeAmplitude;
        terrain.terrainData.size = terrainSizeOverride;
        Vector3 terrainCenterWorldPos = terrain.transform.position + new Vector3(0.5f * terrainSizeOverride.x, 0f, 0.5f * terrainSizeOverride.z);

        Vector2Int heightMapCenter = new Vector2Int(heightMapWidth / 2, heightMapWidth / 2);

        float[,] heightMap = new float[heightMapWidth, heightMapWidth];

        float hmPerlinScale = hmPerlinScaleOrigin + Random.Range(-1f, 1f) * hmPerlinScaleAmplitude;
        float hmPerlinScale2 = hmPerlinScaleOrigin2 + Random.Range(-1f, 1f) * hmPerlinScaleAmplitude2;

        for (int x = 0; x < heightMapWidth; x++)
        {
            for (int y = 0; y < heightMapWidth; y++)
            {
                float distanceToCenter = Vector2Int.Distance(heightMapCenter, new Vector2Int(x, y)) / (heightMapWidth / 2);
                float normalizedHeight = (1 - hmHarmonicFactor) * Mathf.PerlinNoise((hmPerlinSeed.x + x) * hmPerlinScale, (hmPerlinSeed.y + y) * hmPerlinScale);
                normalizedHeight += (hmHarmonicFactor) * Mathf.PerlinNoise((hmPerlinSeed2.x + x) * hmPerlinScale2, (hmPerlinSeed2.y + y) * hmPerlinScale2);
                heightMap[x, y] = normalizedHeight * radialHeightFactor[heightCurveIndex].Evaluate(distanceToCenter);
            }
        }

        terrain.terrainData.SetHeights(0, 0, heightMap);

        Debug.Log("Generating alpha map...");

        int alphaMapWidth = terrain.terrainData.alphamapWidth;
        int alphaTextureCount = 3;

        float[,,] alphaMap = new float[alphaMapWidth, alphaMapWidth, alphaTextureCount];
        float waterTexHeight = waterHeightNormalized * terrainSizeOverride.y + waterTextHeightOffset;
        float baseTextureHeight = waterTexHeight + smoothingHeight;

        for (int x = 0; x < alphaMapWidth; x++)
        {
            for (int y = 0; y < alphaMapWidth; y++)
            {
                float height = heightMap[x, y] * terrainSizeOverride.y;
                if (height > snowAltitude)
                {
                    alphaMap[x, y, 0] = 0;
                    alphaMap[x, y, 1] = 0;
                    alphaMap[x, y, 2] = 1;
                }
                else
                {
                    alphaMap[x, y, 2] = 0;

                    if (height < waterTexHeight)
                    {
                        alphaMap[x, y, 0] = 0;
                        alphaMap[x, y, 1] = 1;
                    }
                    else if (height > baseTextureHeight)
                    {
                        alphaMap[x, y, 0] = 1;
                        alphaMap[x, y, 1] = 0;
                    }
                    else
                    {
                        alphaMap[x, y, 0] = (height - waterTexHeight) / smoothingHeight;
                        alphaMap[x, y, 1] = 1f - (height - waterTexHeight) / smoothingHeight;
                    }
                }
            }
        }

        terrain.terrainData.SetAlphamaps(0, 0, alphaMap);

        Debug.Log("Generating detail map...");

        int detailMapWidth = terrain.terrainData.detailWidth;

        int[,] detailMap0 = new int[detailMapWidth, detailMapWidth];
        int[,] detailMap1 = new int[detailMapWidth, detailMapWidth];

        for (int x = 0; x < detailMapWidth; x++)
        {
            for (int y = 0; y < detailMapWidth; y++)
            {
                float detail = (1 - detHarmonicFactor) * Mathf.PerlinNoise((detPerlinSeed.x + x) * detPerlinScale, (detPerlinSeed.y + y) * detPerlinScale);
                detail += (detHarmonicFactor) * Mathf.PerlinNoise((detPerlinSeed2.x + x) * detPerlinScale2, (detPerlinSeed2.y + y) * detPerlinScale2);
                if (detail < detPerlinThreshold || heightMap[x, y] < minAltitudeNormalized || heightMap[x, y] > maxAltitudeNormalized) detail = 0f;
                else detail = 1f;
                if (heightMap[x, y] > waterHeightNormalized)
                {
                    detailMap0[x, y] = (int)(grassDensity * detail);
                }
                else
                {
                    detailMap1[x, y] = (int)(alguaeDensity * detail);
                }
            }
        }

        terrain.terrainData.SetDetailLayer(0, 0, 0, detailMap0);
        terrain.terrainData.SetDetailLayer(0, 0, 1, detailMap1);

        if (moveTemple)
        {
            Debug.Log("Placing temple...");

            float templeHeight = 0f + terrain.terrainData.GetHeight(heightMapWidth / 2, heightMapWidth / 2);
            Vector3 position = terrainCenterWorldPos + Vector3.up * templeHeight;
            temple.transform.position = position;
        }

        if (movePlayer)
        {
            Debug.Log("Placing player...");

            player.transform.position = playerSpawn.position;
        }

        if (moveWater)
        {
            Debug.Log("Placing water...");

            float waterHeight = waterHeightNormalized * terrainSizeOverride.y;
            Vector3 position = terrainCenterWorldPos + Vector3.up * waterHeight;
            water.transform.position = position;
        }

        int snowDensityIndex = Random.Range(0, snowDensities.Length);
        snow.maxParticles = snowDensities[snowDensityIndex];

        // ruins ============================

        Debug.Log("Placing ruins...");

        int ruinAmount = ruinAmounts[Random.Range(0, ruinAmounts.Length)];

        GameObject ruinPrebab = ruinsPrefabs[Random.Range(0, ruinsPrefabs.Length)];

        for (int i = 0; i < ruinAmount; i++)
        {
            float azimuth = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float distance = Mathf.Lerp(minDistanceNormalized, maxDistanceNormalized, (float)i / (ruinAmount - 1));
            Vector3 position = new Vector3(distance * Mathf.Cos(azimuth), 0f, distance * Mathf.Sin(azimuth));
            //position.y = terrain.terrainData.GetHeight((int)position.x, (int)position.z);
            position.x = position.x * terrainSizeOverride.x * 0.5f + terrainCenterWorldPos.x;
            position.z = position.z * terrainSizeOverride.z * 0.5f + terrainCenterWorldPos.z;
            position.y = GetWorldYAtWorldXZ(position.x, position.z);
            Quaternion rotation = Quaternion.Euler(0f, Random.Range(0, 360), 0f);
            GameObject ruin = Instantiate(ruinPrebab, position, rotation, instancedObjects);
        }

        for (int i = 0; i < NPCs.Length; i++)
        {
            float azimuth = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float distance = Mathf.Lerp(minDistanceNormalized, maxDistanceNormalized, (float)i / ( NPCs.Length - 1));
            Vector3 position = new Vector3(distance * Mathf.Cos(azimuth), 0f, distance * Mathf.Sin(azimuth));
            //position.y = terrain.terrainData.GetHeight((int)position.x, (int)position.z);
            position.x = position.x * terrainSizeOverride.x * 0.5f + terrainCenterWorldPos.x;
            position.z = position.z * terrainSizeOverride.z * 0.5f + terrainCenterWorldPos.z;
            position.y = GetWorldYAtWorldXZ(position.x, position.z);
            //Quaternion rotation = Quaternion.Euler(0f, Random.Range(0, 360), 0f);
            //GameObject ruin = Instantiate(ruinPrebab, position, rotation, instancedObjects);
            NPCs[i].transform.position = position;
        }


        StartCoroutine(BuildNavmesh(terrain.GetComponent<NavMeshSurface>()));

    }

    // called by startcoroutine whenever you want to build the navmesh
    IEnumerator BuildNavmesh(NavMeshSurface surface)
    {

        foreach (NPC_Navigation npc in NPCs)
        {
            npc.gameObject.SetActive(false);
        }


        // get the data for the surface
        var data = InitializeBakeData(surface);
        surface.RemoveData();
        surface.navMeshData = null;

        Application.backgroundLoadingPriority = ThreadPriority.Low;

        // start building the navmesh
        asyncop = surface.UpdateNavMesh(data);

        // wait until the navmesh has finished baking
        yield return asyncop;

        Debug.Log("finished");

        // you need to save the baked data back into the surface
        surface.navMeshData = data;

        // call AddData() to finalize it
        surface.AddData();

        foreach (NPC_Navigation npc in NPCs)
        {
            npc.gameObject.SetActive(true);
            npc.AdjustPosition();
            if (Application.isPlaying) npc.SetNewRandomTarget();

        }
    }

    // creates the navmesh data
    static NavMeshData InitializeBakeData(NavMeshSurface surface)
    {
        var emptySources = new List<NavMeshBuildSource>();
        var emptyBounds = new Bounds();
        Debug.Log(surface);
        Debug.Log(emptySources);
        Debug.Log(emptyBounds);

        return NavMeshBuilder.BuildNavMeshData(surface.GetBuildSettings(), emptySources, emptyBounds, surface.transform.position, surface.transform.rotation);
    }

    void CleanUp()
    {
        int amountToDestroy = instancedObjects.childCount;
        GameObject[] toDestroy = new GameObject[amountToDestroy];
        for (int i = 0; i < amountToDestroy; i++)
        {
            toDestroy[i] = instancedObjects.GetChild(i).gameObject;
        }
        for (int i = 0; i < amountToDestroy; i++)
        {
            GameObject obj = toDestroy[i];
            if (Application.isPlaying)
            {
                Destroy(obj);
            }
            else
            {
                DestroyImmediate(obj);
            }
        }

    }


    void OnDisable()
    {
        //CleanUp();
        StopAllCoroutines();
    }

    void OnApplicationQuit()
    {
        //CleanUp();
        StopAllCoroutines();
    }

    float GetWorldYAtWorldXZ(float x, float z)
    {
        float height = 0f;
        Debug.Log("x - terrain.transform.position.x : " + (x - terrain.transform.position.x));
        int hmRes = terrain.terrainData.heightmapResolution;
        int hmX = hmRes * (int)(x - terrain.transform.position.x) / terrainSizeXZ; Debug.Log("hmX : " + hmX);
        int hmY = hmRes * (int)(z - terrain.transform.position.z) / terrainSizeXZ;
        if (hmX >= hmRes || hmX < 0 || hmY >= hmRes || hmY < 0) return 0;
        height = terrain.terrainData.GetHeight(hmX, hmY) + terrain.transform.position.y;
        return height;
    }
}