using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(MySimpleAnimatorV2_1))]
[RequireComponent(typeof(BoxCollider2D))] //this and the "RequireComponent" below are here because they are needed for the alien to work right, even though they aren't needed in this script. This just ensures that they are there. 
[RequireComponent(typeof(Rigidbody2D))]
public class SimpleAlienController : MonoBehaviour
{
	public Transform[] wayPoints;
	[SerializeField] private float walkSpeed = 2f;
	[SerializeField] private float walkFrameDelaySecs = 0.18f;
	[SerializeField] private float scaleForCollection = 0.1f;
	[SerializeField] private float collectionScaleIncrementAmt = -0.05f;
	[SerializeField] private float deleteSelfDelay = 0.1f;
	
	[SerializeField] private AlienType alienType = AlienType.Green;

	[HideInInspector] public enum AlienType { Green, Blue, Red }

	private Transform currWaypoint;
	private Transform player;
	private bool isInBeam = false;
	private bool isBeingCollected = false;
	private bool collectionTriggered = false;
	private bool isFleeing = false;
	private GameObject partFXChild;
	private GameManager manager;
	private MySimpleAnimatorV2_1 simpleAnim;
	private SpriteRenderer rend;
	private Transform fleeLeftTrans, fleeRightTrans;


	// Start is called before the first frame update
	void Start()
	{
		manager = GameObject.Find("GameController").GetComponent<GameManager>();
		if (manager.gameState == GameManager.GameState.InGame)
		{
			GameObject obj = GameObject.Find("Player");
			if(obj != null)
			{ player = obj.transform; }
		}

		simpleAnim = GetComponent<MySimpleAnimatorV2_1>();
		rend = GetComponent<SpriteRenderer>();
		partFXChild = transform.GetChild(0).gameObject;
		partFXChild.SetActive(false);

		if (alienType == AlienType.Red)
		{ simpleAnim.animFrameSprites = manager.redAlienWalkFrames; }
		else if (alienType == AlienType.Blue)
		{ simpleAnim.animFrameSprites = manager.blueAlienWalkFrames; }
		else
		{ simpleAnim.animFrameSprites = manager.greenAlienWalkFrames; }

		simpleAnim.secsBetweenFrameChange = walkFrameDelaySecs;
		PickNewWaypoint();
		simpleAnim.PlayAnim();
		fleeLeftTrans = manager.fleeLeftTrans;
		fleeRightTrans = manager.fleeRightTrans;
	}

    // Update is called once per frame
    void Update()
    {
		if (manager.gameState == GameManager.GameState.InGame && player != null)
		{
			if (isInBeam == false && isBeingCollected == false)
			{ MoveTowardsWaypoint(); }
			else if (isBeingCollected == true)
			{ GetCollected(); }
		}
		else
		{ simpleAnim.StopAnim(); }
	}
	private void GetCollected()
	{
		if (transform.localScale.x > scaleForCollection)
		{ transform.localScale -= new Vector3(-collectionScaleIncrementAmt, -collectionScaleIncrementAmt, -collectionScaleIncrementAmt); }
		else
		{
			partFXChild.SetActive(true);
			Invoke("DeleteSelf", deleteSelfDelay);
			if(alienType == AlienType.Green && collectionTriggered == false)
			{
				manager.CollectAlien(0);
				collectionTriggered = true;
			}
			else if(alienType == AlienType.Blue && collectionTriggered == false)
			{
				manager.CollectAlien(1);
				collectionTriggered = true;
			}
			else if(alienType == AlienType.Red && collectionTriggered == false)
			{
				manager.CollectAlien(2);
				collectionTriggered = true;
			}
		}

	}
	private void MoveTowardsWaypoint()
	{
		Vector2 newPos = Vector2.zero;

		if(isFleeing == true && manager.gameState == GameManager.GameState.InGame)
		{
			if(player.position.x <= transform.position.x)
			{ currWaypoint = fleeRightTrans; }
			else
			{ currWaypoint = fleeLeftTrans; }
		}
		if (currWaypoint.position.x < transform.position.x)
		{ rend.flipX = true; }
		else
		{ rend.flipX = false; }

		if (isFleeing == false)
		{ newPos = Vector2.MoveTowards(transform.position, currWaypoint.position, walkSpeed); }
		else
		{ newPos = Vector2.MoveTowards(transform.position, currWaypoint.position, walkSpeed + 2); }

		float newX = Mathf.Lerp(transform.position.x, newPos.x, Time.deltaTime);
		transform.position = new Vector3(newX, transform.position.y, transform.position.z);
	}
	private void PickNewWaypoint()
	{
		int rand = Random.Range(0, wayPoints.Length - 1);
		currWaypoint = wayPoints[rand];
	}
	private void DeleteSelf()
	{ Destroy(gameObject); }
	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Waypoint" & other.gameObject.name == currWaypoint.gameObject.name)
		{ PickNewWaypoint(); }
		else if (other.tag == "TractorBeam")
		{ 
			isInBeam = true;
			if (manager.audioControl.sfxSource.isPlaying == false)
			{ manager.audioControl.PlaySFX("abduct"); }
		}
		else if (other.tag == "CollectDetector" && isBeingCollected == false)
		{ isBeingCollected = true; }
		else if(other.tag == "FleeTrigger")
		{ isFleeing = true; }
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		if(collision.tag == "TractorBeam" && manager.audioControl.sfxSource.isPlaying == false)
		{ manager.audioControl.PlaySFX("abduct"); }
	}
	private void OnTriggerExit2D(Collider2D other)
	{
		if(other.tag == "TractorBeam")
		{ 
			isInBeam = false;
			if (manager.audioControl.sfxSource.isPlaying == true)
			{ manager.audioControl.StopSFX(); }
		}
		else if(other.tag == "FleeTrigger")
		{
			isFleeing = false;
			PickNewWaypoint();
		}

	}
}
