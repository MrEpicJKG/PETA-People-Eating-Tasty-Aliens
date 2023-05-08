using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreditController : MonoBehaviour
{
	public Scrollbar vertScrollBar;
	[SerializeField] private float scrollSpeedMult = 1;
	[SerializeField] private float scrollIncrementAmountPerTick = 0.02f;

	private bool canScroll = false;


	private void Awake()
	{
		vertScrollBar.value = 1;
		canScroll = true;
	}
	// Update is called once per frame
	void Update()
    {
        if(canScroll == true & (vertScrollBar.value <= 1 && vertScrollBar.value >= 0)) //last 2 conditions should always be true, used single & so that it wouldn't even test the rest of the statement if it isn't gong to do anything with the info
		{
			float newValue = Mathf.Lerp(vertScrollBar.value, vertScrollBar.value - scrollIncrementAmountPerTick, Time.deltaTime * scrollSpeedMult);
			vertScrollBar.value = Mathf.Clamp(newValue, 0, 1);
			if(vertScrollBar.value == 0)//reached bottom of scroll
			{ canScroll = false; }
		}
    }
}
