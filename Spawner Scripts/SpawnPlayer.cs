using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SpawnPlayer : MonoBehaviour
{
	public static int activeButton;

	public GameObject[] player; // list of units prefabs
	public GameObject[] playerNew; // new list of units prefab that is arranged regarded to the order of spawn button
	public Button[] spawners; // spawning buttons

	public GameObject manager;

	GameState gm;
	UIManagers uiM;

	bool tutorialElementsCalled = false;

	void Start () 
	{
		gm = manager.GetComponent<GameState>();
		uiM = manager.GetComponent<UIManagers>();
		CanInstantiate(true);
	}

	// this method is called by the spawn button
	public void SpawnObject(int num)
	{
		// in case if the user decided to spawn different unit the previous on hold unit will be destroyed
		if(gm.unitOnHold != null)
		{
			gm.DestroyUnitOnHold();
		}

		if(gm.canInsantiate && GameState.money >= GameManager.unitCost[playerNew[num].GetComponent<UnitScript>().ReturnID()])
		{
			//Instantiate(player[num] , Input.mousePosition , Quaternion.identity);
			GameObject unit =  (GameObject)Instantiate(playerNew[num] , Input.mousePosition , Quaternion.identity);
			uiM.HighlightButton(num);
			gm.unitOnHold = unit; // this line is needed
			gm.canInsantiate = false;
			activeButton = num;

			if(GameState.tutorial == true && tutorialElementsCalled == false)
			{
				tutorialElementsCalled = true;
				TutorialScript ts = GameObject.Find("Canvas/Tutorial").GetComponent<TutorialScript>();
				ts.DisableLastPanel();
				ts.GridIndicatorSetting(true);
				GameObject.Find("ObjectSpawner").GetComponent<EnemySpawner>().StartWave();
			}
		}
	}

    // this function is called from each spawned units own class from preventing to spawn same units while previous 
    //is not positioned
	public bool CanInstantiate(bool b)
	{
		gm.canInsantiate = b;
		return gm.canInsantiate;
	}
}
