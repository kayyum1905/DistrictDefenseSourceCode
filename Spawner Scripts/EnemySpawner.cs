using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour 
{
	public GameObject[] enemy;
	public GameObject manager;
	public GameObject lm;

	public int enemyLimit; // the amount of enemy units that is going to be spawned

	float timeMin;
	float timeMax;

	int[] enemyTypeLimit = {3, 1, 1, 1, 2, 2, 2, 2, 3}; // they size of enemy type at each single level
	float[] yPos = {0.7f , -0.3f , -1.3f , -2.3f , -3.3f}; // Y positions enemy can be spawned randomly

	int spawnRow; // to prevent spawning enemy in the same row again
	int enemySpawned; // after certain enemy amount spawned the time will be lower playing will be harder

	GameState gameState;
	LevelManager levelManager;

	void Start ()
	{
		gameState = manager.GetComponent<GameState>();
		levelManager = lm.GetComponent<LevelManager>();

		int x = gameState.currentLevel;

		// [0] element is for survival mode
		// the array of enemy numbers to be spawned according to the current level
		int[] wave = {100, 12, 16, 20, 24, 27, 30, 34, 37, 40, 42}; 

		float[] minTimeArray = {7.2f, 6.95f, 6.75f, 6.35f, 6.05f, 5.75f, 5.55f, 5.45f, 5.35f, 5.25f, 5.2f};
		float[] maxTimeArray = {9.6f, 9.3f, 9.1f, 8.7f, 8.4f, 8.2f, 8.1f, 8.1f, 7.95f, 7.9f, 7.85f};

		spawnRow = -1;
		enemySpawned = 0;

		if(gameState.currentLevel > 10)
		{
			enemyLimit = wave[10]; 
			timeMin = minTimeArray[10];
			timeMax = maxTimeArray[10];
			return;
		}
		//else:
		enemyLimit = wave[x];
		timeMin = minTimeArray[x];
		timeMax = maxTimeArray[x];
	}

	// called by the members panel when start game is selected
	public void StartWave () 
	{
		InvokeRepeating( "SpawnEnemy" , 1.5f , Random.Range(timeMin , timeMax));
	}

	//stop wave when gameover
	public void StopWave()
	{
		CancelInvoke();
	}

	// invoke cancel and reinvoke spawning enemy within lower time limits
	void ReInvokeSpawning()
	{
		CancelInvoke();
		InvokeRepeating( "SpawnEnemy" , Random.Range(timeMin , timeMax) , Random.Range(timeMin , timeMax));
	}

	void SpawnEnemy () 
	{
		int e = 0;

		if(gameState.currentLevel > 8)
			e = enemyTypeLimit[8];
		else
			e = enemyTypeLimit[gameState.currentLevel];

		int x = Random.Range(0 , e); // the size of enemy type - limit
		int y = RandomY();
		spawnRow = y;

		if(x > enemy.Length)
			x = enemy.Length;

		if(enemyLimit > 0)
		{
			if(gameState.currentLevel == 0 && enemyLimit < 10)
			{
				enemyLimit += 100;
			}

			//this is to mark the last enemy so that when it no longer exists: level up!
			if(enemyLimit == 1)
			{
				GameObject lastEnemy = (GameObject)Instantiate(enemy[x] , new Vector2 ( 10f , yPos[y]) , Quaternion.identity);
				enemyLimit -= 1;
				StartCoroutine(CheckLastEnemy(lastEnemy));
				return; // code below will be skipped
			}

			Instantiate(enemy[x] , new Vector2 ( 10f , yPos[y]) , Quaternion.identity);
			enemySpawned++;
			enemyLimit -= 1;

			if((GameState.money > 125) && (LevelManager.unitNo > 10 && LevelManager.unitNo <= 18))
			{
				int l = RandomY();
				spawnRow = l;
				Instantiate(enemy[Random.Range(0 , e)] , new Vector2 ( 10f , yPos[l]) , Quaternion.identity);
				enemySpawned++;
				//enemyLimit -= 1;
				print("extra spawn enemy +1");
			}
			else if(GameState.money > 125 && LevelManager.unitNo > 18)
			{
				int l = RandomY();
				spawnRow = l;
				Instantiate(enemy[Random.Range(0 , e)] , new Vector2 ( 10f , yPos[l]) , Quaternion.identity);

				int m = RandomY();
				spawnRow = m;
				Instantiate(enemy[Random.Range(0 , e)] , new Vector2 ( 10f , yPos[m]) , Quaternion.identity);

				enemySpawned += 2;
				//enemyLimit -= 2;
				print("extra spawn enemy +2");
			}
		}

		// For every 4 enemy spawned decrease the duration between enemy spawns
		if(enemySpawned % 4 == 0)
		{
			timeMin -= .07f;
			timeMax -= .11f;

			if(timeMin < 2.25f)
			{
				timeMin = 2.25f;
				print("timemin set to: " + timeMin);
			}

			if(timeMax < 3.47f)
			{
				timeMax = 3.47f;
				print("timemax set to:" + timeMax);
			}

			ReInvokeSpawning();

			print("update min enemy spawn time: " + timeMin + "update max enemy spawn time: " + timeMax);
		}
	}

	// This function is to set the Y pos of enemy
	int RandomY()
	{
		int y = Random.Range(0 , yPos.Length);

		if(y == spawnRow)
		{
			y = RandomY();
		}
		return y;
	}

	// when the last enemy unit is spawned
	IEnumerator CheckLastEnemy(GameObject le)
	{
		while(le != null)
		{
			yield return new WaitForSeconds(0.5f);

			if(le == null)
			{
				// last enemy dead may not mean there is no enemy left
				le = GameObject.FindWithTag("Enemy"); // check if any other enemy unit remains

				if(le == null)
				{
					CancelInvoke(); // To cancel calling this method
					gameState.UpdateLevel();
					levelManager.SelectScene(1);
				}
			}
		}
	}
}
