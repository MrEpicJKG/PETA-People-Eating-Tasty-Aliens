using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MySimpleAnimatorV2_1))]
[RequireComponent(typeof(BoxCollider2D))] //this and the "RequireComponent" below are here because they are needed for the alien to work right, even though they aren't needed in this script. This just ensures that they are there. 
[RequireComponent(typeof(Rigidbody2D))]
public class AlienController : MonoBehaviour
{
	#region Serialized Private Vars
	[Header("Speeds")]
	[SerializeField] private float walkSpeed = 2f;
	[SerializeField] private float runSpeed = 4f;

	[Header("Other Variables")]
	[SerializeField] private float mapWrapSpawnOffsetAmount = 1;
	[SerializeField] [Range(0, 99)] private float changeDirectionChance = 5;
	[SerializeField] private int changeDirectionTestIntervalTicks = 100; //approx. 2 secs
	[SerializeField] [Range(0, 99)] private float changeActionChance = 5;
	[SerializeField] private int changeActionTestIntervalTicks = 100; //approx. 2 secs
	[SerializeField] private float playerDetectDistance = 8f;
	[SerializeField] private float idleFrameDelaySecs = 0.4f;
	[SerializeField] private float walkFrameDelaySecs = 0.18f;
	[SerializeField] private float runFrameDelaySecs = 0.12f;
	[SerializeField] private Actions currentAction = Actions.Walking;
	[SerializeField] private AlienType alienType = AlienType.Green;

	#endregion

	#region Hidden Public Vars
	[HideInInspector] public enum Actions { Idle, Walking, Fleeing };
	[HideInInspector] public enum AlienType { Green, Blue, Red }
	#endregion

	#region Private Vars
	private GameManager manager;
	private Transform playerTrans;
	private MySimpleAnimatorV2_1 simpleAnim;
	private SpriteRenderer rend;
	private Rigidbody2D rb;
	private BeamController beamControl;
	private bool isMovingLeft = true;
	private float currMoveSpeed = 0;
	private Actions prevAction;
	private bool isInTractorBeam = false;
	private int currChangeDirectionTicks = 0;
	private int currChangeActionTicks = 0;
	private List<Sprite> idleFrames;
	private List<Sprite> walkFrames;
	private List<Sprite> runFrames;
	private Transform alienWrapLeft, alienWrapRight;
	#endregion

