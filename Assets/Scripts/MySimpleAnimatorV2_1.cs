using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;

//I know I could (and perhaps should) have used "getters & setters" here, instead of functions, but I'm more familiar with functions so that was just a faster way to do it. 

[RequireComponent(typeof(SpriteRenderer))]
public class MySimpleAnimatorV2_1 : MonoBehaviour
{
	public List<Sprite> animFrameSprites;
	public float secsBetweenFrameChange = 0.1f;
	[SerializeField] private bool playOnStart = true;
	[SerializeField] private bool useRandomStartingFrame = false;
	[SerializeField] private bool shouldLoop = true; 
	[SerializeField] private bool playReversed = false;

	[HideInInspector] public int currFrameNum = 0;
	[HideInInspector] public bool isOnFirstFrame = false;
	[HideInInspector] public bool isRunning = false;

	private SpriteRenderer rend;

	private GameManager manager;
	
	void Awake()
    {
		rend = GetComponent<SpriteRenderer>();
		manager = GameObject.Find("GameController").GetComponent<GameManager>();
        if(animFrameSprites.Count == 0)
		{ 
			Debug.LogWarning("The MySimpleAnimator on " + this.gameObject.name + "has no sprites assigned to it!!");
			return;
		}

		if (useRandomStartingFrame == true)
		{ currFrameNum = Random.Range(0, animFrameSprites.Count); }

		if (playOnStart == true)
		{ PlayAnim(); }
    }

	//private void Update()
	//{
	//	//if(manager.isGameRunning == true && IsInvoking("ShowNextFrame") == false && shouldLoop == true)
	//	//{ Invoke("ShowNextFrame", secsBetweenFrameChange); }
	//	//else if(manager.isGameRunning == false && IsInvoking("ShowNextFrame") == true)
	//	//{ CancelInvoke("ShowNextFrame"); }
		
	//}

	private void ShowNextFrame()
	{
		if (manager.isGameRunning == true)
		{
			if (playReversed == false)
			{
				if (currFrameNum < animFrameSprites.Count - 1) //if not at the end of the array of sprites
				{ 
					currFrameNum++;	//increment the var controlling which sprite to show
					Invoke("ShowNextFrame", secsBetweenFrameChange); //Invoke this function again

					if(isOnFirstFrame == true)
					{ isOnFirstFrame = false; }
				}
				else if(shouldLoop == true) //if it IS at the end of the array of sprites and it SHOULD loop
				{ 
					currFrameNum = 0; //loop back to the beginning
					isOnFirstFrame = true;
					Invoke("ShowNextFrame", secsBetweenFrameChange); //Invoke this function again
				}
				else if(IsInvoking("ShowNextFrame") == true) //if it IS at the end of the array of sprites and it SHOULD NOT loop,
				{ StopAnim(); } //stop it from playing
			}
			else //if(playReversed == true)
			{ //do everything above, but with the sprites playing in reversed order
				if (currFrameNum > 0) //if not at the start of the array of sprites
				{
					currFrameNum--; //decrement the var controlling which sprite to show
					Invoke("ShowNextFrame", secsBetweenFrameChange); //Invoke this function again

					if (isOnFirstFrame == true)
					{ isOnFirstFrame = false; }
				}
				else if (shouldLoop == true) //if it IS at the end of the array of sprites and it SHOULD loop
				{
					currFrameNum = animFrameSprites.Count - 1; //loop back to the end of the array of sprites
					isOnFirstFrame = true;
					Invoke("ShowNextFrame", secsBetweenFrameChange); //Invoke this function again
				}
				else if (IsInvoking("ShowNextFrame") == true) //if it IS at the beginning of the array of sprites and it SHOULD NOT loop,
				{ PauseAnim(); }
			}


			rend.sprite = animFrameSprites[currFrameNum]; //update which sprite is showing
		}
	}

	public void PlayAnim()
	{
		isRunning = true;
		ShowNextFrame();
	}

	public void PauseAnim()
	{
		isRunning = false;
		if (IsInvoking("ShowNextFrame") == true)
		{ CancelInvoke("ShowNextFrame"); }
	}

	public void StopAnim()
	{
		isRunning = false;
		if(IsInvoking("ShowNextFrame") == true)
		{ CancelInvoke("ShowNextFrame"); }
		ResetToCycleStart();
	}

	public void SwitchDirection()
	{
		if (playReversed == true)
		{ playReversed = false; }
		else
		{ playReversed = true; }
	}

	public void SetReversed(bool shouldPlayReversed)
	{ playReversed = shouldPlayReversed; }

	public void SetShouldLoop(bool shouldThisLoop)
	{ shouldLoop = shouldThisLoop; }

	public void ResetToCycleStart()
	{
		if(playReversed == false)
		{ currFrameNum = 0; }
		else
		{ currFrameNum = animFrameSprites.Count - 1; }
	}
}
