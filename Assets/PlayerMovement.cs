using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public float forwardSpeed = 10f;
    public float lateralSpeed = 10f;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        float dt = Time.deltaTime;

        float forwardMovement = Input.GetAxis("Vertical");
        float lateralMovement = Input.GetAxis("Horizontal");

        Vector3 forwardDirection = Vector3.forward;
        Vector3 rightDirection = Vector3.right;

        transform.position += forwardMovement * forwardDirection * forwardSpeed * dt;
        transform.position += lateralMovement * rightDirection * lateralSpeed * dt;

    }
}
