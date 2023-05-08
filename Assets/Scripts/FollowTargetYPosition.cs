using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTargetYPosition : MonoBehaviour
{
	[SerializeField] private Transform target;
    void Update()
    {
		if (target != null)
		{
			if (transform.position.y != target.position.y)
			{ transform.position = new Vector3(transform.position.x, target.position.y, transform.position.z); }
		}
    }
}
