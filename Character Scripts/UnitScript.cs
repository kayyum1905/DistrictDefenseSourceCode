using UnityEngine;
using System.Collections;

public class UnitScript :  MonoBehaviour
{
	public GameObject bullet; //Bullet object
	public GameObject healthBarSprite , healthBarBack ; // the health bar and the red background
	public GameObject manager; // unit class may have to access this class for disabling the spawn button for while
	public Transform bulletPos; // the position where the bullet will be spawned 
	public bool positioned; // bool check if the unit is positioned/snaped
	public float reloadTime , timeBetweenFire; // time of reload and the period between each shoot
	public int bulletCount; // mainly its one but for shotgun it is more
	public int shootRound; // how much bullets will be shoot between reloads ( e.g. shotgun round is one)
	public int initialHealth; // every unit have its unique initial health, declared in editor

	// when character selected from the selection panel, buttonlocation is the position of spawning button
	public int buttonLocation;

	 // the grid id it is placed
	 public int gridID; 

	// the id of the unit must be set even before it is spawned. therefor it is serliazable
	[SerializeField]
	public int id; // the id of the unit, useful when calculating cost related things
	public bool pay; //actually it indicates that the snaping was successful and paymenty must be done
	public bool positionedByGrid; // instead of dragging unit was positioned via press the grids

	protected int health; // the health
	protected int yPosState; // this is to indicate the position on Y and query from gamestate enemy position to shoot or not
	protected bool canShoot; // must get the info from update and apply it to the enumaration to shoot
	protected bool canAssemble; // can be positioned
	protected bool triggered; // if triggered by other firendly units while dragging
	protected bool playWithTouch; // check if platform is mobile or editor

	protected Collider2D player; // when dragging collided unit object is taken, if null a bool will set default
	protected GameObject spawner; // to reach to the spawning button components
	protected Animator anim; // animator
	protected SpriteRenderer spriteRenderer; //  there are useful occasions
	protected GameState gm;
	protected SoundManager soundManager;

	protected IEnumerator shootRoutine; // to call enumerator when there is enemy in the same row
	protected UIManagers uiManager; // to access the highlest price text func
	protected AudioSource audioSource;

	//bool deactivateSpawnButton = true; // if the snapping operationm is successful, the specific spawn button can be disabled

		void Awake()
	{
		#if UNITY_EDITOR // do that things dont go different on mobile testing and unity editor testing/playing
			if(UnityEditor.EditorApplication.isRemoteConnected)
			{
				playWithTouch = true;
			}
			else
			{
				playWithTouch = false;
			}
		#elif UNITY_IOS
			playWithTouch = true;
		#elif UNITY_ANDROID
			playWithTouch = true;
		#else
			playWithTouch = false;
		#endif
	}

	protected virtual void Start () 
	{
		// at the very begining since objects are not positioned nor triggered these are the setups
		// therefor they can be assembled or snaped to a grid
		positioned = false;
		canAssemble = true;
		triggered = false;
		canShoot = false;
		positionedByGrid = false;
		pay = true;

		// the object spawner contains the spawner buttons and script related to spawning all kind of units
		spawner = GameObject.Find("ObjectSpawner");

		// to access to the level manager object and the script
		manager = GameObject.Find("Managers");

		// play sfx from sound manager instead of playing them individually
		soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();

		// to access the bool if enemy exist in the same row/col of the unit
		gm = manager.GetComponent<GameState>();

		uiManager = manager.GetComponent<UIManagers>();
	}

	protected void registerIDToParentClass(int x)
	{
		this.id = x;
	}

	public int ReturnID()
	{
		return this.id;
	}

	protected virtual void Fire()
	{

	}

	protected virtual void Update () 
	{

	}

	protected virtual void PositioningSetups()
	{
		
	}

	protected virtual void OnTriggerStay2D (Collider2D unit)
	{

	}

	protected virtual void OnTriggerExit2D (Collider2D unit)
	{

	}

	// when hit by a bullet it takes different color then this method gives the default color
	protected virtual void SetColor()
	{
		spriteRenderer.color = new Color32(255 , 255 , 255 , 255);
	}

	protected virtual void StartShooting()
	{
		
	}