	void Start()
    {
		manager = GameObject.Find("GameController").GetComponent<GameManager>();
		simpleAnim = GetComponent<MySimpleAnimatorV2_1>();
		playerTrans = GameObject.Find("Player").transform;
		rend = GetComponent<SpriteRenderer>();
		rb = GetComponent<Rigidbody2D>();
		beamControl = playerTrans.GetComponentInChildren<BeamController>();

		if(alienType == AlienType.Green)
		{
			idleFrames = manager.greenAlienIdleFrames;
			walkFrames = manager.greenAlienWalkFrames;
			runFrames = manager.greenAlienRunFrames;
		}
		else if(alienType == AlienType.Blue)
		{
			idleFrames = manager.blueAlienIdleFrames;
			walkFrames = manager.blueAlienWalkFrames;
			runFrames = manager.blueAlienRunFrames;
		}
		else if(alienType == AlienType.Red)
		{
			idleFrames = manager.redAlienIdleFrames;
			walkFrames = manager.redAlienWalkFrames;
			runFrames = manager.redAlienRunFrames;
		}

	
		if(idleFrames.Count == 0)
		{ Debug.LogError("idleFrames has no sprites in it!!"); }
		if(walkFrames.Count == 0)
		{ Debug.LogError("walkFrames has no sprites in it!!"); }
		if(runFrames.Count == 0)
		{ Debug.LogError("runFrames has no sprites in it!!"); }

		//the 2 lines below are used to randomize the starting behavior of the aliens so that they dont all start doing the same thing.
		TestChangeDirection();
		TestChangeAction();

		if (currentAction == Actions.Idle)
		{
			simpleAnim.animFrameSprites = idleFrames;
			simpleAnim.secsBetweenFrameChange = idleFrameDelaySecs;
			currMoveSpeed = 0;
		}
		else if(currentAction == Actions.Walking)
		{ 
			simpleAnim.animFrameSprites = walkFrames;
			simpleAnim.secsBetweenFrameChange = walkFrameDelaySecs;
			if(isMovingLeft == true)
			{ currMoveSpeed = -walkSpeed; }
			else
			{ currMoveSpeed = walkSpeed; }
		}
		else if(currentAction == Actions.Fleeing)
		{ 
			simpleAnim.animFrameSprites = runFrames;
			simpleAnim.secsBetweenFrameChange = runFrameDelaySecs;
			if (isMovingLeft == true)
			{ currMoveSpeed = -runSpeed; }
			else
			{ currMoveSpeed = runSpeed; }
		}

		simpleAnim.ResetToCycleStart();
		simpleAnim.PlayAnim();

		prevAction = currentAction;
    }
    void Update()
    {
		if (manager.gameState == GameManager.GameState.InGame)
		{
			//if (TestForPlayerProx() == false) //player is not close by
		//	{ 
			ControlTestCounters(); 
			//}
			//else
			//{ DoFlee(); }

			if (isInTractorBeam == false)
			{ Move(); }
			else
			{
				if(currentAction != Actions.Idle)
				{ currentAction = Actions.Idle; }
			}

			if(isMovingLeft == true && rend.flipX == false)
			{ rend.flipX = true; }
			else if(isMovingLeft == false && rend.flipX == true)
			{ rend.flipX = false; }

			if (prevAction != currentAction) //action changed this tick
			{
				if (currentAction == Actions.Idle)
				{
					simpleAnim.animFrameSprites = idleFrames;
					simpleAnim.secsBetweenFrameChange = idleFrameDelaySecs;
					currMoveSpeed = 0;
				}
				else if (currentAction == Actions.Walking)
				{
					simpleAnim.animFrameSprites = walkFrames;
					simpleAnim.secsBetweenFrameChange = walkFrameDelaySecs;
					if (isMovingLeft == true)
					{ currMoveSpeed = -walkSpeed; }
					else
					{ currMoveSpeed = walkSpeed; }
				}
				else if (currentAction == Actions.Fleeing)
				{
					simpleAnim.animFrameSprites = runFrames;
					simpleAnim.secsBetweenFrameChange = runFrameDelaySecs;
					if (isMovingLeft == true)
					{ currMoveSpeed = -runSpeed; }
					else
					{ currMoveSpeed = runSpeed; }
				}

				simpleAnim.ResetToCycleStart();
				simpleAnim.PlayAnim();
			}			

			prevAction = currentAction;
		}
    }
	private void Move()
	{
		float newX = Mathf.Lerp(transform.position.x, transform.position.x + currMoveSpeed, Time.deltaTime / 5);
		transform.position = new Vector3(newX, transform.position.y, transform.position.z);
	}
	private bool TestForPlayerProx()
	{
		bool isPlayerNear = false;
		if (playerTrans != null)
		{
			if (Vector2.Distance(transform.position, playerTrans.position) < playerDetectDistance)
			{ isPlayerNear = true; }
		}
		return isPlayerNear;
	}
	private int TestPlayerSide() //this tests which side of this alien the player is on. -1 = left, 0 = directly above, 1 = right, -2 = default value (-2 should never be returned)
	{
		int side = -2;
		if (playerTrans.position.x < transform.position.x)
		{ side = -1; }
		else if (playerTrans.position.x > transform.position.x)
		{ side = 1; }
		else //if(playerTrans.position.x == transform.position.x)
		{ side = 0; }
		return side;
	}
	private void DoFlee()
	{
		if (isInTractorBeam == false)
		{
			if (currentAction != Actions.Fleeing)
			{ 
				currentAction = Actions.Fleeing;
				simpleAnim.secsBetweenFrameChange = 0.2f;
			}

			int side = TestPlayerSide();
			if ((side == -1 && isMovingLeft == true) || (side == 1 && isMovingLeft == false))
			{ DoChangeDirection(); }
		}
		else
		{ currentAction = Actions.Idle; }
	}
	private void ControlTestCounters()
	{
		if(currChangeDirectionTicks >= changeDirectionTestIntervalTicks)
		{
			currChangeDirectionTicks = 0;
			if (TestChangeDirection() == true)
			{ DoChangeDirection(); }
		}
		else
		{ currChangeDirectionTicks++; }

		if(currChangeActionTicks >= changeActionTestIntervalTicks)
		{
			currChangeActionTicks = 0;
			if(TestChangeAction() == true)
			{ DoChangeAction(); }
		}
		else
		{ currChangeActionTicks++; }
	}
	private bool TestChangeDirection()
	{
		bool shouldTurnAround = false;
		if (currentAction != Actions.Fleeing)
		{
			if (Random.Range(0, 101) < changeDirectionChance)
			{ shouldTurnAround = true; }
		}
		return shouldTurnAround;
	}
	private void DoChangeDirection()
	{
		if(isMovingLeft == true)
		{ isMovingLeft = false; }
		else
		{ isMovingLeft = true; }
	}
	private bool TestChangeAction()
	{
		bool shouldChangeAction = false;
		if (currentAction != Actions.Fleeing)
		{
			if (Random.Range(0, 101) < changeActionChance)
			{ shouldChangeAction = true; }
		}
		return shouldChangeAction;
	}
	private void DoChangeAction()
	{
		if (currentAction == Actions.Idle)
		{ 
			currentAction = Actions.Walking;
			simpleAnim.secsBetweenFrameChange = 0.2f;
		}
		else if(currentAction == Actions.Walking)
		{
			currentAction = Actions.Idle;
			simpleAnim.secsBetweenFrameChange = 0.4f;
		}
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "AlienWrapLeft")
		{ transform.position = new Vector3(alienWrapLeft.position.x + mapWrapSpawnOffsetAmount, transform.position.y, transform.position.z); }
		else if (other.tag == "AlienWrapRight")
		{ transform.position = new Vector3(alienWrapRight.position.x - mapWrapSpawnOffsetAmount, transform.position.y, transform.position.z); }
		else if(other.tag == "CollectDetector")
		{ 
			//Collect the Aliens Here 
		}
		else if(other.tag == "TractorBeam")
		{ 
			isInTractorBeam = true;
		}
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		if(other.tag == "TractorBeam")
		{ isInTractorBeam = false; }
	}
}
