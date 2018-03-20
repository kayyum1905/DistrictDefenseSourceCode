using UnityEngine;
using System.Collections;

public class FreezerScript : UnitScript 
{
	public GameObject ice;

	BoxCollider2D objectCollider;
	bool freezerTriggered = false;

	protected override void Start () 
	{
		id = 8;

		// check unit1scipt for all explanations
		spriteRenderer = GetComponent<SpriteRenderer>();
		spriteRenderer.sortingOrder = 8;
		objectCollider = GetComponent<BoxCollider2D>();
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
			this.transform.position = new Vector2(pos.x-0.16f,pos.y-0.5f);
			PositioningSetups();
		}
	
		if(!playWithTouch)
		{
			if(!positioned)
			{
				transform.position = base.CalculatePositionToClick();   // with mouse
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
	}

	protected override void PositioningSetups()
	{
		positioned = true;
		GameState.money -= GameManager.unitCost[id];
		uiManager.HighlightPriceText();
		objectCollider.offset = new Vector2(0, .5f);
		canAssemble = false;
		base.DestroyUnitOnHold();
		base.UnitCount(1);
	}

	IEnumerator CheckEnemyPosition(GameObject other)
	{
		bool enemyIn = true;

		while (enemyIn == true) 
		{
			yield return new WaitForSeconds(.01f);
			if (other.transform.position.x > this.transform.position.x - 0.399f && 
				other.transform.position.x < this.transform.position.x + .1f) 
			{
				enemyIn = false;
				RevealIce(other);
			}
		}
	}

	void RevealIce(GameObject unit)
	{
		freezerTriggered = true;

		Vector2 pos = this.transform.position;
		GameObject iceSpawned = (GameObject)Instantiate(ice , new Vector2(pos.x, pos.y + .4f) , Quaternion.identity);

		IceScript _is = iceSpawned.GetComponent<IceScript>();

		_is.gridID = this.gridID;
		_is.manager = this.manager;
		_is.enemy = unit;
		_is.SetYPos(this.yPosState);

		iceSpawned.GetComponent<SpriteRenderer>().sortingOrder = this.spriteRenderer.sortingOrder + 1;

		base.KillMe();
		//Invoke("DestroyFreezer", 0.3f);
	}

	void DestroyFreezer()
	{
		base.KillMe();
	}

//	void OnTriggerEnter2D(Collider2D other)
//	{
//		if(other.gameObject.tag == "Enemy" && positioned && !freezerTriggered)
//			Physics2D.IgnoreCollision(other.GetComponent<Collider2D>(), GetComponent<Collider2D>(), true);
//
//		if(other.gameObject.tag == "Enemy" && positioned && !freezerTriggered)
//		{
//			if (other.transform.position.x >= this.transform.position.x - 0.4f)
//			{
//				if (other.transform.position.x <= this.transform.position.x)
//				{
//					//Physics2D.IgnoreCollision(other.GetComponent<Collider2D>(), GetComponent<Collider2D>(), true);
//					StartCoroutine(RevealIce(other.gameObject));
//				}
//			}
//		}
//	}

	protected override void OnTriggerStay2D (Collider2D other)
	{
		if(other.gameObject.tag == "Unit" && !positioned)
		{
			triggered = true;
			canAssemble = false;
			this.player = other;
		}

//		if(other.gameObject.tag == "Enemy" && positioned && !freezerTriggered)
//			Physics2D.IgnoreCollision(other.GetComponent<Collider2D>(), GetComponent<Collider2D>(), true);

		if(other.gameObject.tag == "Enemy" && positioned && !freezerTriggered)
		{
			Physics2D.IgnoreCollision(other.GetComponent<Collider2D>(), GetComponent<Collider2D>(), true);
			if (other.transform.position.x > this.transform.position.x - 0.29f)
			{
				StartCoroutine(CheckEnemyPosition(other.gameObject));
			}
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

	protected override Vector2 SnapToGrid (Vector2 pos)
	{
		Vector2 newPoz = base.SnapToGrid(pos);
		//MoneyScript.money -= MoneyScript.unitCost[0];
		return new Vector2(newPoz.x-0.16f,newPoz.y-0.5f);
	}
}
