#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AlienSpawnController))]
[CanEditMultipleObjects]
[ExecuteInEditMode]

public class AlienSpawnControllerEditor : Editor
{
	SerializedProperty greenAlienSpawnChance;
	SerializedProperty blueAlienSpawnChance;
	SerializedProperty redAlienSpawnChance;

	private float prevGreenChance = 0;
	private float prevBlueChance = 0;
	private float prevRedChance = 0;
	private float frameNum = 1;

	void OnEnable()
	{
		greenAlienSpawnChance = serializedObject.FindProperty("greenAlienSpawnChance");
		blueAlienSpawnChance = serializedObject.FindProperty("blueAlienSpawnChance");
		redAlienSpawnChance = serializedObject.FindProperty("redAlienSpawnChance");

		prevGreenChance = greenAlienSpawnChance.floatValue;
		prevBlueChance = blueAlienSpawnChance.floatValue;
		prevRedChance = redAlienSpawnChance.floatValue;
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		base.OnInspectorGUI();
		EditorGUILayout.Space(10);
		EditorGUILayout.LabelField("I discovered custom editors!! The 3 values below this text should always equal 100,");
		EditorGUILayout.LabelField("even as they are changed in the editor. Try modifying them and see for yourself!!");
		EditorGUILayout.PropertyField(greenAlienSpawnChance);
		EditorGUILayout.PropertyField(blueAlienSpawnChance);
		EditorGUILayout.PropertyField(redAlienSpawnChance);
		EditorGUILayout.Space(10);
		EditorGUILayout.LabelField("Just for sh*ts and giggles, I tried to put an ASCII art design below this,");
		EditorGUILayout.LabelField("but Unitys' default font uses variable-width characters,");
		EditorGUILayout.LabelField("which meant it didn't work correctly.");
		EditorGUILayout.LabelField("To view the ASCII art correctly, view it at the bottom of the Editor script in Visual Studio.");
		EditorGUILayout.LabelField("It is supposed to be the first idle frame for one of the aliens.");
		EditorGUILayout.SelectableLabel(DrawASCIIArt(), GUILayout.Height(470));
		

		float greenChance = greenAlienSpawnChance.floatValue;
		float blueChance = blueAlienSpawnChance.floatValue;
		float redChance = redAlienSpawnChance.floatValue;
		Vector2 newChances = Vector2.zero;

		if (greenChance + blueChance + redChance != 100)
		{
			if (greenChance != prevGreenChance) //change blue and red to make it equal 100
			{
				newChances = BalanceSpawnChances(greenChance, blueChance, redChance);
				blueChance = newChances.x;
				redChance = newChances.y;
			}
			else if (blueChance != prevBlueChance) //change green and red to make it equal 100
			{
				newChances = BalanceSpawnChances(blueChance, greenChance, redChance);
				greenChance = newChances.x;
				redChance = newChances.y;
			}
			else if (redChance != prevRedChance) //change green and blue to make it equal 100
			{
				newChances = BalanceSpawnChances(redChance, greenChance, blueChance);
				greenChance = newChances.x;
				blueChance = newChances.y;
			}
		}


		greenAlienSpawnChance.floatValue = greenChance;
		blueAlienSpawnChance.floatValue = blueChance;
		redAlienSpawnChance.floatValue = redChance;

		prevGreenChance = greenAlienSpawnChance.floatValue;
		prevBlueChance = blueAlienSpawnChance.floatValue;
		prevRedChance = redAlienSpawnChance.floatValue;
		
		serializedObject.ApplyModifiedProperties();
	}

	private Vector2 BalanceSpawnChances(float changedValue, float otherValue1, float otherValue2) //I returned a Vector2 here because it is a single variable that holds 2 floats. I'm not using it as a Vector2.
																								  //changedValue is the value that was manually changed in the editor, otherValue1 and otherValue2 are the other 2 values that will be modified in this function to make the total other the 3 still be 100.
	{
		float halfDifference = (100 - (changedValue + otherValue1 + otherValue2)) / 2;
		float newVal1 = otherValue1 + halfDifference;
		float newVal2 = otherValue2 + halfDifference;

		if (newVal1 < 0)
		{
			newVal2 -= Mathf.Abs(newVal1);
			newVal1 = 0;
		}

		if (newVal2 < 0)
		{
			newVal1 -= Mathf.Abs(newVal2);
			newVal2 = 0;
		}

		if (newVal1 > 100)
		{
			newVal2 += 100 - newVal1;
			newVal1 = 100;
		}

		if (newVal2 > 100)
		{
			newVal1 += 100 - newVal2;
			newVal2 = 100;
		}

		newVal1 = Mathf.Clamp(newVal1, 0, 100); //I put the ifs above because I believe that the Mathf.Clamp function just subtracts and then discards whatever amount is needed to put the value back inside the stated range,
		newVal2 = Mathf.Clamp(newVal2, 0, 100); //however, I need to apply that value to the other "newVal", to make sure it still equals 100, instead of discarding it. These are only here as a backup.
		Vector2 otherValues = new Vector2(newVal1, newVal2);
		return otherValues;
	}

