using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DancingLight : MonoBehaviour
{

    public float rotationSpeed;
    public float rotationRadius;
    private Vector3 originalPosition;

    // Use this for initialization
    void Start()
    {
        originalPosition = transform.position;
        transform.position += Vector3.right * rotationRadius;
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(originalPosition, Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
