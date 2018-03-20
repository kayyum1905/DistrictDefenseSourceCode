using UnityEngine;
using System.Collections;

public class IceScript : UnitScript 
{
	public Sprite[] iceSprite;
	public GameObject enemy;

	protected override void Start ()
	{
		// to access to the level manager object and the script
		//manager = GameObject.Find("Managers");

		StartCoroutine(FreezeEnemy(enemy));

	}

	// Freezer when instantating must set the ystatepos of this object. It must let enemies to shoot
	public void SetYPos(int y)
	{
		// to access the bool if enemy exist in the same row/col of the unit
		gm = manager.GetComponent<GameState>();

		health = initialHealth;
		spriteRenderer = GetComponent<SpriteRenderer>();
		positioned = true;
		canAssemble = false;
		triggered = false;

		uiManager = manager.GetComponent<UIManagers>();
		yPosState = y;
		gm.unitPresentY[yPosState] = true;
	}

	protected override void Update () 
	{
		if(gm.unitPresentY[yPosState] == false && this.positioned )
			gm.unitPresentY[yPosState] = true;
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.gameObject.tag == "EnemyBullet" && positioned)
		{
			StartCoroutine(GetDamaged(other.gameObject));
		}
	}


	IEnumerator FreezeEnemy(GameObject enemy)
	{
		//Physics2D.IgnoreCollision(enemy.GetComponent<Collider2D>(), GetComponent<Collider2D>(), true);
		yield return new WaitForSeconds(.09f);
		if(enemy != null)
		{
			enemy.transform.position = new Vector2(this.transform.position.x - .14f, enemy.transform.position.y);
			enemy.GetComponent<EnemyScript>().Freeze();
		}
		InvokeRepeating("Melting", 1f, 1f);
	}

	void Melting()
	{
		if(health > 0)
		{
			health--;

			if(this.health <= 6 && this.health > 4)
				this.spriteRenderer.sprite = iceSprite[0];
			else if(this.health <= 4 && this.health > 0)
				this.spriteRenderer.sprite = iceSprite[1];
			else if(health <= 0)
			{
				gm.unitPresentY[yPosState] = false;
				SetGirdAvailableBool(true, gridID);
				if(enemy != null)
					enemy.GetComponent<EnemyScript>().UnFreeze();
				Destroy(this.gameObject);
			}
		}
	}

	IEnumerator GetDamaged(GameObject unit)
	{
		yield return new WaitForSeconds(Random.Range(0.05f , 0.1f));

		if(health > 0)
		{
			if(unit.gameObject != null)
			{
				health -= unit.GetComponent<BulletScript>().damageCapacity;
				unit.GetComponent<BulletScript>().SendToPool();
			}

			if(this.health <= 6 && this.health > 4)
				this.spriteRenderer.sprite = iceSprite[0];
			else if(this.health <= 4 && this.health > 0)
				this.spriteRenderer.sprite = iceSprite[1];
			else if(health <= 0)
			{
				gm.unitPresentY[yPosState] = false;
				SetGirdAvailableBool(true, gridID);

				if(enemy != null)
					enemy.GetComponent<EnemyScript>().UnFreeze();
				Destroy(this.gameObject);
			}
		}
		spriteRenderer.color = new Color32( 220 , 20 , 20 , 225);
		Invoke("SetColor" , 0.03f);
	}

	protected override void SetColor()
	{
		base.SetColor();
	}
}