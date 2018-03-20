using UnityEngine;
using System.Collections;

public class EnemyScript : MonoBehaviour 
{
	public GameObject bullet;
	public GameObject healthBarSprite , healthBarBack ;
	public GameObject flame;  // When the bullet is instantiated flame is enabled
	public GameObject hurtSprite;
	public GameObject money;
	public Transform bulletPos;

	public float shootCount;
	public float initialSpeed;
	public int initialHealth;
	public bool isSick;  // when collided with the toxin
	 
	protected Rigidbody2D rb;
	protected Collider2D player;  // when collided with player saves it, in case player is killed this unit must know it is null
	protected Collider2D enemyUnit; // to know the collided enemy unit is null when standing beside

	protected int health;
	protected int yPosState; // this is to indicate the position on Y and inform gamestate so the freindly unit may shoot or not
	protected float speed;

	protected bool triggered;
	protected bool triggeredByEnemyUnit;
	protected bool isMoving;
	protected bool canShoot;
	[SerializeField]
	public bool freezed;

	protected Animator anim;
	protected SpriteRenderer sprite;
	protected GameState gm;
	protected IEnumerator shootRoutine;
	protected GameObject manager;
	protected SoundManager soundManager;
	protected AudioSource audioSource;

	void Awake()
	{
		this.isSick = false;
	}

	protected virtual void Start () 
	{
		manager = GameObject.Find("Managers");
		gm = manager.GetComponent<GameState>();

		soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
		audioSource = GetComponent<AudioSource>();
	}

	protected void Fire()
	{
		ActivateFlame();
		Instantiate(bullet , new Vector2(bulletPos.position.x , bulletPos.position.y) , Quaternion.identity);
		PlayShootSFX();
		Invoke("ActivateFlame" , 0.1f);
	}

	protected void ActivateFlame()
	{
		flame.SetActive(!flame.activeSelf);
	}


	protected virtual void StartShooting()
	{
		canShoot = gm.PermitEnemyToShoot(yPosState);

		if(canShoot)
			StartCoroutine(shootRoutine);
	}

	protected void FixedUpdate () 
	{
		rb.velocity = new Vector2 ( - speed , 0f) * Time.deltaTime;
	}

	protected virtual void OnTriggerEnter2D (Collider2D unit)
	{

	}

	protected virtual void OnTriggerExit2D (Collider2D other)
	{

	}

	//when the unit moves by the toxin smoke : unit4 has toxic meal there
	protected void GetSick()
	{
		if(this.isSick == false) // it is possible that colliding with anathor toxic unit can reverse some settings
		{
			this.sprite.color = new Color32( 120, 255, 120, 255);
			this.SetSpeed(-initialSpeed/2f);
			this.isSick = true;
		}
	}

	// when hit by a bullet it takes different color then this method gives the default color
	protected virtual void SetColor()
	{
		if(!this.isSick)
			this.sprite.color = new Color32(255 , 255 , 255 , 255);
		else
			this.sprite.color = new Color32( 120, 255, 120, 255);
	}

	protected virtual void SetSpeed(float s)
	{
		if(this.freezed == true)
			return;
		
		speed = s;

		if(speed > 0)
			this.isMoving = true;
		else
			this.isMoving = false;
	}

	public void Freeze()
	{
		this.SetSpeed(0);
		this.anim.SetBool("Collision" , true);
		//this.canShoot = false;
		this.freezed = true;
		gm.EnemyStatePos(yPosState, false);
		this.StopShooting();
	}

	public void UnFreeze()
	{
		this.freezed = false;
		this.SetSpeed(initialSpeed);
		this.anim.SetBool("Collision" , false);
		//this.canShoot = true;
		gm.EnemyStatePos(yPosState, true);
		this.triggered = false;
	}

	void StopShooting() /// stop shooting
	{
		StopCoroutine(shootRoutine);
	}

	void PlayShootSFX()
	{
		if(soundManager.canPlaySFX == false)
			return;
		
		audioSource.Play();
	}

	protected virtual void KillUnit (int y)
	{
		Instantiate(money , new Vector3(this.transform.position.x , this.transform.position.y - 0.25f ),
				 		    Quaternion.identity);
		gm.EnemyStatePos(y, false);
		Destroy(this.gameObject);
	}
}
