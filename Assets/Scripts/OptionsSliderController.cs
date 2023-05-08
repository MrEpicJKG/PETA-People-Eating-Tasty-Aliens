using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class OptionsSliderController : MonoBehaviour
{
	[SerializeField] private OptionToEdit optionToEdit = OptionToEdit.A_MusicVolume;
	[SerializeField] private AudioSource musicAudioSource;
	[SerializeField] private AudioSource sfxAudioSource;


	private Slider mySlider;
	private Text valueText;
	private Text valueTextChild;
	[HideInInspector] public enum OptionToEdit { A_MusicVolume, A_SfxVolume, Q_QualityPreset};
    // Start is called before the first frame update
    void Start()
    { 
		mySlider = GetComponent<Slider>();
		int childCount = transform.parent.childCount;
		if (childCount > 0) //should always be true
		{
			valueText = transform.parent.GetChild(0).GetComponent<Text>();

			for (int i = 0; i <= childCount - 1; i++)
			{
				valueText = transform.parent.GetChild(i).GetComponent<Text>();
				if (valueText == null)
				{ continue; }
				else if (valueText.gameObject.name == "ValueTextBack")
				{
					valueTextChild = valueText.transform.GetChild(0).GetComponent<Text>();
					break;
				}
			}
			if (valueText == null) //this is to catch the case where valueText is null for every iteration of the loop and the loop exits simply by ending, rather than the break statement.
			{ Debug.LogError("None of the children of " + transform.parent.gameObject.name + " have Text Components!! Please add one or remove this script."); }

		}
		else
		{ this.enabled = false; }
	}

	public void OnSliderValueChanged()
	{
		//object value; //variable of an unknown type that will be determined later. (actually either 2 or 4 lines below) 
		////the downside to doing it this way is that, anytime I want to math with it, I have to use an explicit conversion to whatever type I need, even if it has already been assigned a value of the correct type.
		////(see the line where I set the music volume in the first "else if()" statement below. There, "value" is already assigned a float number, yet I still need to use an explicit conversion.)
		//if (mySlider.wholeNumbers == true)
		//{ value = (int)mySlider.value; }
		//else
		//{ value = (float)mySlider.value; }
		float value = mySlider.value;

		switch (optionToEdit)
		{
			case OptionToEdit.A_MusicVolume:
				{
					decimal val = decimal.Round((decimal)value, 1); //convert "value" to a decimal and store it in a temp. variable so we can use the decimals' rounding function that allows use to specify a number of decimal places. 
																		//(Mathf.Round() only rounds to whole numbers.)
					value = (float)val; //convert our newly rounded "val" back to a float and store it back in "value".
					valueText.text = value + "%";
					valueTextChild.text = value + "%";
					musicAudioSource.volume = value / 100;
					break;
				}

			case OptionToEdit.A_SfxVolume:
				{
					decimal val = decimal.Round((decimal)value, 1); //convert "value" to a decimal and store it in a temp. variable so we can use the decimals' rounding function that allows use to specify a number of decimal places. 
																		//(Mathf.Round() only rounds to whole numbers.)
					value = (float)val; //convert our newly rounded "val" back to a float and store it back in "value".
					valueText.text = value + "%";
					valueTextChild.text = value + "%";
					sfxAudioSource.volume = value / 100;
					break;
				}

			case OptionToEdit.Q_QualityPreset:
				{
					int val = Mathf.FloorToInt(value);
					QualitySettings.SetQualityLevel(val);
					valueText.text = QualitySettings.names[val];
					valueTextChild.text = QualitySettings.names[val];
					break;
				}
			default:
				Debug.LogError("The switch in the OptionSliderController script on " + gameObject.name + "fell through to the default case!! This means that optionToEdit has an invalid value!!");
				break;

		}

	}
}