	protected virtual Vector2 SnapToGrid (Vector2 pos)
	{
		float x = pos.x;
		float y = pos.y;

		int gidX = 0; // used to calculate gridid

		if( x <= 6.75f && x > 4.75f)
		{
			x = 5.5f;
			gidX = 35;
		}
		else if (x <= 4.75f && x > 3.25f)
		{
			x = 4f;
			gidX = 30;
		}
		else if(x <= 3.25f && x > 1.75f)
		{
			x = 2.5f;
			gidX = 25;
		}
		else if(x <= 1.75f && x > 0.25f)
		{
			x = 1f;
			gidX = 20;
		}
		else if(x <= 0.25f && x > -1.25f)
		{
			x = -0.5f;
			gidX = 15;
		}
		else if(x <= -1.25f && x > -2.75f)
		{
			x = -2f;
			gidX = 10;
		}
		else if(x <= -2.75f && x > -4.25f)
		{
			x = -3.5f;
			gidX = 5;
		}
		else if(x <= -4.25f && x > -6f)
		{
			x = -5f;
			gidX = 0;
		}
		else
		{
			pay = false;
			DisableUnit();
		}

		// the sorting orders are set regarded to the y posiiton of the grid
		if (y <= 1.3f && y > 0.2f)
		{ 
			y = 0.7f;
			spriteRenderer.sortingOrder = 1;
		}
		else if(y <= 0.2f && y > -0.8f)
		{
			y = -0.3f;
			spriteRenderer.sortingOrder = 2;
		}
		else if(y <= -0.8f && y > -1.8f)
		{
			y = -1.3f;
			spriteRenderer.sortingOrder = 3;
		}
		else if(y <= -1.8f && y > -2.8f)
		{
			y = -2.3f;
			spriteRenderer.sortingOrder = 4;
		}
		else if(y <= -2.8f && y > -3.8f)
		{
			y = -3.3f;
			spriteRenderer.sortingOrder = 5;
		}
		else
		{
			pay = false;
			DisableUnit();
		}

		if(pay && /* deactivateSpawnButton && */ !triggered)
		{
			PrePositioningSettings();

			// calculating grid ID, if unit is positoned by place button grid id will be given by game state script
			gridID = gidX + spriteRenderer.sortingOrder - 1;
			SetGirdAvailableBool(false, gridID);
		}

		return new Vector2(x,y);
	}

	public void PrePositioningSettings()
	{
		if(manager == null)
		{
			manager = GameObject.Find("Managers");
			gm = manager.GetComponent<GameState>();
			soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
			audioSource = GetComponent<AudioSource>();
		}

		manager.GetComponent<UIManagers>().ActivateText(buttonLocation); // devactivating spawn button
		yPosState = spriteRenderer.sortingOrder - 1;
		gm.canInsantiate = true;
	}

	// when positioned without clicking to spawn button but via grid
	public void PositionUnitOnGrid(int g)
	{
//		float[] xPos = {-5f, -3.5f, -2f, -.5f, 1f, 2.5f, 4f, 5.5f};
//		float[] yPos = {.7f, -.3f, -1.3f, -2.3f, -3.3f};
//
//		Vector2 pos =  new Vector2(xPos[g / 5], yPos[g % 5]);
//		int[] sorting = {1, 2, 3, 4, 5};
//		this.transform.position = pos;
//
//		pay = true;
//		positionedByGrid = true;
//		GetComponent<SpriteRenderer>().sortingOrder = sorting[g % 5];
//		PrePositioningSettings();
//		gridID = g;
//		gm.gridAvailable[g] = false

		spriteRenderer = GetComponent<SpriteRenderer>();
		//Vector2 pos = new Vector2(0,0);

		if(playWithTouch == true)
			this.transform.position  = CalculatePositionToTouch();
		else if(playWithTouch == false)
			this.transform.position  = CalculatePositionToClick();

//		if(this.gameObject.name == "Bomb(Clone)" || this.gameObject.name == "Freezer(Clone)")
//		{
//			Vector2 posNew = SnapToGrid(this.transform.position );
//			this.transform.position = new Vector2(posNew.x-0.17f,posNew.y-0.5f);
//			this.positionedByGrid = true;
//			return;
//		}

		this.transform.position = SnapToGrid(this.transform.position );
		this.positionedByGrid = true;
	}

	void DisableUnit()
	{
		this.gameObject.SetActive(false);
		gm.canInsantiate = true;
	}

	public void PlayPositionedAnim()
	{
		anim.SetTrigger("positioned");
	}

	// add or subtract unitNo from levelmanager
	protected void UnitCount(int i)
	{
		LevelManager.unitNo += i;
		print("Friendly Unit On field: " + LevelManager.unitNo);
	}

	protected void KillMe()
	{
		UnitCount(-1);
		Destroy(this.gameObject);
	}

	// when the unit is positioned by dragging or it is going to be killed. must inform gamestate to enable place button
	protected void SetGirdAvailableBool(bool n, int m)
	{
		gm.gridAvailable[m] = n;
	}

	// when the unit is positioned it must null anything that in in unit on hold (gamestate) to prevent errors
	protected void DestroyUnitOnHold()
	{
		if(gm.unitOnHold != null)
		{
			gm.unitOnHold = null;
		}
	}

	protected void PlayShootSFX()
	{
		if(soundManager.canPlaySFX == false)
			return;
		
		audioSource.Play();
	}

	protected virtual Vector2 CalculatePositionToClick()
	{
		Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector2 unitPos = transform.position;
		unitPos.x = pos.x;
		unitPos.y = pos.y;
		return unitPos;
	}

	protected virtual Vector2 CalculatePositionToTouch ()
	{
		Vector3 pos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
		Vector2 unitPos = transform.position;
		unitPos.x = pos.x;
		unitPos.y = pos.y;
		return unitPos;
	}
}
