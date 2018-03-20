using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour 
{
	public GameObject levelPanel;
	public GameObject settingsPanel;
	public GameObject gm; // gamemanager object

	public Toggle musicToggle, sfxToggle;

	public GameObject[] levelButtons;
	public Button survival, quick;

	GameObject soundManagerObject;
	GameManager gmScript;
	SoundManager sm;

	void Start ()
	{
		gmScript = gm.GetComponent<GameManager>();
		soundManagerObject = GameObject.Find("SoundManager");
		sm = soundManagerObject.GetComponent<SoundManager>();
		musicToggle.isOn = sm.GetMusicStat();
		sfxToggle.isOn = sm.GetSFXStat();
		UnlockLevelButtons();

		if(sm.canPlay == true)
			sm.PlayMusic(0, true);
	}

	void UnlockLevelButtons()
	{
		int l = gmScript.GetReachedLevel();
		UnlockSurvivalMode(l);
		UnlockQuickButton(l);
		print("Level Reached: " + l);

		if(l >= 16)
			return;

		if(l == 0)
			l = 1;

		for (int i = l; i < levelButtons.Length; i++) 
		{
			levelButtons[i].GetComponent<Button>().interactable = false;
			levelButtons[i].GetComponent<Image>().color = new Color32(70,70,70,255);
			levelButtons[i].GetComponentInChildren<Text>().text = "?";
		}
	}

	void UnlockSurvivalMode(int l)
	{
		if(l >= 6)
		{
			survival.interactable = true;
		}
		else
		{
			survival.interactable = false;
			survival.GetComponent<Image>().color = new Color32(0,200,100,50);
		}
	}

	void UnlockQuickButton(int l)
	{
		if(l >= 1)
			quick.interactable = true;
		else
		{
			quick.interactable = false;
			//quick.GetComponent<Image>().color = new Color32(0,200,100,50);
		}
	}

	// this is called bu the settings can play music or not
	public void MusicSetting()
	{
		sm.SetCanPlay(musicToggle.isOn);
	}

	public void SFXSetting()
	{
		sm.SetCanPlaySFX(sfxToggle.isOn);
	}

	// !------ DANGER ZONE -------!
	public void DeleteData()
	{
		PlayerPrefs.DeleteAll();
		UnlockLevelButtons();
	}

	public void SelectScene (int sceneNo) 
	{
		gmScript.SetCurrentLevel(sceneNo);
		SceneManager.LoadScene( 1 , LoadSceneMode.Single);
		StopIntroMusic();
	}

	public void QuickStart()
	{
		SceneManager.LoadScene( 1 , LoadSceneMode.Single);
		StopIntroMusic();
	}

	public void LevelPanelActivation(bool process)
	{
		levelPanel.SetActive(process);
	}

	public void SettingsPanel()
	{
		settingsPanel.SetActive(true);
	}

	public void GoToAppStore()
	{
		Application.OpenURL("https://itunes.apple.com/app/district-defense/id1157373352?ls=1&mt=8");
	}

	public void CloseSettingsPanel()
	{
		settingsPanel.SetActive(false);
	}

	void StopIntroMusic()
	{
		soundManagerObject.GetComponent<SoundManager>().PlayMusic(0, false);
	}
}
