using UnityEngine;
using System.Collections;

public class ThiefScript : UnitScript 
{
	int theftCount = 0;

	protected override void Start ()
	{
		id = 7;

		// check unit1 script for all explanations
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
	}

	protected override void PositioningSetups()
	{
		positioned = true;
		positionedByGrid = false;
		GameState.money -= GameManager.unitCost[id];
		uiManager.HighlightPriceText();
		canAssemble = false;
		spriteRenderer.color = new Color32(195, 195, 195, 150);
		base.PlayPositionedAnim();
		base.DestroyUnitOnHold();
		base.UnitCount(1);
	}

	protected override void OnTriggerStay2D (Collider2D other)
	{
		if(other.gameObject.tag == "Unit" && !positioned)
		{
			triggered = true;
			canAssemble = false;
			this.player = other ;
		}

		if(other.gameObject.tag == "Enemy" && positioned)
		{
			Invoke("Steal", 1.2f);
			Physics2D.IgnoreCollision(other.GetComponent<Collider2D>(), GetComponent<Collider2D>(), true);
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{

	}

	protected override void OnTriggerExit2D (Collider2D unit)
	{
		if(unit.gameObject.tag == "Unit" && !positioned)
		{
			triggered = false;
			canAssemble = true;
		}
	}

	void Steal()
	{
		anim.SetTrigger("Steal");

		int[] randomMoney = {0, 25, 50, 75, 100};

		GameState.money += randomMoney[Random.Range(0, randomMoney.Length)];
		theftCount++;

		uiManager.HighlightPriceText(); // in case stealen money makes it available to buy unit

		if(theftCount == 7)
		{
			SetGirdAvailableBool(true, gridID);
			base.KillMe();
		}
	}

	protected override void SetColor()
	{
		base.SetColor();
	}

	protected override Vector2 SnapToGrid (Vector2 pos)
	{
		Vector2 newPoz = base.SnapToGrid(pos);
		//MoneyScript.money -= MoneyScript.unitCost[1];
		return new Vector2(newPoz.x,newPoz.y);
	}
}
