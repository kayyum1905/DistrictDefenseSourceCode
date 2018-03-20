using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TutorialScript : MonoBehaviour 
{
	public GameObject[] descriptions;

	string[] descriptionsTexts = {"The buttons here are to spawn friendly units \n Click here to read more...", 
								"You can do it in two ways, Click the button and drag to the grid or click the button and click to " + 
									"the grid you want to place your unit!"};

	
	void Start () 
	{
		if(GameState.tutorial == false)
		{
			for (int i = 0; i < descriptions.Length; i++) 
			{
				descriptions[i].SetActive(false);
			}
		}
		else if(GameState.tutorial == true)
		{
			descriptions[0].SetActive(true);
		}
	}

	// called from the description[0] element or pause game
	public void GotoTributeDescription()
	{
		GameObject.Find("Managers").GetComponent<UIManagers>().ActivateAskTributeButton();
		descriptions[0].SetActive(false);
		descriptions[1].SetActive(true);
	}

	// called from uimanager when clicked to get tribute
	public void GoToSpawnDescription()
	{
		descriptions[0].SetActive(false);
		descriptions[1].SetActive(false);
		descriptions[2].SetActive(true);

		SetDescriptionText(descriptionsTexts[0]);
	}

	// called by the description[2] element
	public void UpdateDesrciptionText()
	{
		SetDescriptionText(descriptionsTexts[1]);
	}

	void SetDescriptionText(string s="No Text")
	{
		descriptions[2].GetComponentInChildren<Text>().text = s;
	}

	// called when a spawn button is clicked
	public void DisableLastPanel()
	{
		descriptions[0].SetActive(false); // in case player directly clicked to spawn button
		descriptions[2].SetActive(false);
		//GameObject.Find("ObjectSpawner").GetComponent<EnemySpawner>().StartWave();
	}

	public void GridIndicatorSetting(bool b)
	{
		descriptions[3].SetActive(b);

		if(b == false)
		{
			GameObject.Find("Managers").GetComponent<GameState>().TutorialDone();
			Destroy(this.gameObject);
		}
	}
}
