using UnityEngine;
using System.Collections;

public class EnemyUnit2Script : EnemyScript 
{
	protected override void Start () 
	{
		base.Start();

		rb = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		sprite = GetComponent<SpriteRenderer>();
		health = initialHealth;
		SetSpeed(initialSpeed);
		float yPos = transform.position.y;

		if(yPos == 0.7f)
		{
			sprite.sortingOrder = 1;
			yPosState = 0;
		}
		else if(yPos == -0.3f)
		{
			sprite.sortingOrder = 2;
			yPosState = 1;
		}
		else if(yPos == -1.3f)
		{
			sprite.sortingOrder = 3;
			yPosState = 2;
		}
		else if(yPos == -2.3f)
		{
			sprite.sortingOrder = 4;
			yPosState = 3;
		}
		else if(yPos == -3.3f)
		{
			sprite.sortingOrder = 5;
			yPosState = 4;
		}

		gm.EnemyStatePos(yPosState, true);
		healthBarSprite.GetComponent<SpriteRenderer>().sortingOrder = sprite.sortingOrder + 1;
		healthBarBack.GetComponent<SpriteRenderer>().sortingOrder = sprite.sortingOrder;

		shootRoutine = ShootFire(3f , shootCount);
	}

	IEnumerator ShootFire(float reloadTimeVar , float betweenFiresVar)
	{
		while(true)
		{
			yield return new WaitForSeconds(reloadTimeVar);
			for (int i = 1 ; i <= 7 ; i ++)
			{
				//anim.SetTrigger("Shoot");
				base.Fire();
				yield return new WaitForSeconds(betweenFiresVar);
			}
			//anim.SetTrigger("Shoot");
			base.Fire();
		}
	}

	void Update()
	{
		if(!canShoot && gm.unitPresentY[yPosState])
			StartShooting();

		if(!gm.unitPresentY[yPosState])
		{
			StopCoroutine(shootRoutine);
			canShoot = false;
		}

		if(triggered && !player)
		{
			SetSpeed(initialSpeed);
			anim.SetBool("Collision" , false);
			triggered = false;
		}

		if(triggeredByEnemyUnit && !enemyUnit && !isMoving)
		{
			SetSpeed(initialSpeed);
			anim.SetBool("Collision" , false);
			triggeredByEnemyUnit = false;
		}

		if(gm.enemyPresentY[yPosState] == false)
			gm.enemyPresentY[yPosState] = true;
	}

	protected override void StartShooting()
	{
		base.StartShooting();
	}

	void OnTriggerStay2D (Collider2D unit)
	{
		if(unit.gameObject.tag == "Unit" && unit.GetComponent<UnitScript>().positioned && !isSick && !freezed)
		{
			if(unit.gameObject.name == "IceObject(Clone)")
				Physics2D.IgnoreCollision(unit.GetComponent<Collider2D>(), GetComponent<Collider2D>(), true);

			triggered = true;
			anim.SetBool("Collision" , true);
			this.player = unit;
			base.SetSpeed(0);
		}
		else if(unit.gameObject.tag == "Unit" && !unit.GetComponent<UnitScript>().positioned)
		{
			return;
		}
	}

	protected override void OnTriggerEnter2D (Collider2D unit)
	{
		if(unit.gameObject.tag == "Bullet")
		{
			StartCoroutine(GetDamaged(unit.gameObject));
		}

		if(unit.gameObject.tag == "Enemy" && isMoving && !isSick && !freezed)
		{
			if(unit.gameObject.GetComponent<EnemyScript>().isSick == false)
			{
				triggeredByEnemyUnit = true;
				anim.SetBool("Collision" , true);
				this.enemyUnit = unit;
				base.SetSpeed(0);
			}
		}

		if(unit.gameObject.tag == "Toxin" && !isSick)
		{
			Invoke( "GetSick", 0.4f);
		}

		if(unit.gameObject.tag == "Unit" && unit.GetComponent<UnitScript>().positioned && !isSick)
		{
			triggered = true;
			anim.SetBool("Collision" , true);
			this.player = unit;
			base.SetSpeed(0);
		}
		else if(unit.gameObject.tag == "Unit" && !unit.GetComponent<UnitScript>().positioned)
		{
			return;
		}

		if(unit.gameObject.tag == "BattleField")
			StartShooting();
	}

	protected override void OnTriggerExit2D (Collider2D other)
	{
		if(other.gameObject.tag == "Enemy" && !isSick)
		{
			if(other.gameObject.GetComponent<EnemyScript>().isSick == false)
			{
				triggeredByEnemyUnit = false;
				this.enemyUnit = other;
				anim.SetBool("Collision" , false);
				base.SetSpeed(initialSpeed);
			}
		}

		if(other.gameObject.tag == "Unit" && !isSick && !freezed)
		{
			base.SetSpeed(initialSpeed);
			anim.SetBool("Collision" , false);
			triggered = false;
		}
	}

	IEnumerator GetDamaged(GameObject unit)
	{
		yield return new WaitForSeconds(Random.Range(0.15f , 0.25f));

		if(health > 0)
		{
			if(unit.gameObject != null)
			{
				health -= unit.GetComponent<BulletScript>().damageCapacity;
				float xScale = (2f/initialHealth) * unit.GetComponent<BulletScript>().damageCapacity;
				healthBarSprite.transform.localScale -= new Vector3( xScale , 0 , 0);
				unit.GetComponent<BulletScript>().SendToPool();
			}

			if(health <= 0)
			{
				base.KillUnit(yPosState);
			}
		}
		sprite.color = new Color32( 220 , 20 , 20 , 225);
		Invoke("SetColor" , 0.03f);
	}

	protected override void SetColor()
	{
		base.SetColor();
	}

	public void GetDamage(int impact)
	{
		this.health -= impact;
		float xScale = (2f/initialHealth) * impact;
		healthBarSprite.transform.localScale -= new Vector3( xScale , 0 , 0);

		if(health <= 0)
		{
			KillUnit(yPosState);
		}
	}
}
