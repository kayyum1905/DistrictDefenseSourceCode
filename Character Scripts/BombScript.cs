using UnityEngine;
using System.Collections;

public class BombScript : UnitScript 
{
	public Sprite sprite;
	public GameObject explosion;

	BoxCollider2D objectCollider;
	bool bombTriggered = false;

	protected override void Start () 
	{
		id = 5;

		// check unit1scipt for all explanations
		health = initialHealth;
		anim = GetComponent<Animator>();
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
			this.transform.position = new Vector2(pos.x-0.17f,pos.y-0.5f); // bombs positon is shifted when positoned
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
		SetObject();
		GameState.money -= GameManager.unitCost[id];
		uiManager.HighlightPriceText();
		objectCollider.offset = new Vector2(0, .5f);
		objectCollider.size = new Vector2(1.2f, 0.55f);
		canAssemble = false;
		base.DestroyUnitOnHold();
		base.UnitCount(1);
	}

	void SetObject()
	{
		this.objectCollider.offset = new Vector2(0 , 0.25f);
		this.objectCollider.size = new Vector2(0.9f , 0.55f);
		this.spriteRenderer.sprite = sprite;
	}

	public void ExplodeThisBomb(bool explodedByOtherBomb)
	{
		if(this.positioned && !bombTriggered)
		{
			bombTriggered = true;
			this.objectCollider.size = new Vector2(4f , 2.7f);
			Vector2 pos = this.transform.position;
			GameObject b = (GameObject)Instantiate(explosion , new Vector2(pos.x , pos.y) , Quaternion.identity);
			b.GetComponent<ExplosionScript>().soundManager = this.soundManager;

			if(explodedByOtherBomb)
				Invoke("KillThisBomb" , .2f);
			}
	}

	void KillThisBomb()
	{
		SetGirdAvailableBool(true, gridID);
		base.KillMe();
	}

	IEnumerator Explode(GameObject enemy)
	{
		if(enemy != null)
			Physics2D.IgnoreCollision(enemy.GetComponent<Collider2D>(), GetComponent<Collider2D>(), true);

		yield return new WaitForSeconds(1.2f);
		ExplodeThisBomb(false);

		if(enemy != null)
			Physics2D.IgnoreCollision(enemy.GetComponent<Collider2D>(), GetComponent<Collider2D>(), false);

		yield return new WaitForSeconds(.2f);
		SetGirdAvailableBool(true, gridID);
		base.KillMe();
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.gameObject.tag == "Enemy" && positioned && !bombTriggered)
		{
			StartCoroutine(Explode(other.gameObject));
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

		if(unit.gameObject.tag == "Enemy" && positioned && !bombTriggered)
		{
			StartCoroutine(Explode(unit.gameObject));
		}

		if(unit.gameObject.tag == "Enemy" && bombTriggered)
		{
			if(unit.gameObject.name == "EnemyRamirez(Clone)")
				unit.GetComponent<EnemyUnit1Script>().GetDamage(14);
			if(unit.gameObject.name == "EnemyUnit2(Clone)")
				unit.GetComponent<EnemyUnit2Script>().GetDamage(12);
			if(unit.gameObject.name == "EnemyUnit3(Clone)")
				unit.GetComponent<EnemyUnit3Script>().GetDamage(10);

			Physics2D.IgnoreCollision(unit.GetComponent<Collider2D>(), GetComponent<Collider2D>(), true);
		}

		if(unit.gameObject.name == "Bomb(Clone)" && this.bombTriggered == true)
		{
			unit.gameObject.GetComponent<BombScript>().ExplodeThisBomb(true);
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
		return new Vector2(newPoz.x-0.17f,newPoz.y-0.5f);
	}
}
