using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienAnimController : MonoBehaviour
{
    private Animator animator;
    private AlienController controller;
    private GameManager manager;
    private int state = 0;
    private bool playerNearby = false;
    private bool isAtWaypoint = false;
    private bool isMovingLeft = false;
    private bool shouldAttack = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
