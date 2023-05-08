using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
public class BeamController : MonoBehaviour
{
	[SerializeField] private float sidePanelSafetyAreaWidth = 330;
	[SerializeField] private float minMaxAngle = 45;

	[HideInInspector] public List<Transform> aliensInBeamTrans;
	private Vector3 target;
	private GameManager manager;
	private int maxAliensInBeam;
	private bool mobileInputEnabled = true;
	private Vector2 prevAimPos;
	 
    // Start is called before the first frame update
    void Start()
    {
		manager = GameObject.Find("GameController").GetComponent<GameManager>();
		aliensInBeamTrans = new List<Transform>();
		maxAliensInBeam = GetComponentInParent<PlayerController>().maxAliensInBeam;

		GetComponent<AreaEffector2D>().forceMagnitude = GetComponentInParent<PlayerController>().beamPullForce;
		prevAimPos = Vector2.zero;
		
		//if (PlayerPrefs.HasKey("MobileInputEnabled") == true & PlayerPrefs.GetString("MobileInputEnabled") == "F")
		//{ mobileInputEnabled = false; }
		//else
		//{ mobileInputEnabled = true; }

		Input.GetButton("Fire1"); //For some weird reason, I had to put this here in order for the CrossPlatformInputManager.GetButton("Fire1") below to recognize the button.
	}

    // Update is called once per frame
    void Update()
    {
		if (manager.isGameRunning == true)
		{
			Vector2 aimPos = prevAimPos;
			//Beam Movement Control
			if (manager.DB_mobileInputEnabled == false /*&& (manager.DB_useDebugMode == true && manager.DB_mobileInputEnabled == false)*/)
			{
				Vector3 mousePos = CrossPlatformInputManager.mousePosition;
				if (mousePos.x > sidePanelSafetyAreaWidth && CrossPlatformInputManager.GetButton("Fire1") == true)
				{ aimPos = mousePos; }
			}
			else
			{
				Touch[] touches = Input.touches;
				foreach (Touch touch in touches)
				{
					if (touch.position.x > sidePanelSafetyAreaWidth)
					{
						aimPos.x = touch.position.x;
						aimPos.y = touch.position.y;
						break;
					}
				}
			}

			PointBeam(aimPos);
			prevAimPos = aimPos;
			////Abduction Control
			//foreach(Transform trans in aliensInBeamTrans)
			//{ trans.position = Vector2.MoveTowards(trans.position, transform.position, beamPullSpeed); }
		}
    }

	private void PointBeam(Vector2 pointPos)
	{
		target = Camera.main.ScreenToWorldPoint(new Vector3(pointPos.x, pointPos.y, transform.position.z));
		Vector3 difference = target - transform.position;
		float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.Euler(0.0f, 0.0f, Mathf.Clamp(rotationZ + 90, -minMaxAngle, minMaxAngle));
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Alien")
		{

			if (aliensInBeamTrans.Count < maxAliensInBeam)
			{ 
				aliensInBeamTrans.Add(other.GetComponent<Transform>());
				//other.GetComponent<Rigidbody2D>().isKinematic = true;
			}
		}
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		if (other.tag == "Alien")
		{
			int otherID = other.GetInstanceID();
			for (int i = 0; i < aliensInBeamTrans.Count - 1; i++)
			{ 
				if (aliensInBeamTrans[i].GetInstanceID() == otherID)
				{
					aliensInBeamTrans.RemoveAt(i);
					//other.GetComponent<Rigidbody2D>().isKinematic = false;
					break;
				}
			}
			if (manager.audioControl.sfxSource.isPlaying == true && aliensInBeamTrans.Count == 0)
			{ manager.audioControl.StopSFX(); }
		}
	}
}
