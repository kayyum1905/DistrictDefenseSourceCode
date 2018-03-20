using UnityEngine;
using System.Collections;

public class ExplosionScript : MonoBehaviour 
{
	public Sprite[] sprites;
	public SoundManager soundManager;
	SpriteRenderer myRenderer;
	AudioSource audioS;

	void Start () 
	{
		myRenderer = GetComponent<SpriteRenderer>();
		audioS = GetComponent<AudioSource>();
		StartCoroutine(ChanngeSprite());

		if(soundManager == null)
		{
			soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
		}

		if(soundManager.canPlaySFX == true)
			audioS.Play();
	}

	IEnumerator ChanngeSprite() 
	{
		yield return new WaitForSeconds(0.05f);
		myRenderer.sprite = sprites[0];
		yield return new WaitForSeconds(0.05f);
		myRenderer.sprite = sprites[1];
		yield return new WaitForSeconds(0.05f);
		myRenderer.sprite = sprites[2];
		yield return new WaitForSeconds(0.05f);
		Destroy(this.gameObject);
	}
}
