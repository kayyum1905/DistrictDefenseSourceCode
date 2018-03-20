using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour 
{
	//district health level
	public static int health;

	//multitask panel will be used as a hit effect when an enemy enters your district
	public GameObject panel;

	//Object unique to each section, some will be disabled
	public GameObject[] objects;

	public GameObject pausePanel;
	public GameObject backGround;
	public GameObject managers;
	public GameObject areaObjectButton;

	//friendly characters and enemies are spawned via the scripts under this object
	public GameObject objectSpawner;

	// the three hearts that indicates the district health level
	public Image[] hearthIMG;

	//area object
	public GameObject areaObject;

	//gameover elements
	public GameObject goText, goPlay;

	// this is to make game harder if Friendly unit number is high, Edited by UnityScript
	public static int unitNo;

	GameState gm;

	void Awake()
	{
		// UnitNo is zero at thew start of every scene, it can remain equal to the number or previous level
		unitNo = 0;
	}

	void Start()
	{
		health = 3;

		gm = managers.GetComponent<GameState>();

		// Survival Mode Start
		if(gm.currentLevel == 0) // Survival Mode, below scripts randomly call one of the existing levels
		{
			int x = Random.Range(1,5);

			if(x == 1)
		 	{
				backGround.GetComponent<SpriteRenderer>().sprite = Resources.Load("level1", typeof(Sprite)) as Sprite;
				objects[0].SetActive(true);
				Destroy(objects[1]);
				Destroy(objects[2]);
				objects[3].SetActive(true);
		 	}
			else if(x == 2)
		 	{
				backGround.GetComponent<SpriteRenderer>().sprite = Resources.Load("level2", typeof(Sprite)) as Sprite;
				Destroy(objects[0]);
				objects[1].SetActive(true);
				Destroy(objects[2]);
				objects[3].SetActive(true);
		 	}
			else if(x == 3)
		 	{
				backGround.GetComponent<SpriteRenderer>().sprite = Resources.Load("level3", typeof(Sprite)) as Sprite;
				Destroy(objects[0]);
				Destroy(objects[1]);
				Destroy(objects[2]);
				objects[3].SetActive(true);
		 	}
			else if(x == 4)
		 	{
				backGround.GetComponent<SpriteRenderer>().sprite = Resources.Load("level4", typeof(Sprite)) as Sprite;
				Destroy(objects[0]);
				Destroy(objects[1]);
				objects[2].SetActive(true);
				Destroy(objects[3]);
		 	}
			ActiveAreaObjectButton();
			return;
		}
		//Survival End

		if(gm.currentLevel <= 4)
		{
			backGround.GetComponent<SpriteRenderer>().sprite = Resources.Load("level1", typeof(Sprite)) as Sprite;
			objects[0].SetActive(true);
			Destroy(objects[1]);
			Destroy(objects[2]);
		//	objects[1].SetActive(false);
		//	objects[2].SetActive(false);
			objects[3].SetActive(true);
		}
		else if(gm.currentLevel > 4 && gm.currentLevel <= 8)
		{
			backGround.GetComponent<SpriteRenderer>().sprite = Resources.Load("level2", typeof(Sprite)) as Sprite;
			Destroy(objects[0]);
			objects[1].SetActive(true);
			Destroy(objects[2]);
			objects[3].SetActive(true);
		}
		else if(gm.currentLevel > 8 && gm.currentLevel <= 12)
		{
			backGround.GetComponent<SpriteRenderer>().sprite = Resources.Load("level3", typeof(Sprite)) as Sprite;
			Destroy(objects[0]);
			Destroy(objects[1]);
			Destroy(objects[2]);
			objects[3].SetActive(true);
			ActiveAreaObjectButton();
		}
		else if(gm.currentLevel > 12)
		{
			backGround.GetComponent<SpriteRenderer>().sprite = Resources.Load("level4", typeof(Sprite)) as Sprite;
			Destroy(objects[0]);
			Destroy(objects[1]);
			objects[2].SetActive(true);
			Destroy(objects[3]);
			ActiveAreaObjectButton();
		}
	}

	public void SelectScene (int sceneNo) 
	{
		if(Time.timeScale ==0)
		{
			Time.timeScale = 1;
			PausePanel(false);
		}
		SceneManager.LoadScene( sceneNo , LoadSceneMode.Single);
	}

	public void PauseGame()
	{
		if(Time.timeScale !=0)
		{
			Time.timeScale = 0;
			PausePanel(true);
		}
		else
		{
			Time.timeScale = 1;
			PausePanel(false);
		}

		if(GameState.tutorial == true)
		{
			GameObject.Find("Canvas/Tutorial").GetComponent<TutorialScript>().GotoTributeDescription();
		}
	}

	public void DistrictHealth()
	{
		health -= 1;

		if(health < 0)
			return;

		StartCoroutine(DamageEffects());
	}

	// to call the area object (bomb all area): called by the buttons lower in canvas
	public void SpawnAreaObject()
	{
		if(GameState.money >= 500)
		{
			GameState.money -= 500;
			Instantiate(areaObject, new Vector2(0.1f, -1.5f), Quaternion.identity);
			areaObjectButton.SetActive(false);
			Invoke("ActiveAreaObjectButton", 8f);
		}
	}

	void ActiveAreaObjectButton()
	{
		areaObjectButton.SetActive(true);
	}

	void GameOver()
	{
		panel.SetActive(true);
		goText.SetActive(true);
		goPlay.SetActive(true);
		objectSpawner.GetComponent<EnemySpawner>().StopWave();
	}

	IEnumerator DamageEffects()
	{
		Image panelImage = panel.GetComponent<Image>();
		yield return new WaitForSeconds(1.3f);
		panel.SetActive(true);
		hearthIMG[health].enabled = false;
		panelImage.color = new Color32( 255, 0, 0,100 );
		yield return new WaitForSeconds(.05f);
		panel.SetActive(false);
		panelImage.color = new Color32( 0,0,0,100);

		if(health <= 0)
		{
			GameOver();
		}
	}

	void PausePanel(bool state)
	{
		pausePanel.gameObject.SetActive(state);
	}
}
