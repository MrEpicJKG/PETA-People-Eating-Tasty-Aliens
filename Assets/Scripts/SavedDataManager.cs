using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using Random = UnityEngine.Random;

//This only manages overall stats that are about the player in general, (e.g. highscores, game progression, etc...)
//It DOES NOT manage in-level, single round stats. (e.g. current level score, health, etc...) Those are in the GameManager.
public class SavedDataManager : MonoBehaviour
{
	#region Serialized Private Vars
	[SerializeField] private string upgradesFilePath;
	[SerializeField] private string savedDataFilePath;
	#endregion

	#region Hidden Public Vars
	//Upgrade Stuff
	[HideInInspector] public List<string> shipNames;
	[HideInInspector] public List<List2D_Float> shipData; //the outer array holds the different levels of the player ship
														  //on the inner array, index 0 = Cost, index 1 = Health Stat, index 2 = Speed Stat, index 3 = Life Force Stat, index 4 = Status Effect Resistance Stat 
	//Overall Game Stats
	[HideInInspector] public int totalGreenAliensCollected = 0;
	[HideInInspector] public int totalBlueAliensCollected = 0;
	[HideInInspector] public int totalRedAliensCollected = 0;
	[HideInInspector] public int totalAliensCollected = 0;
	[HideInInspector] public int totalScore = 0;
	[HideInInspector] public float highestL1Mult = 1;
	[HideInInspector] public float highestL2Mult = 1;
	[HideInInspector] public float highestL3Mult = 1;
	[HideInInspector] public float highestL1Score = 0;
	[HideInInspector] public float highestL2Score = 0;
	[HideInInspector] public float highestL3Score = 0;
	[HideInInspector] public int highestL1GreenCollected = 0;
	[HideInInspector] public int highestL1BlueCollected = 0;
	[HideInInspector] public int highestL1RedCollected = 0;
	[HideInInspector] public int highestL2GreenCollected = 0;
	[HideInInspector] public int highestL2BlueCollected = 0;
	[HideInInspector] public int highestL2RedCollected = 0;
	[HideInInspector] public int highestL3GreenCollected = 0;
	[HideInInspector] public int highestL3BlueCollected = 0;
	[HideInInspector] public int highestL3RedCollected = 0;
	#endregion
	

	// Start is called before the first frame update
	void Start()
    {
		shipNames = new List<string>();
		shipData = new List<List2D_Float>();
		DecodeShipData();
    }

	private void DecodeShipData()
	{
		StreamReader reader = new StreamReader(upgradesFilePath);
		string wholeFile = reader.ReadToEnd();
		reader.Close();

		string[] lines = wholeFile.Split(';');

		for (int i = 0; i < lines.Length - 1; i++)
		{
			if (lines[i].EndsWith(";") == true)
			{ lines[i].Remove(lines[i].Length - 1, 1); } //remove the ending semi-colon from each line
			print(lines[i]);
		}

		for (int i = 0; i < lines.Length - 1; i++)
		{
			if (lines[i].StartsWith("//")) //its a comment, skip it
			{ continue; }
			else if (lines[i].StartsWith("*")) //group starter
			{ shipData.Add(new List2D_Float()); }
			else if(lines[i].StartsWith("~Name")) //its the name of a ship, add it to the list of ship names
			{
				string[] lineParts = lines[i].Split('=');

				shipNames.Add(lineParts[1]);
			}
			else if(lines[i].StartsWith("~")) //its a ship stat, add it to that list.
			{
				string[] lineParts = lines[i].Split('=');
				shipData[shipData.Count - 1].inner.Add(float.Parse(lineParts[1])); //add the stat to the inner list of the last created ship level
			}
		}
	}
	
	private void OnApplicationQuit()
	{
		//save player data here

		PlayerPrefs.Save();
	}

	[HideInInspector] public void StartNewGame()
	{
		
	}


}
