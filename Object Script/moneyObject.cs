using UnityEngine;
using System.Collections;

public class moneyObject : MonoBehaviour 
{
	SpriteRenderer sprite;
	UIManagers uiManager;
	//GameState gameState;
	GameObject m;

	private int value;

	void Start () 
	{
		m = GameObject.Find("Managers");
		uiManager = m.GetComponent<UIManagers>();
		//gameState = m.GetComponent<GameState>();

//		int[] x = {75, 100, 100, 125, 125, 125,125,125,150, 150,150,150,150,150,150,150,150}; // random value will be selected from this array to give the money instantiated a value
//
//		value = x[Random.Range(0 , gameState.currentLevel)]; // max amount will be determind by the level degree

		int[] y = {50, 75, 100, 100, 125, 125, 150};
		value = y[Random.Range(0, y.Length)];

		sprite = GetComponent<SpriteRenderer>();

		StartCoroutine(Fade());
	}

	IEnumerator Fade()
	{
		yield return new WaitForSeconds(.3f);
		int turn = 0;
		while(turn < 9)
		{
			yield return new WaitForSeconds(0.05f);
			sprite.color = new Color32( 255 , 255 , 255 , 0);
			turn++;
			yield return new WaitForSeconds(0.05f);
			sprite.color = new Color32( 255 , 255 , 255 , 225);
			turn++;

			if(turn >= 8)
			{
				GameState.money += (value);
				uiManager.HighlightPriceText();
				Destroy(this.gameObject);
			}
		}
	}
}
