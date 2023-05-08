using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

//Figuring out how to get the CrossPlatformInputManager to accept both mobile and Keyboard controls was excedingly difficult. It took me 5 days to figure out. I tried everything from manually overriding the values returned by 
//the CrossPlatformInputManager.GetAxis() function if the keys were down, (which failed because CrossPlatformInputManager disables the regular input system and has no GetKey() function) to manually changing the MOBILE_INPUT 
//compilier command if mobile input was disabled. I'm kind of annoyed it took me this long to figure out. 

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class PlayerController : MonoBehaviour
{
	#region Public Vars
	[Header("Upgradable Stats")]
	public float statusEffectDurationSecs = 5f;
	public int maxHealth = 100;
	public float maxMoveDeltaPerTick = 5f;
	public float slowedMoveSpeedMult = 0.8f;
	public int maxAliensInBeam = 3;
	public float beamPullForce = 12;
	[Header("--------------------")]
	#endregion
	#region Serialized Private Vars
	[SerializeField] private float inputSafeZoneRadius = 0.1f;
	[SerializeField] private bool useDropInLevelTransitions = true;
	[SerializeField] private float transitionDropDistance = 5f;
	[SerializeField] private float transitionDistBufferAmt = 0.1f;
	[SerializeField] private GameObject rearThrusterLeft, rearThrusterRight;
	[SerializeField] private float onDeathImpulseForce = 1;
	[SerializeField] private GameObject explosionEffect;
	#endregion
	#region Hidden Public Vars
	[HideInInspector] public Status status = Status.Normal;
	[HideInInspector] public enum Status { Normal, Slowed, Dead};
	[HideInInspector] public bool isInTransition = true;
	[HideInInspector] public int currHealth = 100;
	[HideInInspector] public bool mobileInputEnabled = true;
	[HideInInspector] public GameObject hitFX;
	[HideInInspector] public GameManager manager;
	#endregion
	#region Private Vars

	private bool canMoveLeft = true;
	private bool canMoveRight = true;
	private bool canMoveUp = true;
	private bool canMoveDown = true;
	private float initYPos = 0;
	private Vector2 prevPos;
	private Vector2 inputValues;
	private SpriteRenderer rend;
	private bool hasHitGround = false;
	private PersistentObjectManager POM;
	[Range(-1, 1)] private int standaloneAxisHoriz = 0;
	[Range(-1, 1)] private int standaloneAxisVert = 0;


	#endregion
	// Start is called before the first frame update
	void Start()
	{
		manager = GameObject.Find("GameController").GetComponent<GameManager>();
		transform.GetChild(1).gameObject.SetActive(false);
		rend = GetComponent<SpriteRenderer>();
		isInTransition = useDropInLevelTransitions;
		initYPos = transform.position.y;
		prevPos = Vector2.zero;
		currHealth = maxHealth;
		explosionEffect.SetActive(false);
		hitFX = transform.GetChild(6).gameObject;
		hitFX.SetActive(false);
		mobileInputEnabled = manager.DB_mobileInputEnabled;
		
	}

	// Update is called once per frame
	void Update()
    {
		if (manager.isGameRunning == true && manager.gameState == GameManager.GameState.InGame)
		{
			if (isInTransition == true)
			{ DoInOutTransitions(false); }
			else if (status != Status.Dead)
			{
				DoMove();
				if (TestForDeath() == true && status != Status.Dead)
				{ DoDeathSequence(); }
			}
			else if (hasHitGround == true)
			{
				Rigidbody2D rb = GetComponent<Rigidbody2D>();
				if (rb.velocity.x > 0.02f)
				{ rb.velocity *= 0.5f; }
				else
				{ rb.velocity = new Vector2(0, 0); }
			}
		}
		else if(manager.gameState == GameManager.GameState.RoundOver)
		{ DoInOutTransitions(true); }

		prevPos = transform.position;
	}

	private void DoInOutTransitions(bool isShipLeavingLevel)
	{
		if (isShipLeavingLevel == false)
		{
			if (transform.position.y <= (initYPos - transitionDropDistance) + transitionDistBufferAmt)
			{ isInTransition = false; }
			else
			{
				transform.GetChild(1).gameObject.SetActive(true);
				float yPos = transform.position.y;
				float newY = Mathf.Lerp(yPos, initYPos - transitionDropDistance, Time.deltaTime);
				transform.position = new Vector3(transform.position.x, newY, transform.position.z);
			}
		}
		else
		{
			if (transform.position.y >= initYPos - transitionDistBufferAmt)
			{
				isInTransition = false; 
				if(transform.GetChild(1).gameObject.activeSelf == true)
				{ transform.GetChild(1).gameObject.SetActive(false); }
			}
			else
			{
				float yPos = transform.position.y;
				float newY = Mathf.Lerp(yPos, initYPos, Time.deltaTime);
				transform.position = new Vector3(transform.position.x, newY, transform.position.z);
			}
		}
	}
	
	private void DoMove()
	{
		//print("Pre-Input Values: (" + CrossPlatformInputManager.GetAxis("Horizontal") + ", " + CrossPlatformInputManager.GetAxis("Vertical") + ")");

		//if (mobileInputEnabled == true)
		//{
		//	if (Input.GetKey(KeyCode.D) == true || Input.GetKey(KeyCode.RightArrow) == true)
		//	{ CrossPlatformInputManager.SetAxis("Horizontal", 1); }
		//	else if (Input.GetKey(KeyCode.A) == true || Input.GetKey(KeyCode.LeftArrow) == true)
		//	{ CrossPlatformInputManager.SetAxis("Horizontal", -1); }
		//	else if (mobileInputEnabled == false || (manager.DB_useDebugMode == true && manager.DB_mobileInputEnabled == false))
		//	{ CrossPlatformInputManager.SetAxis("Horizontal", 0); }

		//	if (Input.GetKey(KeyCode.W) == true || Input.GetKey(KeyCode.UpArrow) == true)
		//	{ CrossPlatformInputManager.SetAxis("Vertical", 1); }
		//	else if (Input.GetKey(KeyCode.S) == true || Input.GetKey(KeyCode.DownArrow) == true)
		//	{ CrossPlatformInputManager.SetAxis("Vertical", -1); }
		//	else if (mobileInputEnabled == false || (manager.DB_useDebugMode == true && manager.DB_mobileInputEnabled == false))
		//	{ CrossPlatformInputManager.SetAxis("Vertical", 0); }
		//}
		//else
		//{
			if (Input.GetKey(KeyCode.D) == true || Input.GetKey(KeyCode.RightArrow) == true)
			{ standaloneAxisHoriz = 1; }
			else if (Input.GetKey(KeyCode.A) == true || Input.GetKey(KeyCode.LeftArrow) == true)
			{ { standaloneAxisHoriz = -1; } }
			else if (mobileInputEnabled == false || (manager.DB_useDebugMode == true && manager.DB_mobileInputEnabled == false))
			{ { standaloneAxisHoriz = 0; } }

			if (Input.GetKey(KeyCode.W) == true || Input.GetKey(KeyCode.UpArrow) == true)
			{ standaloneAxisVert = 1; }
			else if (Input.GetKey(KeyCode.S) == true || Input.GetKey(KeyCode.DownArrow) == true)
			{ standaloneAxisVert = -1; }
			else if (mobileInputEnabled == false || (manager.DB_useDebugMode == true && manager.DB_mobileInputEnabled == false))
			{ standaloneAxisVert = 0; }
		//}

		//print("Post-Keyboard Values: (" + CrossPlatformInputManager.GetAxis("Horizontal") + ", " + CrossPlatformInputManager.GetAxis("Vertical") + ")");
		if (mobileInputEnabled == true)
		{
			inputValues = new Vector2(CrossPlatformInputManager.GetAxis("Horizontal"), CrossPlatformInputManager.GetAxis("Vertical"));
		}
		else
		{
			inputValues = new Vector2(standaloneAxisHoriz, standaloneAxisVert);
		}
		//print("Post-Mobile Values: (" + CrossPlatformInputManager.GetAxis("Horizontal") + ", " + CrossPlatformInputManager.GetAxis("Vertical") + ")");
		float maxMoveDelta = maxMoveDeltaPerTick;
		


		//----- Move Left/Right
		if (inputValues.x > inputSafeZoneRadius && canMoveRight == true) //move right
		{
			//switch which side thruster is enabled - enable thruster that is pointed left
			if (rearThrusterLeft.activeSelf == false)
			{ rearThrusterLeft.SetActive(true); }
			if (rearThrusterRight.activeSelf == true)
			{ rearThrusterRight.SetActive(false); }

			if (rend.flipX == true)
			{ rend.flipX = false; }

			if (status == Status.Slowed)
			{ maxMoveDelta *= slowedMoveSpeedMult; }
			float newX = Mathf.Lerp(transform.position.x, transform.position.x + (maxMoveDelta * inputValues.x), Time.deltaTime);
			transform.position = new Vector3(newX, transform.position.y, transform.position.z);
		}
		else if (inputValues.x < -inputSafeZoneRadius && canMoveLeft == true) //move left
		{
			//switch which side thruster is enabled - enable thruster that is pointed right
			if (rearThrusterLeft.activeSelf == true)
			{ rearThrusterLeft.SetActive(false); }
			if (rearThrusterRight.activeSelf == false)
			{ rearThrusterRight.SetActive(true); }

			if (rend.flipX == false)
			{ rend.flipX = true; }

			if (status == Status.Slowed)
			{ maxMoveDelta *= slowedMoveSpeedMult; }

			float newX = Mathf.Lerp(transform.position.x, transform.position.x + (maxMoveDelta * inputValues.x), Time.deltaTime);
			transform.position = new Vector3(newX, transform.position.y, transform.position.z);
		}
		else if (inputValues.x >= -inputSafeZoneRadius && inputValues.x <= inputSafeZoneRadius) //if not moving Left or Right, disable side thrusters
		{ 
			if(rearThrusterLeft.activeSelf == true)
			{ rearThrusterLeft.SetActive(false); }
			else if(rearThrusterRight.activeSelf == true)
			{ rearThrusterRight.SetActive(false); }
		}

		//Move Up / Down
		if ((inputValues.y > inputSafeZoneRadius && canMoveUp == true) || (inputValues.y < -inputSafeZoneRadius && canMoveDown == true)) //move up or down (combined into 1 if statement since there is nothing other than movement to control)
		{
			if (status == Status.Slowed)
			{ maxMoveDelta *= slowedMoveSpeedMult; }
			float newY = Mathf.Lerp(transform.position.y, transform.position.y + (maxMoveDelta * inputValues.y), Time.deltaTime);
			transform.position = new Vector3(transform.position.x, newY, transform.position.z);
		}
	}

	private bool TestForDeath()
	{
		bool isDead = false;
		if(currHealth <= 0)
		{ isDead = true; }
		return isDead;
	}

	private void DoDeathSequence()
	{
		manager.audioControl.PlaySFX("explode");
		status = Status.Dead;
		Rigidbody2D rb = GetComponent<Rigidbody2D>();
		if(rb.bodyType != RigidbodyType2D.Dynamic)
		{ rb.bodyType = RigidbodyType2D.Dynamic; }
		if(rb.gravityScale != 1)
		{ rb.gravityScale = 1; }
		explosionEffect.SetActive(true);
		canMoveUp = false;
		canMoveRight = false;
		canMoveLeft = false;
		canMoveDown = false;
		Invoke("DestroySelf", 3);

		if (rend.flipX == false)
		{
			Vector2 deathForce = new Vector2(onDeathImpulseForce, 2);
			rb.AddForce(deathForce, ForceMode2D.Impulse);
		}
		else
		{
			Vector2 deathForce = new Vector2(-onDeathImpulseForce, 2);
			rb.AddForce(deathForce, ForceMode2D.Impulse);
		}

	}

	private void CureSelf()
	{
		if (status != Status.Normal && status != Status.Dead)
		{ status = Status.Normal; }
	}

	private void DestroySelf()
	{ Destroy(gameObject); }

	private void DisableHitFX()
	{ hitFX.SetActive(false); }

	[HideInInspector] public void OnTriggerEnter2D(Collider2D other)
	{
		if (isInTransition == false && status != Status.Dead)
		{
			if (other.gameObject.tag == "MapLeftSide")
			{
				//transform.position = new Vector3(mapRightSide.position.x + mapWrapSpawnOffsetAmount, transform.position.y, transform.position.z);
				canMoveLeft = false;
			}
			else if (other.gameObject.tag == "MapRightSide")
			{
				//transform.position = new Vector3(mapLeftSide.position.x - mapWrapSpawnOffsetAmount, transform.position.y, transform.position.z);
				canMoveRight = false;
			}
			else if (other.gameObject.tag == "MapTop")
			{ canMoveUp = false; }
			else if (other.gameObject.tag == "FlyAreaLowerLimit")
			{ canMoveDown = false; }
		}
	}

	[HideInInspector] public void OnTriggerExit2D(Collider2D other)
	{
		if (status != Status.Dead)
		{
			if (other.gameObject.tag == "MapLeftSide")
			{ canMoveLeft = true; }
			else if (other.gameObject.tag == "MapRightSide")
			{ canMoveRight = true; }
			else if (other.gameObject.tag == "MapTop")
			{ canMoveUp = true; }
			else if (other.gameObject.tag == "FlyAreaLowerLimit")
			{ canMoveDown = true; }
		}
	}

	[HideInInspector] public void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.tag == "Ground" && status == Status.Dead)
		{ hasHitGround = true; }
	}
}
