using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour 
{
	public static SoundManager instance = null;
	public AudioClip[] musics; // 0: intro
	public bool canPlaySFX = true;
	public bool canPlay = true;

	AudioSource source;
	string MusicStat = "Music Stat";
	string SFXStat = "SFX Stat";

	void Awake () 
	{
		if(instance == null)
		{
			instance = this;
			source = GetComponent<AudioSource>();

			int i = PlayerPrefs.GetInt(MusicStat);
			int y = PlayerPrefs.GetInt(SFXStat);

			if(i == 1) // false
				canPlay = false;
			else if(i == 0) // true
				canPlay = true;

			if(y == 1) // false
				canPlaySFX = false;
			else if(y == 0) // true
				canPlaySFX = true;
		}
		else if(instance != this)
		{
			Destroy(gameObject);
		}

		DontDestroyOnLoad(gameObject);
	}
	
	void Update()
	{
		if(source.isPlaying == false && canPlay == true)
			PlayRandom(true, 0.4f);
	}

	public bool GetMusicStat()
	{
		int i = PlayerPrefs.GetInt(MusicStat);
		if(i == 0)
			return true;
		else
			return false;
	}

	public bool GetSFXStat()
	{
		int i = PlayerPrefs.GetInt(SFXStat);
		if(i == 0)
			return true;
		else
			return false;
	}

	public void SetCanPlay(bool c)
	{
		canPlay = c;

		PlayMusic(0, c);

		if(c == true)
			PlayerPrefs.SetInt(MusicStat, 0);
		else if(c == false)
			PlayerPrefs.SetInt(MusicStat, 1);
	}

	public void SetCanPlaySFX(bool c)
	{
		canPlaySFX = c;

		if(c == true)
			PlayerPrefs.SetInt(SFXStat, 0);
		else if(c == false)
			PlayerPrefs.SetInt(SFXStat, 1);
	}

	public void PlayMusic (int i, bool p) 
	{
		if(i == 0 && p == true) // intro
		{
			source.clip = musics[i];
			//source.Play();
			source.PlayDelayed(1.4f);
			source.loop = true;
		}
		else if(i == 0 && p == false)
		{
			source.clip = null;
			source.loop = false;
			source.Stop();
		}
		else if(i == 1 && p == true)
		{
			PlayRandom(p);
		}
		else if(i == 1 && p == false)
		{
			return;
		}
	}

	void PlayRandom(bool p, float i = 1.4f)
	{
		if(canPlay == true)
		{
			source.clip = musics[Random.Range(1, musics.Length)];
			source.PlayDelayed(i);
		}
	}
}
