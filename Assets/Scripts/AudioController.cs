using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
	[SerializeField] private AudioClip music;
	[SerializeField] private AudioClip abductSFX, explodeSFX, clickSFX, shootSFX, impactSFX;
	[HideInInspector] public AudioSource musicSource, sfxSource;
	
    // Start is called before the first frame update
    void Start()
    {
		musicSource = transform.GetChild(0).GetComponent<AudioSource>();
		sfxSource = transform.GetChild(1).GetComponent<AudioSource>();
		musicSource.Play();
    }

	public void PlaySFX(string sfxID) //the ids are the same as the text before "SFX" in the variable names. e.g. the id for dieSFX, is "die".
	{
		if (sfxID == "abduct")
		{
			sfxSource.clip = abductSFX;
			sfxSource.Play();
			sfxSource.loop = true;
		}
		else if (sfxID == "explode")
		{ sfxSource.PlayOneShot(explodeSFX); }
		else if (sfxID == "click")
		{ sfxSource.PlayOneShot(clickSFX); }
		else if (sfxID == "shoot")
		{ sfxSource.PlayOneShot(shootSFX); }
		else if (sfxID == "impact")
		{ sfxSource.PlayOneShot(impactSFX); }
		else
		{ Debug.LogWarning("Invalid Sound FX ID!!"); }

	}

	public void StopSFX()
	{ sfxSource.Stop(); }

}
