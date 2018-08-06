using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraResolutionManager : MonoBehaviour
{
    public UnityEngine.UI.RawImage screenRawImage;
    private new Camera camera;

    public int maxSize = 64;
    public int minSize = 16;
    public int currentSize = 64;
    public int sizeDelta = 4;

    private void Awake()
    {
        camera = GetComponent<Camera>();
    }

    // Use this for initialization
    void Start()
    {
        UpdateResolution(currentSize);
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKey(KeyCode.KeypadPlus)|| Input.GetAxisRaw("Mouse ScrollWheel")>0) UpdateResolution(currentSize + sizeDelta);
        //if (Input.GetKey(KeyCode.KeypadMinus) || Input.GetAxisRaw("Mouse ScrollWheel") < 0) UpdateResolution(currentSize - sizeDelta);
    }

    public void UpdateResolutionNormalized(float normalizedSize) {
        UpdateResolution((int)Mathf.Lerp(minSize, maxSize, normalizedSize));
    }

    public void UpdateResolution(int newSize)
    {
        if (camera.targetTexture != null)
        {
            camera.targetTexture.Release();
        }

        currentSize = Mathf.Clamp(newSize, minSize, maxSize);

        //RenderTextureDescriptor desc = new RenderTextureDescriptor(currentSize, currentSize);
        //desc.msaaSamples = 1;

        RenderTexture tex = new RenderTexture(currentSize, currentSize, 32);
        tex.filterMode = FilterMode.Point;
		tex.depth = 32;
		
        tex.antiAliasing = 1;

        camera.targetTexture = tex;
        screenRawImage.texture = tex;

    }
}
