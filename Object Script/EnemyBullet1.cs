using UnityEngine;
using System.Collections;

public class EnemyBullet1 : BulletScript 
{
	
	void Start () 
	{
		rb = GetComponent<Rigidbody2D>();
		rb.AddForce(new Vector2 (bulletSpeed * direction, yPos) , ForceMode2D.Impulse); 
	}

//	public void SendToPool ()
//	{
//		if(true)
//			Destroy(this.gameObject);
//	}
}
