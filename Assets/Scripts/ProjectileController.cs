using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
	[SerializeField] private int damage = 5;
	[SerializeField] private float destroySelfDelaySecs = 1.0f;
	[SerializeField] private StatusEffect statusEffectToCause = StatusEffect.None;

	private enum StatusEffect { None, Slow};
	private void OnTriggerEnter2D(Collider2D other)
	{
		if(other.gameObject.tag == "MapLeftSide" || other.gameObject.tag == "MapRightSide" || other.gameObject.tag == "MapTop")
		{ Invoke("DestroySelf", destroySelfDelaySecs); }
		else if(other.gameObject.tag == "Player")
		{
			PlayerController pc = other.GetComponent<PlayerController>();
			pc.currHealth -= damage;
			pc.hitFX.SetActive(true);
			pc.Invoke("DisableHitFX", 0.5f);
			pc.manager.audioControl.PlaySFX("impact");
			if(statusEffectToCause == StatusEffect.Slow)
			{
				if(pc.status != PlayerController.Status.Slowed && pc.status != PlayerController.Status.Dead)
				{ 
					pc.status = PlayerController.Status.Slowed; 
					if(pc.IsInvoking("CureSelf") == true)
					{ pc.CancelInvoke("CureSelf"); }
					pc.Invoke("CureSelf", pc.statusEffectDurationSecs);
				}
			}
			Destroy(gameObject);
		}
	}
	private void DestroySelf()
	{ Destroy(gameObject); }

}
