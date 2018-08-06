using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnergy : MonoBehaviour {

    public float fullEnergy = 50f;
    public float maxEnergy = 100f;

    public float energyRegen = 10f;

    public float currentEnergy;

    public CameraResolutionManager cameraResolution;

	// Use this for initialization
	void Start () {

        UpdateEnergy(fullEnergy);
		
	}
	
	// Update is called once per frame
	void Update () {

        AddEnergyDelta(energyRegen * Time.deltaTime);

        if (Input.GetKey(KeyCode.KeypadPlus) || Input.GetAxisRaw("Mouse ScrollWheel") > 0) UpdateFullEnergy(fullEnergy + 2);
        if (Input.GetKey(KeyCode.KeypadMinus) || Input.GetAxisRaw("Mouse ScrollWheel") < 0) UpdateFullEnergy(fullEnergy - 2);

    }

    public void AddEnergyDelta(float delta) {

        UpdateEnergy(currentEnergy + delta);
    }

    public void UpdateFullEnergy(float newValue) // rename full energy
    {

        fullEnergy = Mathf.Clamp(newValue, 10f, maxEnergy);

    }

    void UpdateEnergy(float newValue) {

        currentEnergy = Mathf.Clamp(newValue, 0f, fullEnergy);

        cameraResolution.UpdateResolutionNormalized(currentEnergy / maxEnergy);

    }
}
