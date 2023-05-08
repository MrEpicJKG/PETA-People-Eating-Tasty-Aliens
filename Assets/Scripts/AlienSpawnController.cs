using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienSpawnController : MonoBehaviour
{
	//the 3 floats below have the HideInInspector attribute because they are drawn in the inspector from the Custom editor I made, and I don't want them to be drawn twice. (once from this, and once from the custom editor)
	[HideInInspector] [Range(0, 100)] [SerializeField]
	private float greenAlienSpawnChance = 33.34f;
	[HideInInspector] [Range(0, 100)] [SerializeField] 
	private float blueAlienSpawnChance = 33.33f;
	[HideInInspector] [Range(0, 100)] [SerializeField]
	private float redAlienSpawnChance = 33.33f;
	[SerializeField] private int totalAliensToSpawn = 20;
	[SerializeField] private int minGreenAliensToSpawn = 5;
	[SerializeField] private int minBlueAliensToSpawn = 4;
	[SerializeField] private int minRedAliensToSpawn = 3;
	[SerializeField] [Range(0, 0.25f)] private float spawnDelayRandomizationFactor = 0.1f;
	[SerializeField] private GameObject greenAlien;
	[SerializeField] private GameObject blueAlien;
	[SerializeField] private GameObject redAlien;
	[SerializeField] private Transform[] spawnLocations;
	[SerializeField] private GameObject alienParent;

	private float levelDuration = 1;
	private float delayBetweenSpawnSecs = 2;
	private GameManager manager;
	private bool isInMinSpawnMode = true; //the spawner starts in "MinSpawnMode" which it will stay in until the min number of aliens of each color have been spawned. after that, it will switch to random mode, where the aliens spawned will be random
	private int totalAliensSpawned = 0;
	private int greenAliensSpawned = 0;
	private int blueAliensSpawned = 0;
	private int redAliensSpawned = 0;
	private float spawnPeriod = 1;

    // Start is called before the first frame update
    void Start()
    {
		manager = GameObject.Find("GameController").GetComponent<GameManager>();
		levelDuration = manager.levelDurationSecs;
		spawnPeriod = levelDuration * 0.75f;
		delayBetweenSpawnSecs = spawnPeriod / totalAliensToSpawn;
		Invoke("SpawnNewAlien", GetNewSpawnDelay());
    }

	private void SpawnNewAlien()
	{
		int alienToSpawn = PickAlienToSpawn();
		Vector2 spawnLocation = spawnLocations[Random.Range(0, spawnLocations.Length)].position;
		GameObject newAlien;
		GameObject alienToSpawnGO;
		if(alienToSpawn == 0)
		{
			alienToSpawnGO = greenAlien;
			greenAliensSpawned++;
			totalAliensSpawned++;
		}
		else if(alienToSpawn == 1)
		{
			alienToSpawnGO = blueAlien;
			blueAliensSpawned++;
			totalAliensSpawned++;
		}
		else
		{ 
			alienToSpawnGO = redAlien;
			redAliensSpawned++;
			totalAliensSpawned++;
		}
		newAlien = Instantiate(alienToSpawnGO, spawnLocation, Quaternion.identity, alienParent.transform);
		newAlien.GetComponent<SimpleAlienController>().wayPoints = spawnLocations;
		if(totalAliensSpawned < totalAliensToSpawn && manager.gameState == GameManager.GameState.InGame)
		{ Invoke("SpawnNewAlien", GetNewSpawnDelay()); }

	}

	private int PickAlienToSpawn()
	{
		int alienToSpawn = -1; //return value of 0 = green, 1 = blue, 2 = red
		if (isInMinSpawnMode == true & (greenAliensSpawned >= minGreenAliensToSpawn && blueAliensSpawned >= minBlueAliensToSpawn && redAliensSpawned >= minRedAliensToSpawn))
		{ isInMinSpawnMode = false; }
		
		if(isInMinSpawnMode == true)
		{
			int numIterations = 0;
			do
			{
				if (numIterations < 40)
				{
					numIterations++;
					float value = Random.Range(0, 100);
					if (value <= greenAlienSpawnChance)
					{ alienToSpawn = 0; }
					else if (value <= greenAlienSpawnChance + blueAlienSpawnChance)
					{ alienToSpawn = 1; }
					else
					{ alienToSpawn = 2; }
				}
				else
				{
					alienToSpawn = 0;
					Debug.LogWarning("The do-while loop in the PickAlienToSpawn function of AlienSpawnController overran its iteration cap. Alien Type defaulted to green!!");
					break;
				}
			}
			while (TestCanSpawnColor(alienToSpawn) == false);
		}
		else
		{ alienToSpawn = Random.Range(0, 3); }

		return alienToSpawn;
	}
	private float GetNewSpawnDelay()
	{
		float delayRandFactor = Random.Range(-spawnDelayRandomizationFactor, spawnDelayRandomizationFactor);
		float spawnDelay = delayBetweenSpawnSecs + (delayBetweenSpawnSecs * delayRandFactor);
		return spawnDelay;
	}
	private bool TestCanSpawnColor(int colorID)
	{
		bool canSpawn = false;
		if(colorID == 0 & greenAliensSpawned < minGreenAliensToSpawn) //green
		{ canSpawn = true; }
		else if(colorID == 1 & blueAliensSpawned < minBlueAliensToSpawn) //blue
		{ canSpawn = true; }
		else if(colorID == 2 & redAliensSpawned < minRedAliensToSpawn) //red
		{ canSpawn = true; }
		else
		{ }
		return canSpawn;
	}

}
