using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldUtility : MonoBehaviour {

	public int min;
	public int max;

	public InputField inputField;

	public KeyCode randomNumberKey;
	
	private void Update(){

		if(Input.GetKeyDown(randomNumberKey) && inputField.enabled) RandomValue(); 

	}

	public void ClampValue(){

		if (inputField.text == "") return;

		int value = int.Parse(inputField.text);
		value = (int)Mathf.Clamp(value, min, max);
		inputField.text = value.ToString();

	} 

	private void RandomValue(){

		int value = Random.Range(min, max);
		inputField.text = value.ToString();

	}
}
