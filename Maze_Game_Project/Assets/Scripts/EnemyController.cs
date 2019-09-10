using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private float enemyRotationSpeed;
    private float enemyRotation;
    private float enemySpeed;
    private Vector3 enemyMove;
    //private bool enemyTurnEnable;
    private float timeCheck;

    private bool[] walls;

    private CharacterController enemyController;
    private Animator enemyAnimController;

    private void Awake()
    {
        enemyRotationSpeed = 150;
        enemySpeed = 5;
        enemyMove = Vector3.zero;
        //enemyTurnEnable = true;
        timeCheck = 0;
        walls = new bool[2];

        walls[0] = false;
        walls[1] = false;

        enemyController = GetComponent<CharacterController>();
        enemyAnimController = GetComponent<Animator>();
    }

    //Handle collisions with the invisible walls to control enemy movement area
    private void OnTriggerEnter(Collider other)
    {
        //Rotates enemy 180 degrees if the wall is contacted
        if (other.CompareTag("EnemyRange"))
        {
            Debug.Log("wall 1 reached, turning...");
            enemyRotation = 180;
            transform.eulerAngles = new Vector3(0, enemyRotation, 0);
            enemyRangeReached(0);
        }

        //Rotates enemy 180 degrees if the wall is contacted
        if (other.CompareTag("EnemyRange2"))
        {
            Debug.Log("wall 2 reached, turning...");
            enemyRotation = 0;
            transform.eulerAngles = new Vector3(0, enemyRotation, 0);
            enemyRangeReached(1);
        }
    }

    //Helper function for changing animation to idle, setting time back to 0 and storing that the wall was collided with
    void enemyRangeReached(int wallNum)
    {
        //enemyTurnEnable = false;
        walls[wallNum] = true;
        timeCheck = 0;
        enemyAnimController.SetInteger("IdleWalk", 1);
    }

    //Enemy moves after 2 seconds for three seconds
    void Update()
    {

        timeCheck += Time.deltaTime;

        if (timeCheck > 2 && timeCheck < 5)
        {
            enemyMovement();
        }

        //Reset after 5 seconds to idle and clear wall boolean values
        else if (timeCheck > 5)
        {
            timeCheck = 0;
            enemyAnimController.SetInteger("IdleWalk", 1);
            Debug.Log(timeCheck);
            //enemyTurnEnable = true;
            walls[0] = false;
            walls[1] = false;
        }
    }

    //Handles movement of the enemy and updating of animation to walk animation
    void enemyMovement()
    {
        enemyAnimController.SetInteger("IdleWalk", 0);
        enemyMove = new Vector3(0, 0, 1);
        enemyMove = enemyMove * enemySpeed;
        enemyMove = transform.TransformDirection(enemyMove);

        enemyController.Move(enemyMove * Time.deltaTime);
    }
}