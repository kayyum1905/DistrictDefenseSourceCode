using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour 
{
	public static GameManager instance = null;
	public static int[] unitCost = {75, 125, 175, 200, 200, 25, 225, 125, 75, 225};

	string currentLevel = "Current Level";
	string reachedLevel = "Reached Level";

	void Awake () 
	{
		if(instance == null)
		{
			instance = this;
		}
		else if(instance != this)
		{
			Destroy(gameObject);
		}

		DontDestroyOnLoad(gameObject);
	}

	// just in case if reached or set level is bigger then it is supposed to be, they are fixed to max level
	void Start()
	{
		if(PlayerPrefs.GetInt(reachedLevel) > 16)
			PlayerPrefs.SetInt(reachedLevel, 16); // the value will be increased by every added level

		if(PlayerPrefs.GetInt(currentLevel) > 16)
			PlayerPrefs.SetInt(currentLevel, 16); // the value will be increased by every added level
	}

	public void SetReachedLevel(int l)
	{
		// lets assume the player started from lower level
		// this 'if' is useful so that the reached level is not changed
		if(l > PlayerPrefs.GetInt(reachedLevel)) 
		{
			PlayerPrefs.SetInt(reachedLevel, l);
			PlayerPrefs.SetInt(currentLevel, l);
			return;
		}

		PlayerPrefs.SetInt(currentLevel, l);
	}

	public void SetCurrentLevel(int cl)
	{
		PlayerPrefs.SetInt(currentLevel, cl);
	}

	public int GetCurrentLevel()
	{
		return PlayerPrefs.GetInt(currentLevel);
	}

	public int GetReachedLevel()
	{
		return PlayerPrefs.GetInt(reachedLevel);
	}
}
