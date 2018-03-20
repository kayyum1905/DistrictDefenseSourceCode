using UnityEngine;
using System.Collections;

public class Unit4Script : UnitScript 
{
	CircleCollider2D toxicCollider;
	ParticleSystem particles;

	protected override void Start ()
	{
		id = 6;
		
		// check unit1 script for all explanations
		health = initialHealth;
		anim = GetComponent<Animator>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		spriteRenderer.sortingOrder = 8;
		particles = GetComponent<ParticleSystem>();
		toxicCollider = GetComponentInChildren<CircleCollider2D>();
		particles.Pause(true);
		base.Start();
		base.registerIDToParentClass(id);
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

		if(gm.unitPresentY[yPosState] == false && this.positioned )
			gm.unitPresentY[yPosState] = true;
	}

	protected override void PositioningSetups()
	{
		base.PlayPositionedAnim();
		positioned = true;
		GameState.money -= GameManager.unitCost[id];
		gm.unitPresentY[yPosState] = true;
		positionedByGrid = false;
		uiManager.HighlightPriceText();
		canAssemble = false;
		healthBarSprite.GetComponent<SpriteRenderer>().sortingOrder = spriteRenderer.sortingOrder + 1;
		healthBarBack.GetComponent<SpriteRenderer>().sortingOrder = spriteRenderer.sortingOrder;
		healthBarBack.SetActive(true);
		healthBarSprite.SetActive(true);
		particles.Play(true);
		toxicCollider.enabled = true;
		base.DestroyUnitOnHold();
		base.UnitCount(1);
		InvokeRepeating("DecreaseHealth", 1f, 1f);
	}

	// Since it contains toxic food the units will decrease due to the hazard
	void DecreaseHealth ()
	{
		health -= 1;
		float xScale = (2f/initialHealth);
		healthBarSprite.transform.localScale -= new Vector3( xScale , 0 , 0);

		if(health <= 0)
		{
			gm.unitPresentY[yPosState] = false;
			SetGirdAvailableBool(true, gridID);
			base.KillMe();
		}
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

	protected override Vector2 SnapToGrid (Vector2 pos)
	{
		Vector2 newPoz = base.SnapToGrid(pos);
		//MoneyScript.money -= MoneyScript.unitCost[3];
		return new Vector2(newPoz.x,newPoz.y);
	}
}
