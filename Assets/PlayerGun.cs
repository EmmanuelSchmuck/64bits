using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGun : MonoBehaviour {

    public GameObject bulletPrefab;

    public Transform bulletSpawn;

    public float bulletSpeed;

    public float fireDelay;

    public float energyCost;

    public float bulletLifeTime;

    public Vector3 offset;

    public PlayerEnergy playerEnergy;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetMouseButtonDown(0)) {

            InvokeRepeating("Shoot", 0f, fireDelay);

        }

        if (Input.GetMouseButtonUp(0))
        {

            CancelInvoke();

        }

    }

    void Shoot() {

        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
        Destroy(bullet, bulletLifeTime);
        bullet.GetComponent<Rigidbody>().AddForce(transform.forward * bulletSpeed);
        playerEnergy.AddEnergyDelta(-1f * energyCost);

    }
}
