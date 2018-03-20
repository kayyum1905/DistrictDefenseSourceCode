using UnityEngine;
using System.Collections;

public class MamaScript : UnitScript 
{
	protected override void Start ()
	{
		id = 9;

		// check unit1 script for all explanations
		health = initialHealth;
		anim = GetComponent<Animator>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		spriteRenderer.sortingOrder = 8;
		audioSource = GetComponent<AudioSource>();
		base.Start();
		shootRoutine = ShootFire(reloadTime, timeBetweenFire);
		base.registerIDToParentClass(id);
	}

	protected override void Fire()
	{
		for(int i = 1; i <= bulletCount ; i ++)
		{
			Instantiate(bullet , new Vector2(bulletPos.position.x, bulletPos.position.y) , Quaternion.identity);
			base.PlayShootSFX();
		}
	}

	IEnumerator ShootFire(float reloadTimeVar , float betweenFiresVar)
	{
		while(true)
		{
			for (int i = 1 ; i <= shootRound ; i ++)
			{
				anim.SetTrigger("Shot");
				Fire();
				yield return new WaitForSeconds(betweenFiresVar);
			}
			anim.SetTrigger("Shot");
			Fire();
			yield return new WaitForSeconds(reloadTimeVar);
		}
	}

	protected override void Update () 
	{

		if(!player && !positioned && triggered)
		{
			canAssemble = true;
			triggered = false;
		}

		if(positionedByGrid && canAssemble)
			PositioningSetups();
	
		if(!playWithTouch)
		{
			if(!positioned)
			{
				transform.position = base.CalculatePositionToClick();   // with mouse
			}

			if(Input.GetMouseButton(0) && canAssemble && !positioned)  // with mouse
			{
				Vector2 pos = base.CalculatePositionToClick();
				this.transform.position = SnapToGrid(pos);
				if(pay == true)
					PositioningSetups();
			}
		}
		else if(playWithTouch)
		{
			if(!positioned && Input.touchCount > 0)
			{
				transform.position = base.CalculatePositionToTouch();
			}

			if (!canAssemble && Input.touchCount <= 0 && !positioned)
			{
				Destroy(this.gameObject);
				canAssemble = true;
			}

			if(Input.touchCount <= 0 && canAssemble && !positioned)  // with touch
			{
				Vector3 pos = SnapToGrid(this.transform.position);
				this.transform.position = pos;
				if(pay == true)
					PositioningSetups();
			}
		}

		if(!canShoot && gm.enemyPresentY[yPosState])
			StartShooting();

		if(!gm.enemyPresentY[yPosState])
		{
			StopCoroutine(shootRoutine);
			canShoot = false;
		}

		if(gm.unitPresentY[yPosState] == false && this.positioned )
			gm.unitPresentY[yPosState] = true;
	}

	protected override void PositioningSetups()
	{
		base.PlayPositionedAnim();
		GameState.money -= GameManager.unitCost[id];
		gm.unitPresentY[yPosState] = true;
		positioned = true;
		positionedByGrid = false;
		uiManager.HighlightPriceText();
		canAssemble = false;
		healthBarSprite.GetComponent<SpriteRenderer>().sortingOrder = spriteRenderer.sortingOrder + 1;
		healthBarBack.GetComponent<SpriteRenderer>().sortingOrder = spriteRenderer.sortingOrder;
		healthBarBack.SetActive(true);
		healthBarSprite.SetActive(true);
		base.DestroyUnitOnHold();
		base.UnitCount(1);
		StartShooting();
	}

	protected override void OnTriggerStay2D (Collider2D unit)
	{
		if(unit.gameObject.tag == "Unit" && !positioned)
		{
			triggered = true;
			canAssemble = false;
			this.player = unit;
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.gameObject.tag == "EnemyBullet" && positioned)
		{
			StartCoroutine(GetDamaged(other.gameObject));
		}
	}

	protected override void OnTriggerExit2D (Collider2D unit)
	{
		if(unit.gameObject.tag == "Unit" && !positioned)
		{
			triggered = false;
			canAssemble = true;
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
				float xScale = (2f/initialHealth) * unit.GetComponent<BulletScript>().damageCapacity;
				unit.GetComponent<BulletScript>().SendToPool();
				healthBarSprite.transform.localScale -= new Vector3( xScale , 0 , 0);
			}

			if(health <= 0)
			{
				gm.unitPresentY[yPosState] = false;
				SetGirdAvailableBool(true, gridID);
				base.KillMe();
			}
		}
		spriteRenderer.color = new Color32( 220 , 20 , 20 , 225);
		Invoke("SetColor" , 0.03f);
	}

	protected override void SetColor()
	{
		base.SetColor();
	}

	protected override void StartShooting()
	{
		canShoot = gm.PermitUnitToShoot(yPosState);
//
//		if(canShoot)
//		{
//			StartCoroutine( ShootFire( reloadTime , timeBetweenFire ));
//		}

		if(canShoot && positioned)
		{
			StartCoroutine(shootRoutine);
		}
	}

	protected override Vector2 SnapToGrid (Vector2 pos)
	{
		Vector2 newPoz = base.SnapToGrid(pos);
		//MoneyScript.money -= MoneyScript.unitCost[2];
		return new Vector2(newPoz.x,newPoz.y);
	}
}