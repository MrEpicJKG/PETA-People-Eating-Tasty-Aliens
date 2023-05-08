using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;
public class AdsManager : MonoBehaviour, IUnityAdsListener
{
	[SerializeField] private Button watchAdButton;

	// Start is called before the first frame update
	void Start()
	{
		watchAdButton.interactable = false;
		Advertisement.Initialize("3711352");
		Advertisement.AddListener(this);
	}

	// Update is called once per frame
	//void Update()
	//{

	//}

	public void OnUnityAdsDidError(string message)
	{
		print("Something went wrong with the Ad!!");
	}

	public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
	{
		watchAdButton.interactable = true;
		switch(showResult)
		{
			case ShowResult.Finished:
			{
				print("Ad Completed Successfully");
				break;
			}
			case ShowResult.Skipped:
			{
				print("Ad Skipped");
				break;
			}
			case ShowResult.Failed:
			{
				print("Ad Failed!!");
				break;
			}
		}
	}

	public void OnUnityAdsDidStart(string placementId)
	{
		watchAdButton.interactable = false;
	}

	public void OnUnityAdsReady(string placementId)
	{
		Debug.Log("Ad is ready");
		watchAdButton.interactable = true;
	}

	public void OnAdButtonTap()
	{
		Advertisement.Show("video");
	}
}
