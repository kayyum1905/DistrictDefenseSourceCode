using UnityEngine;
using System.Collections;

public class BarrelScript : UnitScript 
{
	public Sprite[] barrelSprite2;

	protected override void Start ()
	{
		id = 0;

		// check unit1scipt for all explanations
		health = initialHealth;
		anim = GetComponent<Animator>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		spriteRenderer.sortingOrder = 8;
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
		{
			Vector2 pos = this.transform.position;
			this.transform.position = new Vector2(pos.x-0.17f,pos.y); // barrel positon is shifted when positoned
			PositioningSetups();
		}
	
		if(!playWithTouch)
		{
			if(!positioned)
			{
				transform.position = base.CalculatePositionToClick();
			}

			if(Input.GetMouseButton(0) && canAssemble && !positioned)  // with mouse
			{
				Vector2 pos = base.CalculatePositionToClick();
				transform.position = SnapToGrid(pos);
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
		GameState.money -= GameManager.unitCost[id];
		gm.unitPresentY[yPosState] = true;
		positioned = true;
		positionedByGrid = false;
		uiManager.HighlightPriceText();
		canAssemble = false;
		base.DestroyUnitOnHold();
		base.UnitCount(1);

		if(GameState.tutorial == true)
		{
			GameObject.Find("Canvas/Tutorial").GetComponent<TutorialScript>().GridIndicatorSetting(false);
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
				unit.GetComponent<BulletScript>().SendToPool();
			}

			if(this.health <= 14 && this.health > 10)
				this.spriteRenderer.sprite = barrelSprite2[0];
			else if(this.health <= 10 && this.health > 5)
				this.spriteRenderer.sprite = barrelSprite2[1];
			else if(this.health <= 5 && this.health > 0)
				this.spriteRenderer.sprite = barrelSprite2[2];
			else if(health <= 0)
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
		return new Vector2(newPoz.x-0.17f,newPoz.y);
	}
}
