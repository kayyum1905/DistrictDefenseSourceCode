using UnityEngine;
using System.Collections;

public class BarFloorLighting : MonoBehaviour 
{
	SpriteRenderer sprite;

	void Start () 
	{
		sprite = GetComponent<SpriteRenderer>();

		StartCoroutine(Color());
	}

	IEnumerator Color (int x = 0) 
	{
		Color32[] colors = {new Color32(255, 75, 75, 255), new Color32(255, 240, 75, 255),
							new Color32(100, 255, 100, 255), new Color32(255, 110, 160, 255), new Color32(75, 200, 250, 255)};

		while (true) 
		{
			yield return new WaitForSeconds(1f);
			sprite.color = colors[x];
			x++;

			if(x > 4)
				x = 0;
		}
	}
}
