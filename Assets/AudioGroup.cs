using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioGroup : MonoBehaviour
{

    public AudioClip[] music1;
    public AudioClip[] music2;

    public AudioClip[] music3;

    public AudioClip[] music4;

    public AudioClip[] music5;

    public AudioClip[] music6;

    public AudioClip[] music7;

    public AudioSource[] sources;

    public float[] volumeFactors;

	public int maxPitchIndex = 8;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StopAll()
    {
        for (int i = 0; i < sources.Length; i++)
        {
            sources[i].clip = null;
            sources[i].Stop();
        }
    }

	public void SetVolume(float volume){

		  for (int i = 0; i < sources.Length; i++)
        {
            sources[i].volume = volume * volumeFactors[i];
        }

	}

    public void SetAudioMixerGroup(AudioMixerGroup group)
    {

        for (int i = 0; i < sources.Length; i++)
        {
            sources[i].outputAudioMixerGroup = group;
        }

    }

    public void SetupMusic(int musicIndex, int pitchIndex)
    {

        AudioSource source = sources[musicIndex];
        switch (musicIndex)
        {
            case 0: source.clip = music1[pitchIndex]; break;
            case 1: source.clip = music2[pitchIndex]; break;
            case 2: source.clip = music3[pitchIndex]; break;
            case 3: source.clip = music4[pitchIndex]; break;
            case 4: source.clip = music5[pitchIndex]; break;
            case 5: source.clip = music6[pitchIndex]; break;
            case 6: source.clip = music7[pitchIndex]; break;
        }

        source.Play();
        source.loop = true;
    }
}
