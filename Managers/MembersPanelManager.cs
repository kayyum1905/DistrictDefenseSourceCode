using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MembersPanelManager : MonoBehaviour 
{
	public Button[] buttons; // the select your character buttons on the panel
	public Sprite[] buttonSprites; // sprites that are going to be attached to the spawn unit button's image

	public GameObject panel; // the panel its self
	public GameObject playButton;
	public GameObject multiTaskPanel; //the panel back (multi task panel)
	public GameObject manager; // the manager object that contains: ui manager, game state etc...
	public GameObject objectSpawner; // the object that spawns both player units and enemy units

	public Text description;

	SpawnPlayer sp;
	UIManagers uiManager;
	GameState gameState;
	//GameManager gm;

	int quota; // the array of available units
	int limit; // how many members you can select
	int queue = -1; // to measure how many units you have selected

	void Awake()
	{

	}

	void Start()
	{
		// delete later
		//SetPlayButton(true);
		
		gameState = manager.GetComponent<GameState>();
	//	gm = manager.GetComponent<GameManager>();

		// array for team size set by current level to set quota
		int[] teamSize = {9, 2, 4, 6, 7, 8, 8, 9, 9, 9,9,9,9, 9,9,10,10}; // offer
		int[] selectable = {8, 2, 3, 4, 5, 6, 7, 8, 8, 8,8,8,8,8,8,8,8}; // your team

		quota = teamSize[gameState.currentLevel];
		limit = selectable[gameState.currentLevel];

		// this is to disable all select actor buttons before enabling the right ones, ristricting some units
		for (int i = quota; i < buttons.Length; i++) 
		{
			buttons[i].GetComponent<Image>().color = new Color32(70,70,70,255);
			buttons[i].GetComponent<Button>().enabled = false;
		}

		sp = objectSpawner.GetComponent<SpawnPlayer>();

		uiManager = manager.GetComponent<UIManagers>();
	}

	void SetPlayButton(bool myBool)
	{
		playButton.GetComponent<Button>().interactable = myBool;
	}

	// on members panel when clicked on character this function is called
	public void ProcessButton(int index)
	{
		if(index <= quota) // if the unit selected is in range of selectable chaeracters
		{
			buttons[index].GetComponent<Image>().color = new Color32(110,110,110,255);
			buttons[index].GetComponent<Button>().enabled = false;
			queue++;

			sp.spawners[queue].GetComponent<Image>().gameObject.SetActive(true);
			sp.spawners[queue].GetComponent<Image>().sprite = buttonSprites[index];
			AssignPrefab(index);

			// set the price text on spawning button
			uiManager.SetText(queue, GameManager.unitCost[index]);

			DescriptionUpdate(index);

		} // codes below deactivates the button but if the queue is mixed unexpected buttons gets disabled
//		else
//		{
//			buttons[index].GetComponent<Image>().color = new Color(1,1,1);
//			buttonActive[index] = true;
//			sp.spawners[queue].GetComponent<Image>().gameObject.SetActive(false);
//			queue--;
//		}

		// ***** when reached limit disable all selectable buttons *****
		if(queue == limit - 1)
		{
			SetPlayButton(true);
			for (int i = 0; i < quota; i++) 
			{
				buttons[i].GetComponent<Image>().color = new Color32(110,110,110,255);
				buttons[i].GetComponent<Button>().enabled = false;
			}
		}

	}

	// The description panel
	void DescriptionUpdate(int i)
	{
		string[] h = {"++", "++", "+++", "+++", "+", "", "+++", "", "+","++++"};
		string[] s = {"Defender", "shooter", "Shotgun", "Don the Shooter", "Instant Killer", "Mine Bomb","Toxic Hazard",  
				      "Thief", "Freezer","Big Mama"};

		string d = "ID: " + i.ToString() + "\nPrice: " + GameManager.unitCost[i].ToString() + "\nHealth: " + h[i] + "\nSpec: \n" + s[i];
		description.text = d;
	}

	// code below will be called from the reverse button on member selection panel
	// this will give the player opportubity to reset the selection
	public void ReverseSelection() // RESET (for now)
	{
		for (int i = 0; i < quota; i++) // reseting buttons to initial state
		{
			buttons[i].GetComponent<Image>().color = new Color(1,1,1);
			buttons[i].GetComponent<Button>().enabled = true;

		}

		for (int i = 0; i < sp.spawners.Length; i++) 
		{
			sp.spawners[i].GetComponent<Image>().gameObject.SetActive(false);	
		}

		queue = -1;
		SetPlayButton(false);

		//RESET DECRİPTİOP	TEXT
		string d = "ID: 0" + "\nPrice: 0" + "\nHealth: ";
		description.text = d;
	}

	void AssignPrefab(int q)
	{
		sp.playerNew[queue] = sp.player[q]; //queue is the procces state, regarding the buttons will be activated

		//every member will store the queue via buttonlocation var, look ->unitscript for further explanation
		sp.playerNew[queue].GetComponent<UnitScript>().buttonLocation = queue; 
	}

	// when you are done choosing your team members, the start button will call this
	public void ClosePanel()
	{
		multiTaskPanel.SetActive(false); //close the bacxkground dim panel
		panel.SetActive(false); // close the very panel it self

		// Get tribute function from ui manager will be called
		Invoke("StartTributeButton", 12f);

		//Destroy this object
		Invoke("DestroyThisGameObject", 12.5f);

		if(GameState.tutorial == false)
		{
			objectSpawner.GetComponent<EnemySpawner>().StartWave(); // start the enemy wave
		}
	}

	// destroy the whole members panel for memory purpose
	void DestroyThisGameObject()
	{
		Destroy(this.gameObject);
	}

	void StartTributeButton()
	{
		uiManager.ActivateAskTributeButton();
	}
}
