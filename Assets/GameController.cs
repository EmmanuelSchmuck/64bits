using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameController : MonoBehaviour
{

	public GameObject player;
	public GameObject menu;
	public Text currentSeedText;
	public Text newSeedText;

	public InputField inputField;

	public KeyCode menuKey;
	public KeyCode exitKey;

	public int maxSeed = 999;

	private bool inMenu;

	private int currentSeed;

	public static GameController Instance;

	private void Awake() => Instance = this;

	// Use this for initialization
	void Start()
	{

		inputField.text = "";
		inputField.transform.GetChild(0).GetComponent<Text>().text = "";
		menu.SetActive(false);

		GenerateNewRandomWorld(true);



	}

	// Update is called once per frame
	void Update()
	{

		if (Input.GetKeyDown(menuKey))
		{
			if (inMenu) CloseMenu();
			else OpenMenu();
		}

			if (Input.GetKeyDown(exitKey))
		{
			Application.Quit();
		}

	}

	private void OpenMenu()
	{
		inMenu = true;

		menu.SetActive(true);
		//inputField.OnDeselect(new BaseEventData(EventSystem.current));
		//inputField.Select();
		inputField.ActivateInputField();
		//inputField.text = currentSeed.ToString();

		//Time.timeScale = 0f;
		//player.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = false;

		//newSeedText.text = currentSeed.ToString();
	}

	private void CloseMenu()
	{
		if (inputField.text != "")
		{
			int newSeed = int.Parse(inputField.text);
			inputField.text = "";
			if(newSeed != currentSeed) 	GenerateNewWorld(newSeed);
		
		}
		//inputField.
		
		inMenu = false;
		menu.SetActive(false);
		//Time.timeScale = 1f;
		//player.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = true;
	}

	private void GenerateNewWorld(int seed, bool gameStart = false)
	{
		currentSeed = seed;
		currentSeedText.text = "Island " + currentSeed.ToString();
		WorldGenerator.Instance.StartGeneration(currentSeed, gameStart);

	}

	public void GenerateNewRandomWorld(bool gameStart = false) => GenerateNewWorld(Random.Range(0, maxSeed), gameStart);


	//private void OpenMenu;
}
