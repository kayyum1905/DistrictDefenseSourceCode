using UnityEngine;
using System.Collections;
using System.IO;

public class GameState : MonoBehaviour 
{
	public static int money;
	public static bool tutorial;

	// in case you want to place the unit directly without clicking but by clicking the grid
	// why starts with false, because initially you must use spawn button
	public static bool[] canPlace = {false, false, false, false, false, false, false, false};

	public bool[] enemyPresentY; // Enem. Units updates the specific bool to report their y pos
	public bool[] unitPresentY;  // Frien. Units updates the specific bool to report their y pos
	public bool[] gridAvailable = new bool[40];

	public int currentLevel;
	public bool canInsantiate; // player can spawn new unit

	// the spawned object will be spawned here, in case it is not positioned and new one is spawned this one will be destroyed
	public GameObject unitOnHold;

	public GameObject spawner;

	GameManager gameManager;
	SpawnPlayer sp;
	//UIManagers uiManager;

	string tutorialDone = "Tutorial Done";

	void Awake ()
	{
		//this is to test faster
		//Time.timeScale = 3f;

		int[] initMoneyArray = {400, 300, 300, 300, 325, 350, 350, 375, 400, 400,400,400,400, 400,400,400,400};

		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
		//uiManager = GameObject.Find("Managers").GetComponent<UIManagers>();
		currentLevel = gameManager.GetCurrentLevel(); // this is the default value
		//currentLevel = 16; // ONLY FOR TEST!
		money = initMoneyArray[currentLevel] + 25;
		//money = 10000; // for testing purposes

		int tutState = PlayerPrefs.GetInt(tutorialDone); // 0: not done (default value), 1: done

		if(currentLevel == 1 && tutState == 0)
		{
			tutorial = true;
		}
		else
		{
			tutorial = false;
			Destroy(GameObject.Find("Canvas/Tutorial"));
		}

		for (int i = 0; i < gridAvailable.Length; i++) 
		{
			gridAvailable[i] = true;
		}
	}

	void Start()
	{
		sp = spawner.GetComponent<SpawnPlayer>();
	}

	void Update()
	{
		if(unitOnHold == null && canInsantiate == false)
		{
			canInsantiate = true;
		}
	}

	// called by tutorial script to indicate player finished tutorial
	public void TutorialDone()
	{
		tutorial = false;
		PlayerPrefs.SetInt(tutorialDone, 1);	
	}

	// this func is called when the level is successfully finished
	public void UpdateLevel()
	{
		currentLevel++;

		// make sure it is not bigger then the available levels
		if(currentLevel > 16)
			currentLevel = 16; // update this when you add new level

		gameManager.SetReachedLevel(currentLevel);
	}

	//to destroy the object on hold if some other action is taken
	public void DestroyUnitOnHold()
	{
		Destroy(unitOnHold);
		canInsantiate = true;
	}

	//to place the unit if in case user touches the grids
	public void PlaceUnit(int buttonID)
	{
		if(gridAvailable[buttonID] == false || canInsantiate == false)
			return;

		if(unitOnHold == null && canPlace[SpawnPlayer.activeButton] == true)
		{
			sp.SpawnObject(SpawnPlayer.activeButton);
			unitOnHold.GetComponent<UnitScript>().PositionUnitOnGrid(buttonID);
			unitOnHold = null;
			return;
		}
		else if(unitOnHold == null && canPlace[SpawnPlayer.activeButton] == false)
			return;

		float[] xPos = {-5f, -3.5f, -2f, -.5f, 1f, 2.5f, 4f, 5.5f};
		float[] yPos = {.7f, -.3f, -1.3f, -2.3f, -3.3f};

		Vector2 pos =  new Vector2(xPos[buttonID / 5], yPos[buttonID % 5]);
		int[] sorting = {1, 2, 3, 4, 5};
		unitOnHold.transform.position = pos;

		UnitScript u = unitOnHold.GetComponent<UnitScript>();

		//us.positioned = true;
		u.pay = true;
		u.positionedByGrid = true;
		u.GetComponent<SpriteRenderer>().sortingOrder = sorting[buttonID % 5];
		u.PrePositioningSettings();
		u.gridID = buttonID;
		unitOnHold.SetActive(true);
		gridAvailable[buttonID] = false;

		unitOnHold = null;
	}

	//*** The method below are called by Units and Enemy units ***

	// This method is called when enemy unit enters the battle field or when it is going to die
	public void EnemyStatePos (int pos, bool present) 
	{
		enemyPresentY[pos] = present;
	}

	// This is called by the unit script when positioned or enemy unit is in the same row
	public bool PermitUnitToShoot(int y)
	{
		return enemyPresentY[y];
	}

	// when unit is positioned this is called or when is going to die
	public void UnitStatePos (int pos, bool present) 
	{
		unitPresentY[pos] = present;
	}

	// when enemy enters or a unit is spawned to the same row
	public bool PermitEnemyToShoot(int y)
	{
		return unitPresentY[y];
	}

	//*** The method above are called by Units and Enemy units ***
}
