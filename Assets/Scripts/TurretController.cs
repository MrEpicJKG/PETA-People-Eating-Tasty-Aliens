using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{
	[SerializeField] private Transform target;
	[SerializeField] private GameObject projectile;
	[SerializeField] private float detectDist = 20;
	[SerializeField] private float delayBetweenShotsSecs = 1;
	[SerializeField] private Vector3 projSpawnOffset;
	[SerializeField] private float boltSpeed = 20;

	private GameManager manager;
	private Transform pivot;
	private float distanceToTarget = 0;
	private Vector3 difference;
	private float rotationZ = 0;
	private PlayerController pc;
	//private bool canFire = false;

    // Start is called before the first frame update
    void Start()
    {
		manager = GameObject.Find("GameController").GetComponent<GameManager>();
		pivot = transform.GetChild(0);
		difference = Vector3.zero;
		pc = target.GetComponent<PlayerController>();
	}

    // Update is called once per frame
    void Update()
    {
		if (pc.isInTransition == false && pc.status != PlayerController.Status.Dead)// && canFire == true)
		{
			distanceToTarget = Vector2.Distance(transform.position, target.position);
			difference = target.position - transform.position;
			if (distanceToTarget <= detectDist)
			{
				TrackTarget();
				if (IsInvoking("Shoot") == false)
				{ Invoke("Shoot", delayBetweenShotsSecs); }
			}
			else
			{
				if (IsInvoking("Shoot") == true)
				{ CancelInvoke("Shoot"); }
			}
		}

    }
	private void TrackTarget()
	{
		rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
		pivot.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ - 90);
	}
	private void Shoot()
	{
		manager.audioControl.PlaySFX("shoot");
		float distance = difference.magnitude;
		Vector2 direction = difference / distance;
		direction.Normalize();

		GameObject bolt = Instantiate(projectile, transform.GetChild(0).GetChild(0).position + projSpawnOffset, Quaternion.Euler(0.0f, 0.0f, rotationZ - 90));
		bolt.GetComponent<Rigidbody2D>().velocity = new Vector2(direction.x * boltSpeed, direction.y * boltSpeed);
	}

	//private void OnTriggerEnter2D(Collider2D collision) //I used OnTriggerStay2D so that the turrets that are already in the trigger at the start of the game will be able to fire
	//{
	//	if(canFire == false)
	//	{
	//		if (collision.gameObject.tag == "MainCamera")
	//		{ canFire = true; }
	//	}
	//}

	//private void OnTriggerExit2D(Collider2D collision)
	//{
	//	if(collision.gameObject.tag == "MainCamera" & canFire == true)
	//	{ canFire = false; }
	//}
}
