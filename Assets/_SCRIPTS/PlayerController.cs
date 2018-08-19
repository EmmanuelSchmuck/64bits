using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public UnityEngine.PostProcessing.PostProcessManager ppManager;
    public UnityEngine.Audio.AudioMixerGroup audioMaster;
    public UnityEngine.Audio.AudioMixerGroup audioUnderwater;
    public AudioGroup audioRegular, audioAlternate;
    public ParticleSystem clouds;
    public ParticleSystem snow;

    public int minCloudDensity;
    public int maxCloudDensity;
    public float cloudHeightMinDensity;
    public float cloudHeightMaxDensity;


    public Transform water;
    public Transform camera;

    public Color waterFogColor;
    private bool underwater;

    // Use this for initialization
    void Start()
    {

        //baseFogColor = RenderSettings.fogColor;

    }

    void Update()
    {
        float waterHeight = water.position.y;
        float cameraHeight = camera.position.y - waterHeight; // aboveWater
        int cloudDensity = (int)Mathf.Lerp(minCloudDensity, maxCloudDensity, map(cameraHeight, cloudHeightMinDensity, cloudHeightMaxDensity, 0, 1));
        clouds.maxParticles = cloudDensity;

        if (cameraHeight < cloudHeightMinDensity && !clouds.isStopped)
        {
            clouds.Stop();
        }
        else if (cameraHeight > cloudHeightMinDensity && clouds.isStopped)
        {
            clouds.Play();
        }
        if (cameraHeight < 0 && !snow.isStopped)
        {
            snow.Stop();
        }
        else if (cameraHeight > 0 && snow.isStopped)
        {
            snow.Play();
        }
        if (cameraHeight < 0 && !underwater)
        {
            underwater = true;
            ppManager.OnEnteringWater();
            //RenderSettings.fogColor = waterFogColor;
            audioRegular.SetAudioMixerGroup(audioUnderwater);
            audioAlternate.SetAudioMixerGroup(audioUnderwater);
        }
        else if (cameraHeight > 0 && underwater)
        {
            underwater = false;
            ppManager.OnExitingWater();
            //RenderSettings.fogColor = WorldGenerator.Instance.currentFogColor;
            audioRegular.SetAudioMixerGroup(audioMaster);
            audioAlternate.SetAudioMixerGroup(audioMaster);
        }

        //debug :

        if(Input.GetKeyDown(KeyCode.U)) transform.position += Vector3.up * 100;
    }

    private float map(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Bounds") || col.CompareTag("NPC"))
        {
            GameController.Instance.GenerateNewRandomWorld();
        }
    }
}
