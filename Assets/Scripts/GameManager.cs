using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class GameManager : MonoBehaviour
{
	///<remarks>
	///PlayerPrefs Keys Used:
	///	MusicVolume				(float)
	///	SFXVolume				(float)
	///	QualityLevel			(int)
	///	HasSavedGame			(string) [used as bool]
	///	MobileInputEnabled		(string) [used as bool]
	///	
	/// UpgradeLevel			(int)
	/// MoneyAmount				(int)
	/// CurrentLevel			(int)
	/// TotalGreenAliensGotten	(int) XX
	/// TotalBlueAliensGotten	(int) XX
	/// TotalRedAliensGotten	(int) XX
	/// CurrentPrimMissionType	(int)
	/// CurrentSecMissionType	(int)
	/// 
	/// MissionL2-1Done			(string) [used as bool]
	/// MissionL2-1Value		(int)
	/// MissionL2-2Done			(string) [used as bool]
	/// MissionL2-2Value		(int)
	/// MissionL3-1Done			(string) [used as bool]
	/// MissionL3-1Value		(int)
	/// MissionL3-2Done			(string) [used as bool]
	/// MissionL3-2Value		(int)
	/// </remarks>

	#region Public Vars

	[Header("General Stuff")]
	public GameState gameState = GameState.InGame;
	public float levelDurationSecs = 60;
	public int currentLevel = 1;
	public GameObject persistObjectPrefab;

	[Header("Alien Data")]
	public int greenAlienValue = 2;
	public int blueAlienValue = 6;
	public int redAlienValue = 10;
	public Transform fleeLeftTrans;
	public Transform fleeRightTrans;
	public List<Sprite> greenAlienIdleFrames;
	public List<Sprite> greenAlienWalkFrames;
	public List<Sprite> greenAlienRunFrames;
	public List<Sprite> blueAlienIdleFrames;
	public List<Sprite> blueAlienWalkFrames;
	public List<Sprite> blueAlienRunFrames;
	public List<Sprite> redAlienIdleFrames;
	public List<Sprite> redAlienWalkFrames;
	public List<Sprite> redAlienRunFrames;
	#endregion
	#region Hidden Public Vars
	//Single Round Game Stats
	[HideInInspector] public float secsLeftInLevel = 1;
	[HideInInspector] public int greenAliensCollected = 0;
	[HideInInspector] public int blueAliensCollected = 0;
	[HideInInspector] public int redAliensCollected = 0;
	[HideInInspector] public float currentScoreMult = 1;
	[HideInInspector] public int currentScore = 0;

	
	
	//Other Stuff
	[HideInInspector] public bool isGameRunning = true; //this will be false if the game is paused
	[HideInInspector] public enum GameState { MainMenu, UpgradeScreen, LevelSelectScreen, InGame, RoundOver };

	[HideInInspector] public int upgradeLevel = 1;
	[HideInInspector] public int moneyAmount = 0;
	[HideInInspector] public AudioController audioControl;

	[Header("Debug Stuff")]
	public bool DB_useDebugMode = false;
	public bool DB_mobileInputEnabled = true;
	[HideInInspector] public bool DB_ClickToCompleteLvl2Miss1 = false;
	[HideInInspector] public bool DB_ClickToCompleteLvl2Miss2 = false;
	[HideInInspector] public bool DB_ClickToCompleteLvl3Miss1 = false;
	[HideInInspector] public bool DB_ClickToCompleteLvl3Miss2 = false;
	#endregion
	#region Private Vars
	private PlayerController playerControl;
	private SavedDataManager SDM;
	private PersistentObjectManager POM;
	 
	#endregion
	// Start is called before the first frame update
	void Start()
	{
		GameObject persistObj = GameObject.Find("PersistentObject");
		if (persistObj == null)
		{ persistObj = GameObject.Find("PersistentObject(Clone)"); }
		if(persistObj == null)
		{ persistObj = Instantiate(persistObjectPrefab); }
		audioControl = persistObj.GetComponent<AudioController>();
		SDM = persistObj.GetComponent<SavedDataManager>();
		POM = persistObj.GetComponent<PersistentObjectManager>();

		//CreateAnyMissingPlayerPrefs();
		//LoadDataFromPlayerPrefs();

		
		if (gameState == GameState.InGame)
		{ 
			playerControl = GameObject.Find("Player").GetComponent<PlayerController>();
			secsLeftInLevel = levelDurationSecs;
			DB_mobileInputEnabled = POM.mobileInputEnabled;
			Invoke("DecrementTime", 1);

		}
		
	}

	//private void CreateAnyMissingPlayerPrefs()
	//{
	//	if (PlayerPrefs.HasKey("UpgradeLevel") == false)
	//	{ PlayerPrefs.GetInt("UpgradeLevel", 1); }

	//	if (PlayerPrefs.HasKey("MoneyAmount") == false)
	//	{ PlayerPrefs.SetInt("MoneyAmount", 0); }

	//	if (PlayerPrefs.HasKey("CurrentLevel") == false)
	//	{ PlayerPrefs.SetInt("CurrentLevel", 1); }

	//	if (PlayerPrefs.HasKey("TotalGreenAliensCollected") == false)
	//	{ PlayerPrefs.SetInt("TotalGreenAliensCollected", totalGreenAliensCollected); }

	//	if (PlayerPrefs.HasKey("TotalBlueAliensCollected") == false)
	//	{ PlayerPrefs.SetInt("TotalBlueAliensCollected", totalBlueAliensCollected); }

	//	if (PlayerPrefs.HasKey("TotalRedAliensCollected") == false)
	//	{ PlayerPrefs.SetInt("TotalRedAliensCollected", totalRedAliensCollected); }

	//	if (PlayerPrefs.HasKey("TotalScore") == false)
	//	{ PlayerPrefs.SetInt("TotalScore", totalScore); }

	//	if (PlayerPrefs.HasKey("HighestL1Multiplier") == false)
	//	{ PlayerPrefs.SetFloat("HighestL1Multiplier", 1); }

	//	if (PlayerPrefs.HasKey("HighestL2Multiplier") == false)
	//	{ PlayerPrefs.SetFloat("HighestL2Multiplier", 1); }

	//	if (PlayerPrefs.HasKey("HighestL3Multiplier") == false)
	//	{ PlayerPrefs.SetFloat("HighestL3Multiplier", 1); }

	//	if (PlayerPrefs.HasKey("HighestL1Score") == false)
	//	{ PlayerPrefs.SetFloat("HighestL1Score", 1); }

	//	if (PlayerPrefs.HasKey("HighestL2Score") == false)
	//	{ PlayerPrefs.SetFloat("HighestL2Score", 1); }

	//	if (PlayerPrefs.HasKey("HighestL3Score") == false)
	//	{ PlayerPrefs.SetFloat("HighestL3Score", 1); }

	//	if (PlayerPrefs.HasKey("HighestL1GreenCollected") == false)
	//	{ PlayerPrefs.SetInt("HighestL1GreenCollected", 0); }

	//	if (PlayerPrefs.HasKey("HighestL1BlueCollected") == false)
	//	{ PlayerPrefs.SetInt("HighestL1BlueCollected", 0); }

	//	if (PlayerPrefs.HasKey("HighestL1RedCollected") == false)
	//	{ PlayerPrefs.SetInt("HighestL1RedCollected", 0); }

	//	if (PlayerPrefs.HasKey("HighestL2GreenCollected") == false)
	//	{ PlayerPrefs.SetInt("HighestL2GreenCollected", 0); }

	//	if (PlayerPrefs.HasKey("HighestL2BlueCollected") == false)
	//	{ PlayerPrefs.SetInt("HighestL2BlueCollected", 0); }

	//	if (PlayerPrefs.HasKey("HighestL2RedCollected") == false)
	//	{ PlayerPrefs.SetInt("HighestL2RedCollected", 0); }

	//	if (PlayerPrefs.HasKey("HighestL3GreenCollected") == false)
	//	{ PlayerPrefs.SetInt("HighestL3GreenCollected", 0); }

	//	if (PlayerPrefs.HasKey("HighestL3BlueCollected") == false)
	//	{ PlayerPrefs.SetInt("HighestL3BlueCollected", 0); }

	//	if (PlayerPrefs.HasKey("HighestL3RedCollected") == false)
	//	{ PlayerPrefs.SetInt("HighestL3RedCollected", 0); }




	//	#region Unused PlayerPrefs
	//	/*All PlayerPrefs below are not currently used. They are for future features.
	//	if (PlayerPrefs.HasKey("CurrentPrimMissionType") == false)
	//	{ PlayerPrefs.SetInt("CurrentPrimMissionType", 0); }

	//	if (PlayerPrefs.HasKey("CurrentSecMissionType") == false)
	//	{ PlayerPrefs.SetInt("CurrentSecMissionType", 0); }



	//	if (PlayerPrefs.HasKey("MissionL2-1Done") == false)
	//	{ PlayerPrefs.SetString("MissionL2-1Done", "F"); }

	//	if (PlayerPrefs.HasKey("MissionL2-1Value") == false)
	//	{ PlayerPrefs.SetInt("MissionL2-1Value", 0); }

	//	if (PlayerPrefs.HasKey("MissionL2-2Done") == false)
	//	{ PlayerPrefs.SetString("MissionL2-2Done", "F"); }

	//	if (PlayerPrefs.HasKey("MissionL2-2Value") == false)
	//	{ PlayerPrefs.SetInt("MissionL2-2Value", 0); }

	//	if (PlayerPrefs.HasKey("MissionL3-1Done") == false)
	//	{ PlayerPrefs.SetString("MissionL3-1Done", "F"); }

	//	if (PlayerPrefs.HasKey("MissionL3-1Value") == false)
	//	{ PlayerPrefs.SetInt("MissionL3-1Value", 0); }

	//	if (PlayerPrefs.HasKey("MissionL3-2Done") == false)
	//	{ PlayerPrefs.SetString("MissionL3-2Done", "F"); }

	//	if (PlayerPrefs.HasKey("MissionL3-2Value") == false)
	//	{ PlayerPrefs.SetInt("MissionL3-2Value", 0); }*/
	//	#endregion
	//}

	//[HideInInspector] public void LoadDataFromPlayerPrefs()
	//{
	//	if (gameState == GameState.InGame || gameState == GameState.UpgradeScreen || gameState == GameState.LevelSelectScreen)
	//	{
	//		upgradeLevel = PlayerPrefs.GetInt("UpgradeLevel");
	//		moneyAmount = PlayerPrefs.GetInt("MoneyAmount");
	//		currentLevel = PlayerPrefs.GetInt("CurrentLevel");
	//		highestL1Mult = PlayerPrefs.GetFloat("HighestL1Multiplier");
	//		highestL1Score = PlayerPrefs.GetFloat("HighestL1Score");
	//		highestL2Mult = PlayerPrefs.GetFloat("HighestL2Multiplier");
	//		highestL2Score = PlayerPrefs.GetFloat("HighestL2Score");
	//		highestL3Mult = PlayerPrefs.GetFloat("HighestL3Multiplier");
	//		highestL3Score = PlayerPrefs.GetFloat("HighestL3Score");
	//		highestL1GreenCollected = PlayerPrefs.GetInt("HighestL1GreenCollected");
	//		highestL1BlueCollected  = PlayerPrefs.GetInt("HighestL1BlueCollected");
	//		highestL1RedCollected   = PlayerPrefs.GetInt("HighestL1RedCollected");
	//		highestL2GreenCollected = PlayerPrefs.GetInt("HighestL2GreenCollected");
	//		highestL2BlueCollected  = PlayerPrefs.GetInt("HighestL2BlueCollected");
	//		highestL2RedCollected   = PlayerPrefs.GetInt("HighestL2RedCollected");
	//		highestL3GreenCollected = PlayerPrefs.GetInt("HighestL3GreenCollected");
	//		highestL3BlueCollected  = PlayerPrefs.GetInt("HighestL3BlueCollected");
	//		highestL3RedCollected   = PlayerPrefs.GetInt("HighestL3RedCollected");

	//		if (gameState == GameState.LevelSelectScreen)
	//		{
	//			totalGreenAliensCollected = PlayerPrefs.GetInt("TotalGreenAliensCollected");
	//			totalBlueAliensCollected = PlayerPrefs.GetInt("TotalBlueAliensCollected");
	//			totalRedAliensCollected = PlayerPrefs.GetInt("TotalRedAliensCollected");
	//			totalAliensCollected = totalGreenAliensCollected + totalBlueAliensCollected + totalRedAliensCollected;
	//		}
	//	}
	//}

	private void SendStatsToPlayer()
	{
		//WIP
	}

	[HideInInspector] public void CollectAlien(int typeID) //Type Id: 0 = Green, 1 = Blue, 2 = Red
	{
		if(typeID == 0) //green
		{
			greenAliensCollected++;
			currentScore += greenAlienValue;
			currentScoreMult += greenAlienValue / 10;
		}
		else if(typeID == 1) //blue
		{
			blueAliensCollected++;
			currentScore += blueAlienValue;
			currentScoreMult += blueAlienValue / 10;
		}
		else if(typeID == 2)
		{
			redAliensCollected++;
			currentScore += redAlienValue;
			currentScoreMult += redAlienValue / 10;
		}
	}

	[HideInInspector]
	public void DecrementTime()
	{
		if (secsLeftInLevel > 0)
		{
			secsLeftInLevel -= 1;
			Invoke("DecrementTime", 1);
		}
		else
		{
			Invoke("DoRoundOver", 0.5f);
			gameState = GameState.RoundOver;
			//StoreRoundDataInPlayerPrefs();

		}
	}

	//[HideInInspector] public void StoreRoundDataInPlayerPrefs()
	//{
	//	if (currentLevel == 1)
	//	{
	//		if (currentScore > highestL1Score)
	//		{ 
	//			highestL1Score = currentScore;
	//			PlayerPrefs.SetFloat("HighestL1Score", currentScore);
	//		}

	//		if(currentScoreMult > highestL1Mult)
	//		{
	//			highestL1Mult = currentScoreMult;
	//			PlayerPrefs.SetFloat("HighestL1Multiplier", currentScoreMult);
	//		}

	//		if(greenAliensCollected > highestL1GreenCollected) //Level 1 Green aliens Collected Highscore
	//		{
	//			highestL1GreenCollected = greenAliensCollected;
	//			PlayerPrefs.SetInt("HighestL1GreenCollected", greenAliensCollected);
	//		}

	//		if (blueAliensCollected > highestL1BlueCollected) //Level 1 Blue Aliens Collected Highscore
	//		{
	//			highestL1BlueCollected = blueAliensCollected;
	//			PlayerPrefs.SetInt("HighestL1BlueCollected", blueAliensCollected);
	//		}

	//		if (redAliensCollected > highestL1RedCollected) //Level 1 Red Aliens Collected Highscore 
	//		{
	//			highestL1RedCollected = redAliensCollected;
	//			PlayerPrefs.SetInt("HighestL1RedCollected", redAliensCollected);
	//		}

			
	//	}
	//	else if(currentLevel == 2)
	//	{
	//		if (currentScore > highestL2Score)
	//		{
	//			highestL2Score = currentScore;
	//			PlayerPrefs.SetFloat("HighestL2Score", currentScore);
	//		}

	//		if (currentScoreMult > highestL2Mult)
	//		{
	//			highestL2Mult = currentScoreMult;
	//			PlayerPrefs.SetFloat("HighestL2Multiplier", currentScoreMult);
	//		}

	//		if (greenAliensCollected > highestL2GreenCollected) //Level 2 Green aliens Collected Highscore
	//		{
	//			highestL2GreenCollected = greenAliensCollected;
	//			PlayerPrefs.SetInt("HighestL2GreenCollected", greenAliensCollected);
	//		}

	//		if (blueAliensCollected > highestL2BlueCollected) //Level 2 Blue Aliens Collected Highscore
	//		{
	//			highestL2BlueCollected = blueAliensCollected;
	//			PlayerPrefs.SetInt("HighestL2BlueCollected", blueAliensCollected);
	//		}

	//		if (redAliensCollected > highestL2RedCollected) //Level 2 Red Aliens Collected Highscore 
	//		{
	//			highestL2RedCollected = redAliensCollected;
	//			PlayerPrefs.SetInt("HighestL2RedCollected", redAliensCollected);
	//		}
	//	}
	//	else if(currentLevel == 3)
	//	{
	//		if (currentScore > highestL3Score)
	//		{
	//			highestL3Score = currentScore;
	//			PlayerPrefs.SetFloat("HighestL3Score", currentScore);
	//		}

	//		if (currentScoreMult > highestL3Mult)
	//		{
	//			highestL3Mult = currentScoreMult;
	//			PlayerPrefs.SetFloat("HighestL3Multiplier", currentScoreMult);
	//		}

	//		if (greenAliensCollected > highestL3GreenCollected) //Level 3 Green aliens Collected Highscore
	//		{
	//			highestL3GreenCollected = greenAliensCollected;
	//			PlayerPrefs.SetInt("HighestL3GreenCollected", greenAliensCollected);
	//		}

	//		if (blueAliensCollected > highestL3BlueCollected) //Level 3 Blue Aliens Collected Highscore
	//		{
	//			highestL3BlueCollected = blueAliensCollected;
	//			PlayerPrefs.SetInt("HighestL3BlueCollected", blueAliensCollected);
	//		}

	//		if (redAliensCollected > highestL3RedCollected) //Level 3 Red Aliens Collected Highscore 
	//		{
	//			highestL3RedCollected = redAliensCollected;
	//			PlayerPrefs.SetInt("HighestL3RedCollected", redAliensCollected);
	//		}
	//	}
	//	PlayerPrefs.SetInt("TotalScore", totalScore + currentScore);
	//	totalScore += currentScore;
	//	PlayerPrefs.Save();
	//}

	private void DoRoundOver()
	{ GetComponent<UIManager>().ShowRoundOverPanel(); }
}