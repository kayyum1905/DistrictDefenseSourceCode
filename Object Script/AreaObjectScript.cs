using UnityEngine;
using System.Collections;

public class AreaObjectScript : MonoBehaviour 
{
	public GameObject explosion;

	void Exterminate()
	{
		Destroy(gameObject);
	}

	void OnTriggerEnter2D (Collider2D unit)
	{
		if(unit.gameObject.tag == "Enemy")
		{
			if(unit.gameObject.name == "EnemyRamirez(Clone)")
				unit.GetComponent<EnemyUnit1Script>().GetDamage(10);
			if(unit.gameObject.name == "EnemyUnit2(Clone)")
				unit.GetComponent<EnemyUnit2Script>().GetDamage(8);
			if(unit.gameObject.name == "EnemyUnit3(Clone)")
				unit.GetComponent<EnemyUnit3Script>().GetDamage(6);

			Vector2 pos = unit.transform.position;
			Instantiate(explosion , new Vector2(pos.x, pos.y) , Quaternion.identity);

			Invoke("Exterminate", 0.2f);
		}
	}
}