	private string DrawASCIIArt()
	{
		return
"                 &&						\n" +
"               /***...            **..*(	\n" +
"               &((((#            #((***.	\n" +
"                .(*%               &/(& 	\n" +
"            &//*&(*&***..,/&      #(.   	\n" +
"        /((((**/(/.*********..,&&(/.*   	\n" +
"     &*(((((***%/%*************#/.&     	\n" +
"    %((((((**********************.%		\n" +
"   &*((((((**********************,.&		\n" +
"   %/(((((********/&(((#&******&(%*(       \n" +
"   &/((((((******&(((((&&*****%(((&/		\n" +
"   &*((((((*****&&&&&&&&&*****&&&&&(		\n" +
"    &*(((((/***&&&&&&&&&&*****&&&&&&		\n" +
"     (*((((((***&&&&&&&&/******&&&&&		\n" +
"       &/((((((************((/*****		\n" +
"          /((%#/(((/*************&			\n" +
"         &/*&(*****(/((/*******			\n" +
"      ((((*.&(((((((((((**((*.&			\n" +
"   #/(%(**,&((((((((((******#**.%			\n" +
"  *((/**/*&(((((((((******..*%(*,.&		\n" +
" /(((**.%((((((((((********..*((*,.&		\n" +
"&(((*(.&((((((((((/********..*&((/./		\n" +
"&((*(,*(((((((((((**********.*(/#(*&		\n" +
" (*&((*((((((((((((***********#&&%		    \n" +
"      &*&****&(((((*********&*#			\n" +
"       &*((/**.*#((((/****%((**			\n" +
"        /(((((*..(((((((&(((*,(			\n" +
"        &(#****&..&((*//((#&**&			\n" +
"         //((**./(    &&/(***,				\n" +
"        &/(((***.&	   *((****(				\n" +
"       &(% (**,*/*   &((%(**,//			\n" +
"        %((((*****.   #((((*****,			";
		//EditorGUILayout.LabelField("                 &&                     ");
		//EditorGUILayout.LabelField("               /***...            **..*(");
		//EditorGUILayout.LabelField("               &((((#            #((***.");
		//EditorGUILayout.LabelField("                .(*%               &/(& ");
		//EditorGUILayout.LabelField("            &//*&(*&***..,/&      #(.   ");
		//EditorGUILayout.LabelField("        /((((**/(/.*********..,&&(/.*   ");
		//EditorGUILayout.LabelField("     &*(((((***%/%*************#/.&     ");
		//EditorGUILayout.LabelField("    %((((((**********************.%     ");
		//EditorGUILayout.LabelField("   &*((((((**********************,.&    ");
		//EditorGUILayout.LabelField("   %/(((((********/&(((#&******&(%*(    ");
		//EditorGUILayout.LabelField("   &/((((((******&(((((&&*****%(((&/    ");
		//EditorGUILayout.LabelField("   &*((((((*****&&&&&&&&&*****&&&&&(    ");
		//EditorGUILayout.LabelField("    &*(((((/***&&&&&&&&&&*****&&&&&&    ");
		//EditorGUILayout.LabelField("     (*((((((***&&&&&&&&/******&&&&&    ");
		//EditorGUILayout.LabelField("       &/((((((************((/*****     ");
		//EditorGUILayout.LabelField("          /((%#/(((/*************&      ");
		//EditorGUILayout.LabelField("         &/*&(*****(/((/*******         ");
		//EditorGUILayout.LabelField("      ((((*.&(((((((((((**((*.&         ");
		//EditorGUILayout.LabelField("   #/(%(**,&((((((((((******#**.%       ");
		//EditorGUILayout.LabelField("  *((/**/*&(((((((((******..*%(*,.&     ");
		//EditorGUILayout.LabelField(" /(((**.%((((((((((********..*((*,.&    ");
		//EditorGUILayout.LabelField("&(((*(.&((((((((((/********..*&((/./    ");
		//EditorGUILayout.LabelField("&((*(,*(((((((((((**********.*(/#(*&    ");
		//EditorGUILayout.LabelField(" (*&((*((((((((((((***********# &&%     ");
		//EditorGUILayout.LabelField("      &*&****&(((((*********&*#         ");
		//EditorGUILayout.LabelField("       &*((/**.*#((((/****%((**         ");
		//EditorGUILayout.LabelField("        /(((((*..(((((((&(((*,(         ");
		//EditorGUILayout.LabelField("        &(#****&..&((*//((#&**&         ");
		//EditorGUILayout.LabelField("         //((**./(    &&/(***,          ");
		//EditorGUILayout.LabelField("        &/(((***.&    *((****(          ");
		//EditorGUILayout.LabelField("        &(%(**,*/*   &((%(**,//         ");
		//EditorGUILayout.LabelField("        %((((*****.   #((((*****,       ");
	}
}
#endif
 