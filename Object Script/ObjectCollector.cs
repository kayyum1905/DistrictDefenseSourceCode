using UnityEngine;
using System.Collections;

public class ObjectCollector : MonoBehaviour 
{
	void OnTriggerEnter2D (Collider2D unit)
	{
		Destroy(unit.gameObject);
	}
}
