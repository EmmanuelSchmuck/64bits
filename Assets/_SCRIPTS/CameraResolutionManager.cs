using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraResolutionManager : MonoBehaviour
{
    public UnityEngine.UI.RawImage screenRawImage;
    private new Camera camera;
    public AudioGroup audioRegular, audioAlternate;

    public Vector2Int aspectRatio;

    public int maxSize = 64;
    public int minSize = 16;
    public int currentSize = 64;
    public int sizeDelta = 4;

    [Header("Time Evolution")]
    public bool evolveWithTime;
    public float refreshTimer;
    public bool randomResolution;
    public float minRandom, maxRandom;
    public AnimationCurve normalizedResolutionOverTime;
    public AnimationCurve normalizedResolutionOverTime_GameStart;
    public float cycleDuration;
    public bool loop;

    private void Awake()
    {
        camera = GetComponent<Camera>();
    }

    // Use this for initialization
    void Start()
    {
        //UpdateResolution(currentSize);
        //if (evolveWithTime) TriggerEvolution(true);
    }

    public void TriggerEvolution(bool gameStart)
    {
        StopAllCoroutines();
        StartCoroutine(EvolveWithTime(gameStart));
    }

    public IEnumerator EvolveWithTime(bool gameStart)
    {

        float timer = 0f;
        float lastTime = Time.time;
        bool evolve = true;
        while (evolveWithTime && evolve)
        {

            timer += Time.time - lastTime;
            lastTime = Time.time;
            if (timer > cycleDuration)
            {
                if (loop) timer = 0f;
                else evolve = false;
            }
            float resolutionNormalized = gameStart ?
                normalizedResolutionOverTime_GameStart.Evaluate(timer / cycleDuration)
                : normalizedResolutionOverTime.Evaluate(timer / cycleDuration);

            UpdateResolutionNormalized(resolutionNormalized);

            yield return new WaitForSeconds(refreshTimer);
        }
        UpdateResolution(maxSize);
    }

    public void UpdateResolutionNormalized(float normalizedSize)
    {
        UpdateResolution((int)Mathf.Lerp(minSize, maxSize, normalizedSize));
    }

    public void UpdateResolution(int newSize)
    {
        if (camera.targetTexture != null)
        {
            camera.targetTexture.Release();
        }

        currentSize = Mathf.Clamp(newSize, minSize, maxSize);

        //audio
        //audioMixer.SetFloat("DistortionValue", audioDistortCurve.Evaluate((float)currentSize/maxSize));
        audioRegular.SetVolume((float)currentSize / maxSize);
        audioAlternate.SetVolume(1f - (float)currentSize / maxSize);



        int biggest = aspectRatio.x > aspectRatio.y ? aspectRatio.x : aspectRatio.y;
        int width = (int)(currentSize * (float)aspectRatio.x / biggest);
        int height = (int)(currentSize * (float)aspectRatio.y / biggest);

        RenderTexture tex = new RenderTexture(width, height, 32);
        tex.filterMode = FilterMode.Point;
        tex.depth = 32;

        tex.antiAliasing = 1;

        camera.targetTexture = tex;
        screenRawImage.texture = tex;

    }
}
