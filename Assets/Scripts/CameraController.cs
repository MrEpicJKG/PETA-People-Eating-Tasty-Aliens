using UnityEngine;

public class CameraController : MonoBehaviour
{
	#region Serialized Private Vars
	[SerializeField] private Transform target;
	[SerializeField] private float farLeftPosition = -19f;
	[SerializeField] private float farRightPosition = 19f;
	[SerializeField] private float xOffsetAmount = -2f;
	#endregion
	#region Private Vars
	private GameManager manager;
	#endregion
	private void Start()
	{ 
		manager = GameObject.Find("GameController").GetComponent<GameManager>(); 

		if(target == null)
		{ Debug.LogError("CameraController.target is null. The Camera must have a target in order to follow it."); }
	}
	void Update()
    {
		if (manager.isGameRunning == true && manager.gameState == GameManager.GameState.InGame && target != null)
		{
			if (target.position.x > farLeftPosition && target.position.x < farRightPosition - xOffsetAmount)
			{ transform.position = new Vector3(target.position.x + xOffsetAmount, transform.position.y, transform.position.z); }
		}
    }
}
