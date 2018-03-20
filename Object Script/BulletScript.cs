using UnityEngine;
using System.Collections;

public class BulletScript : MonoBehaviour 
{
	public int damageCapacity;
	public Transform initPos;

	public float direction;
	public float bulletSpeed;
	public float yPos;

	protected Rigidbody2D rb;

	void Start () 
	{
		rb = GetComponent<Rigidbody2D>();
		rb.AddForce(new Vector2 (bulletSpeed * direction, yPos) , ForceMode2D.Impulse); 
	}

	public void SendToPool ()
	{
		Destroy(this.gameObject);
//		this.gameObject.SetActive(false);
//		this.transform.position = initPos.position;
	}

}
