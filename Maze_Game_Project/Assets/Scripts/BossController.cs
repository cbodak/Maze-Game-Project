using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour {
    private float enemyRotationSpeed;
    private float enemyRotation;
    private float enemySpeed;
    private Vector3 enemyMove;
   
    private float timeCheck;

    private bool[] walls;

    private CharacterController enemyController;
    private Animator enemyAnimController;

    private void Awake()
    {
        enemyRotationSpeed = 150;
        enemySpeed = 5;
        enemyMove = Vector3.zero;
     
        timeCheck = 0;
        walls = new bool[4];

        walls[0] = false;
        walls[1] = false;
        walls[2] = false;
        walls[3] = false;

        enemyController = GetComponent<CharacterController>();
        enemyAnimController = GetComponent<Animator>();
        
    }

    //Handle collisions with the maximum range invisible walls to control enemy movement
    private void OnTriggerEnter(Collider other)
    {   
        //Turn to face opposite direction of each wall
        if (other.CompareTag("EnemyRange"))
        {
         Debug.Log("wall 1 reached, turning...");
         enemyRotation = enemyRotation * (-1);
         transform.eulerAngles = new Vector3(0, enemyRotation, 0);
         enemyRangeReached(0);
        }

        if (other.CompareTag("EnemyRange2"))
        {
         Debug.Log("wall 2 reached, turning...");
         enemyRotation = enemyRotation * (-1);
         transform.eulerAngles = new Vector3(0, enemyRotation, 0);
         enemyRangeReached(1);
        }

        if (other.CompareTag("EnemyRange3"))
        {
         Debug.Log("wall 3 reached, turning...");
         enemyRotation = 0;
         transform.eulerAngles = new Vector3(0, enemyRotation, 0);
         enemyRangeReached(2);
        }

        if (other.CompareTag("EnemyRange4"))
        {
         Debug.Log("wall 4 reached, turning...");
         enemyRotation = 180;
         transform.eulerAngles = new Vector3(0, enemyRotation, 0);
         enemyRangeReached(3);
        }
    }

    //Handle updating enemy rotation when they reach maximum range (set rotation to opposite of the wall collided with)
    void enemyRangeReached(int wallNum)
    {
        walls[wallNum] = true;
        timeCheck = 0;
        enemyAnimController.SetInteger("IdleWalk", 1);
    }
 
    void Update () {

        timeCheck += Time.deltaTime;

        //Idle for 2 seconds, move for 3 seconds
        if (timeCheck > 2 && timeCheck < 5)
        {
            enemyMovement();

            int enemyTurnRandom = Random.Range(1, 25);

            if ((enemyTurnRandom == 1))//&& (enemyTurnEnable))
            {
                enemyTurn();
            }
        }

        //Reset everything after 5 seconds back to idle and clear wall booleans
        else if(timeCheck > 5)
        {
            timeCheck = 0;
            enemyAnimController.SetInteger("IdleWalk", 1);
            Debug.Log(timeCheck);
            //enemyTurnEnable = true;
            walls[0] = false;
            walls[1] = false;
            walls[2] = false;
            walls[3] = false;
        }
	}

    //Handle movement of the enemy
    void enemyMovement()
    {
        enemyAnimController.SetInteger("IdleWalk", 0);
        enemyMove = new Vector3(0, 0, 1);
        enemyMove = enemyMove * enemySpeed;
        enemyMove = transform.TransformDirection(enemyMove);

        enemyController.Move(enemyMove * Time.deltaTime);
    }

    //Handle random turning of the enemy
    void enemyTurn()
    {
        int enemyTurnAngle = Random.Range(-25, 25);
        enemyRotation +=  enemyTurnAngle * enemyRotationSpeed * Time.deltaTime;
        transform.eulerAngles = new Vector3(0, enemyRotation, 0);
    }
}
