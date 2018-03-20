using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class UIManagers : MonoBehaviour 
{
	public Text[] priceTexts;
	public GameObject[] imagesToBeHighlightid;

	//whenever you spawn a gang member, there will be a countdown a period between spawning same member
	//list contains of 8 element
	public GameObject[] countDownText;

	//friendly characters and enemies are spawned via the scripts under this object
	public GameObject objectSpawner;

	public GameObject askTributeButton;
	public GameObject manager;

	//when started new level
	public GameObject levelPanel;
	public Text levelNo;

	public Text moneyText;

	GameState gm;

	void Start () 
	{
		gm = manager.GetComponent<GameState>();

		int s = 0;
		int l = 0;

		if(gm.currentLevel == 0)
		{
			levelNo.text = "Survival Mode";
			Invoke("DisableLevelPanel", 1f);
			return;
		}

		if(gm.currentLevel > 0 && gm.currentLevel <= 4)
		{
			s = 1;
			l = gm.currentLevel;
		}
		else if(gm.currentLevel > 4 && gm.currentLevel <= 8)
		{
			s = 2;
			l = gm.currentLevel - 4;
		}
		else if(gm.currentLevel > 8 && gm.currentLevel <= 12)
		{
			s = 3;
			l = gm.currentLevel - 8;
		}
		else if(gm.currentLevel > 12 && gm.currentLevel <= 16)
		{
			s = 4;
			l = gm.currentLevel - 12;
		}

		string n = "LEVEL\n" + s.ToString() + " - " + l.ToString();
		levelNo.text = n;
		Invoke("DisableLevelPanel", 1f);

	}

	void Update ()
	{
		moneyText.text = "$: " + GameState.money.ToString();
	}

	void DisableLevelPanel()
	{
		Destroy(levelPanel);
	}

	// activate ask tibute button, called gamestate tribute method
	public void ActivateAskTributeButton()
	{
		askTributeButton.SetActive(true);
	}

	// highlight the button that was selected
	public void HighlightButton(int b)
	{
		for (int i = 0; i < imagesToBeHighlightid.Length; i++) 
		{
			if (b == i) 
			{
				imagesToBeHighlightid[i].SetActive(true);
			}
			else
				imagesToBeHighlightid[i].SetActive(false);
		}
	}

	// called by the ask tribute button
	public void AskTributeMethod()
	{
		int[] value = {50, 75, 100};
		int t = value[UnityEngine.Random.Range(0, value.Length)];
		GameState.money += t;
		print("Tribute given: " + t);
		askTributeButton.SetActive(false);
		Invoke("ActivateAskTributeButton", UnityEngine.Random.Range(18f,28f));
		//if any tribute is given method below must be called so the price text color is updated
		if(t > 0)
			HighlightPriceText();

		if(gm.currentLevel > 1)
			return;

		if(GameState.tutorial == true)
		{
			GameObject.Find("Canvas/Tutorial").GetComponent<TutorialScript>().GoToSpawnDescription();
		}
	}

	// this method is called by members panel
	public void SetText (int index, int price) 
	{
		priceTexts[index].text = price.ToString();
	}

	// this function is called by unit classes when they are spawned to de-activate the spawn button for a while
	public void ActivateText(int index) // activates the countdown text
	{
		countDownText[index].SetActive(true);
		GameState.canPlace[index] = false;

		Image spIMG = objectSpawner.GetComponent<SpawnPlayer>().spawners[index].GetComponent<Image>();

		spIMG.raycastTarget = false;
		spIMG.color = new Color32(110,110,110,255);

		StartCoroutine( TimeDecrease(8 , index, spIMG) );
	}

	// due to lack of money price texts will be highlighted
	// this function must be called after money payment (when money -= cost) or even when added
	public void HighlightPriceText()
	{
		for (int i = 0; i < priceTexts.Length; i++) 
		{
			System.String s = priceTexts[i].text.ToString();
			int x;

			try 
			{
				x = Convert.ToInt32(s);
			} 
			catch  
			{
				return;
			}

			if(GameState.money < x)
				priceTexts[i].GetComponent<Text>().color = new Color32(255,0,0,255);
			else if(GameState.money >= x)
				priceTexts[i].GetComponent<Text>().color = new Color32(255,255,255,255);
		}
	}

	// started by the function ActivateText
	IEnumerator TimeDecrease(int loop , int i, Image sp)
	{
		for(int l = loop ; l > 0 ; l --)
		{
			countDownText[i].GetComponent<Text>().text = l.ToString("#");
			yield return new WaitForSeconds(1f);

			if( l == 1 )
			{
				ActivateButton(i, sp);
				GameState.canPlace[i] = true;
			}
		}
	}

	// called by the couroutine TimeDecrease when coroutine is finished
	void ActivateButton(int buttonIndex, Image sp)
	{
		StopCoroutine(TimeDecrease(0,0,null));

		countDownText[buttonIndex].SetActive(false);
		sp.raycastTarget = true;
		sp.color = new Color(1,1,1);
	}
}
