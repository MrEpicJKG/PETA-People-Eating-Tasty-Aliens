using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
	#region Serialized Private Vars
	[Header("Level Select")]
	[SerializeField] private GameObject statsPanel;
	[SerializeField] private GameObject levelSelectPanel;
	[SerializeField] private Text totAliensCollectedText;
	[SerializeField] private Text greenAliensCollectedText;
	[SerializeField] private Text blueAliensCollectedText;
	[SerializeField] private Text redAliensCollectedText;
	[SerializeField] private Text totalScoreReachedText;
	[SerializeField] private Image lvl2LockIcon;
	[SerializeField] private Image lvl3LockIcon;
	[SerializeField] private Image missionL2_1Check;
	[SerializeField] private Image missionL2_2Check;
	[SerializeField] private Image missionL3_1Check;
	[SerializeField] private Image missionL3_2Check;
	[SerializeField] private Button lvl2Btn;
	[SerializeField] private Button lvl3Btn;

	[Header("In-Game")]
	[SerializeField] private GameObject inGamePanel;
	[SerializeField] private Text aliensCollectedText;
	[SerializeField] private Text scoreText;
	[SerializeField] private Text multiplierText;
	[SerializeField] private Text mission1ValueText;
	[SerializeField] private Text mission2ValueText;
	[SerializeField] private Slider healthSlider;
	[SerializeField] private Slider timeSlider;

	[Header("In-Game (Paused)")]
	[SerializeField] private GameObject pausePanel;
	[SerializeField] private GameObject confirmPanel;

 	[Header("In-Game (Round Over)")]
	[SerializeField] private GameObject roundOverPanel;
	[SerializeField] private Text levelScoreText;
	[SerializeField] private Text levelMultText;
	[SerializeField] private Text timeSurvivedText;
	[SerializeField] private Text finalScoreText;
	[SerializeField] private Text greenCollectedText;
	[SerializeField] private Text blueCollectedText;
	[SerializeField] private Text redCollectedText;

	#endregion
	#region Private Vars
	private GameManager manager;
	private PlayerController playerControl;
	private SavedDataManager SDM;
	private int prevHealthValue = 100;
	private float prevTimeValue = 100;
	private float prevMission1Value = 0;
	private float prevMission2Value = 0;
	private int prevAliensCollectedValue = 0;
	private int prevScoreValue = 0;
	private float prevMultiplierValue = 1;
	private bool isPaused = false;

	#endregion
	// Start is called before the first frame update
	[HideInInspector] public void Start()
    {
		GameObject persistObj = GameObject.Find("PersistentObject");
		if (persistObj == null)
		{ persistObj = GameObject.Find("PersistentObject(Clone)"); }
		if (persistObj == null)
		{ Debug.LogError("Unable to find Persistent Object!!"); }
		else
		{ SDM = persistObj.GetComponent<SavedDataManager>(); }


		manager = GameObject.Find("GameController").GetComponent<GameManager>();
		if (manager.gameState == GameManager.GameState.InGame)
		{
			inGamePanel.SetActive(true);
			roundOverPanel.SetActive(false);
			pausePanel.SetActive(false);
			playerControl = GameObject.Find("Player").GetComponent<PlayerController>();
			timeSlider.maxValue = manager.levelDurationSecs;
			timeSlider.value = manager.levelDurationSecs;
			healthSlider.maxValue = playerControl.maxHealth;
			healthSlider.value = playerControl.currHealth;
			if (manager.currentLevel == 1)
			{
				mission1ValueText.text = SDM.totalAliensCollected.ToString() + "/50";
				mission2ValueText.text = manager.currentScoreMult.ToString() + "x/5x";
			}
			else if (manager.currentLevel == 2)
			{
				mission1ValueText.text = manager.blueAliensCollected.ToString() + "/10";
				mission2ValueText.text = SDM.totalScore.ToString() + "/3000";
			}
			else if (manager.currentLevel == 3)
			{
				mission1ValueText.text = "";
				mission2ValueText.text = "";
			}
		}
		else if(manager.gameState == GameManager.GameState.LevelSelectScreen)
		{
			if(TestLevelUnlocked(2) == true)
			{
				lvl2Btn.interactable = true;
				lvl2LockIcon.enabled = false;
			}

			if(TestLevelUnlocked(3) == true)
			{
				lvl3Btn.interactable = true;
				lvl3LockIcon.enabled = false;
			}

			totAliensCollectedText.text = SDM.totalAliensCollected.ToString();
			greenAliensCollectedText.text = SDM.totalGreenAliensCollected.ToString();
			blueAliensCollectedText.text = SDM.totalBlueAliensCollected.ToString();
			redAliensCollectedText.text = SDM.totalRedAliensCollected.ToString();
			totalScoreReachedText.text = SDM.totalScore.ToString();
		}
	}

	// Update is called once per frame
	void Update()
	{
		//All these ifs prevent the UI from being redrawn every frame and mean that only the things that change get redrawn.
		if (manager.gameState == GameManager.GameState.InGame)
		{
			if (manager.secsLeftInLevel != prevTimeValue)
			{ timeSlider.value = manager.secsLeftInLevel; }
			
			if(manager.currentScore != prevScoreValue)
			{ scoreText.text = manager.currentScore.ToString(); }
			
			if(manager.currentScoreMult != prevMultiplierValue)
			{ multiplierText.text = "x" + manager.currentScoreMult.ToString(); }

			if(playerControl.currHealth != prevHealthValue)
			{ healthSlider.value = playerControl.currHealth; }

			if(prevAliensCollectedValue != manager.greenAliensCollected + manager.blueAliensCollected + manager.redAliensCollected)
			{ aliensCollectedText.text = (manager.greenAliensCollected + manager.blueAliensCollected + manager.redAliensCollected).ToString(); }

			if (manager.currentLevel == 1)
			{
				if (prevMission1Value != SDM.totalAliensCollected)
				{ mission1ValueText.text = SDM.totalAliensCollected.ToString() + "/50"; }
				
				if(prevMission2Value != manager.currentScoreMult)
				{ mission2ValueText.text = manager.currentScoreMult.ToString() + "/5"; }
			}

			if (manager.currentLevel == 2)
			{
				if (prevMission1Value != manager.blueAliensCollected)
				{ mission1ValueText.text = manager.blueAliensCollected.ToString() + "/10"; }

				if (prevMission2Value != manager.currentScore)
				{ mission2ValueText.text = manager.currentScore.ToString() + "/3000"; }
			}

			prevTimeValue = manager.secsLeftInLevel;
			prevScoreValue = manager.currentScore;
			prevMultiplierValue = manager.currentScoreMult;
			prevHealthValue = playerControl.currHealth;
			prevAliensCollectedValue = manager.greenAliensCollected + manager.blueAliensCollected + manager.redAliensCollected;
			if (manager.currentLevel == 1)
			{
				prevMission1Value = SDM.totalAliensCollected;
				prevMission2Value = prevMultiplierValue;
			}
			else if (manager.currentLevel == 2)
			{
				prevMission1Value = manager.blueAliensCollected;
				prevMission2Value = prevScoreValue;
			}
		}
    }

	#region Button Events
	public void OnBackTap()
	{
		manager.audioControl.PlaySFX("click");
		if (manager.gameState == GameManager.GameState.InGame)
		{
			if (isPaused == true)
			{
				Time.timeScale = 1;
				pausePanel.SetActive(false);
				inGamePanel.SetActive(true);
				isPaused = false;
				manager.isGameRunning = true;
			}
		}
		else if(manager.gameState == GameManager.GameState.LevelSelectScreen)
		{
			if(statsPanel.activeSelf == true)
			{
				levelSelectPanel.SetActive(true);
				statsPanel.SetActive(false);
			}
			else
			{ 
				SceneManager.LoadScene("MainMenuScene");
				manager.audioControl.StopSFX();
			}
		}
	}

	public void OnMenuTap()
	{
		manager.audioControl.PlaySFX("click");
		pausePanel.SetActive(false);
		confirmPanel.SetActive(true);
	}

	public void OnYesTap()
	{
		manager.audioControl.PlaySFX("click");
		SceneManager.LoadScene("LevelSelect");
		manager.audioControl.StopSFX();
	}

	public void OnNoTap()
	{
		manager.audioControl.PlaySFX("click");
		confirmPanel.SetActive(false);
		pausePanel.SetActive(true);
	}

	public void OnContinueTap()
	{
		manager.audioControl.PlaySFX("click");
		if (manager.gameState == GameManager.GameState.UpgradeScreen)
		{
			manager.audioControl.StopSFX();
			SceneManager.LoadScene("LevelSelect");
		}
		else if(manager.gameState == GameManager.GameState.RoundOver)
		{
			SceneManager.LoadScene("LevelSelect");
			manager.audioControl.StopSFX();
		} //normally we would return to the upgrades screen here, but since that is still a WIP, we will just go to level select instead
	}

	public void OnStatsTap()
	{
		manager.audioControl.PlaySFX("click");
		levelSelectPanel.SetActive(false);
		statsPanel.SetActive(true);
		totAliensCollectedText.text = SDM.totalAliensCollected.ToString();
		redAliensCollectedText.text = SDM.totalRedAliensCollected.ToString();
		greenAliensCollectedText.text = SDM.totalGreenAliensCollected.ToString();
		blueAliensCollectedText.text = SDM.totalBlueAliensCollected.ToString();
		totalScoreReachedText.text = SDM.totalScore.ToString();
	}

	public void OnLvl1Tap()
	{
		manager.audioControl.PlaySFX("click"); 
		SceneManager.LoadScene("Level1Scene");
		manager.audioControl.StopSFX();
	}

	public void OnLvl2Tap()
	{
		manager.audioControl.PlaySFX("click"); 
		SceneManager.LoadScene("Level2Scene");
		manager.audioControl.StopSFX();
	}

	public void OnLvl3Tap()
	{ 
		SceneManager.LoadScene("Level3Scene");
		manager.audioControl.PlaySFX("click");
		manager.audioControl.StopSFX();
	}

	public void OnPauseTap()
	{
		manager.audioControl.PlaySFX("click");
		Time.timeScale = 0;
		inGamePanel.SetActive(false);
		pausePanel.SetActive(true);
		isPaused = true;
		manager.isGameRunning = false;
	}

	#endregion

	[HideInInspector] public void ShowRoundOverPanel()
	{
		manager.gameState = GameManager.GameState.RoundOver;
		roundOverPanel.SetActive(true);
		inGamePanel.SetActive(false);
		pausePanel.SetActive(false);
		manager.isGameRunning = false;

		levelScoreText.text = manager.currentScore.ToString();
		levelMultText.text = manager.currentScoreMult.ToString();
		timeSurvivedText.text = (manager.levelDurationSecs - manager.secsLeftInLevel).ToString();
		float finalScore = (manager.currentScore * manager.currentScoreMult) + (manager.levelDurationSecs - manager.secsLeftInLevel); //<= Final Score Equation
		finalScoreText.text = Mathf.Round(finalScore).ToString();
		greenCollectedText.text = manager.greenAliensCollected.ToString();
		blueCollectedText.text = manager.blueAliensCollected.ToString();
		redCollectedText.text = manager.redAliensCollected.ToString();
		//manager.StoreRoundDataInPlayerPrefs();
	}

	private bool TestLevelUnlocked(int lvlToTest)
	{
		bool isUnlocked = false;
		bool mission1Done = false;
		bool mission2Done = false;
		if (lvlToTest == 2)
		{
			if (SDM.totalAliensCollected >= 50)
			{
				mission1Done = true;
				missionL2_1Check.enabled = true;
			}
			else
			{ missionL2_1Check.enabled = false; }

			if (SDM.highestL1Mult >= 5)
			{
				mission2Done = true;
				missionL2_2Check.enabled = true;
			}
			else
			{ missionL2_2Check.enabled = false; }
		}
		else if(lvlToTest == 3)
		{
			if (SDM.highestL2BlueCollected >= 10)
			{
				mission1Done = true;
				missionL3_1Check.enabled = true;
			}
			else
			{ missionL3_1Check.enabled = false; }

			if (SDM.highestL2Score >= 3000)
			{
				mission2Done = true;
				missionL3_2Check.enabled = true;
			}
			else
			{ missionL3_2Check.enabled = false; }
		}
		if(mission1Done == true && mission2Done == true)
		{ isUnlocked = true; }

		return isUnlocked;
	}
}
