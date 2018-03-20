using UnityEngine;
using System.Collections;

public class DistrictHealth : MonoBehaviour 
{
	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.gameObject.tag == "Enemy")
			Invoke("DecreaseHealth" , 1f);
	}

	void DecreaseHealth()
	{
		GameObject lm = GameObject.Find("LevelManager");
		lm.gameObject.GetComponent<LevelManager>().DistrictHealth();
	}
}
