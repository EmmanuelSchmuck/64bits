using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateSphere : MonoBehaviour {

	public GameObject sphere;
	public float radius;
	public float speed;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		float time = Time.time;

		Vector3 localPosition = new Vector3(0,0,0);
		localPosition.x = radius * Mathf.Cos(time * speed);
		localPosition.z = radius * Mathf.Sin(time * speed);

		sphere.transform.localPosition = localPosition;

		
	}
}
