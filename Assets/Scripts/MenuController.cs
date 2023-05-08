using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
	
	[SerializeField] private GameObject menuPanel, optionsPanel, creditsPanel, newGameConfirmationPanel, howToPanel, musicLicensingPanel;//, whiteFadePanel;
	[SerializeField] private Slider musicSlider, sfxSlider, qualitySlider;
	[SerializeField] private AudioSource musicAudioSource, sfxAudioSource;
	[SerializeField] private Button continueGameButton;
	[SerializeField] private PersistentObjectManager POM;
	//[SerializeField] private float transitionDurationSeconds = 0.75f;
	//[SerializeField] private float alphaDeltaPerChange = 0.05f;

	//private bool isFading = false; //is the white transition panel fading at all?
	//private bool isFadingToWhite = false; //ingored if above is false. if this is true, the panel is fading from clear to white, else it is fading from white back to clear.
	//private float delayBetweenFadeChanges = 0;
	//private int changesPerHalfTransition = 0;

	private AudioController audioControl;
	private void Start()
	{
		audioControl = GameObject.Find("PersistentObject").GetComponent<AudioController>();
		menuPanel.SetActive(true);
		optionsPanel.SetActive(false);
		creditsPanel.SetActive(false);
		howToPanel.SetActive(false);
		newGameConfirmationPanel.SetActive(false);
		musicLicensingPanel.SetActive(false);

		#region PlayerPref Setup
		if (PlayerPrefs.HasKey("MusicVolume") == true)
		{ musicAudioSource.volume = PlayerPrefs.GetFloat("MusicVolume"); }
		else
		{ PlayerPrefs.SetFloat("MusicVolume", musicAudioSource.volume); }

		if (PlayerPrefs.HasKey("SFXVolume") == true)
		{ sfxAudioSource.volume = PlayerPrefs.GetFloat("SFXVolume"); }
		else
		{ PlayerPrefs.SetFloat("SFXVolume", sfxAudioSource.volume); }

		if (PlayerPrefs.HasKey("QualityLevel") == true)
		{ QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("QualityLevel")); }
		else
		{ PlayerPrefs.SetInt("QualityLevel", QualitySettings.GetQualityLevel()); }

		//----- UNCOMMENT THIS ONCE CONTINUE GAME FEATURE IS ADDED -----
		//if (PlayerPrefs.HasKey("HasSavedGame") == true) //this is effectivly a bool value that will rarely be changed. Due to this, I'm simply using the existence of the PlayerPrefs key to represent the value.
		//												//if it exists, its true, otherwise, its false.
		//{ continueGameButton.interactable = true; }
		//else
		//{ continueGameButton.interactable = false; }
		#endregion
	}

	#region Events
	public void OnReturnClick()
	{
		audioControl.PlaySFX("click");
		if (optionsPanel.activeSelf == true)
		{
			PlayerPrefs.SetFloat("MusicVolume", musicAudioSource.volume);
			PlayerPrefs.SetFloat("SFXVolume", sfxAudioSource.volume);
			PlayerPrefs.SetInt("QualityLevel", QualitySettings.GetQualityLevel());
		}
		menuPanel.SetActive(true);
		optionsPanel.SetActive(false);
		creditsPanel.SetActive(false);
		howToPanel.SetActive(false);
		newGameConfirmationPanel.SetActive(false);
		musicLicensingPanel.SetActive(false);
	}

	public void OnOptionsClick()
	{
		audioControl.PlaySFX("click");
		menuPanel.SetActive(false);
		optionsPanel.SetActive(true);
		creditsPanel.SetActive(false);
		howToPanel.SetActive(false);
		newGameConfirmationPanel.SetActive(false);
		Invoke("SetupSliders", 0.01f); //set up the slider values 0.01 seconds after making the panels appear, thus ensuring that the panels and sliders are initialized before trying to change things on them.
	}

	public void OnCreditsClick()
	{
		audioControl.PlaySFX("click");
		menuPanel.SetActive(false);
		optionsPanel.SetActive(false);
		creditsPanel.SetActive(true);
		howToPanel.SetActive(false);
		newGameConfirmationPanel.SetActive(false);
		creditsPanel.GetComponent<CreditController>().vertScrollBar.value = 1;
	}

	public void OnHowToClick()
	{
		audioControl.PlaySFX("click");
		menuPanel.SetActive(false);
		optionsPanel.SetActive(false);
		creditsPanel.SetActive(false);
		howToPanel.SetActive(true);
		newGameConfirmationPanel.SetActive(false);
	}

	public void OnNewGameClick()
	{
		audioControl.PlaySFX("click");
		//menuPanel.SetActive(false);
		//optionsPanel.SetActive(false);
		//creditsPanel.SetActive(false);
		//newGameConfirmationPanel.SetActive(true);
		SceneManager.LoadScene("LevelSelect"); //normally we would go to the Upgrade scene here, but since that is still a WIP, we will just go to level select instead.
		audioControl.StopSFX();
	}

	public void OnContinueGameClick()
	{
		audioControl.PlaySFX("click");
		//Load saved game from here
		PlayerPrefs.Save();
	}

	public void OnConfirmStartNewGameClick()
	{
		audioControl.PlaySFX("click");
		if (PlayerPrefs.HasKey("HasSavedGame") == false)
		{ PlayerPrefs.SetString("HasSavedGame", ""); }
		//The string value is empty since the existence of the PlayerPrefs key is what represents the bool value. I did it this way to minimize memory usage.

		PlayerPrefs.Save();
		SceneManager.LoadScene("UpgradeScene");
	}

	public void OnMusicLicensingClick()
	{
		audioControl.PlaySFX("click");
		menuPanel.SetActive(false);
		optionsPanel.SetActive(false);
		creditsPanel.SetActive(false);
		howToPanel.SetActive(false);
		newGameConfirmationPanel.SetActive(false);
		musicLicensingPanel.SetActive(true);
	}

	public void OnMobileInputToggleValueChanged(bool value)
	{
		audioControl.PlaySFX("click");
		if (value == true)
		{
			PlayerPrefs.SetString("MobileInputEnabled", "T");
			UnityStandardAssets.CrossPlatformInput.CrossPlatformInputManager.SwitchActiveInputMethod(UnityStandardAssets.CrossPlatformInput.CrossPlatformInputManager.ActiveInputMethod.Touch);
			POM.mobileInputEnabled = true;
		}
		else
		{
			PlayerPrefs.SetString("MobileInputEnabled", "F");
			UnityStandardAssets.CrossPlatformInput.CrossPlatformInputManager.SwitchActiveInputMethod(UnityStandardAssets.CrossPlatformInput.CrossPlatformInputManager.ActiveInputMethod.Hardware);
			POM.mobileInputEnabled = false;
		}
	}

	#endregion

	//private void StartFade(GameObject currentPanel, GameObject newPanel)
	//{

	//}
	//private void DoFadeToWhite()
	//{

	//}
	//private void DoFadeToClear()
	//{

	//}

	private void SetupSliders()
	{
		musicSlider.value = (int)(audioControl.musicSource.volume * 100);
		sfxSlider.value = (int)(audioControl.sfxSource.volume * 100);
		qualitySlider.value = PlayerPrefs.GetInt("QualityLevel");
	}
}
