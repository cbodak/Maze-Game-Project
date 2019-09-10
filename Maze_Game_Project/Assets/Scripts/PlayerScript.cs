using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEditor;

public class PlayerScript : MonoBehaviour {

    private float playerHP;
    private float playerStamina;
    private bool[] playerKeys;
    private float playerTime;
    private float playerSpeed;
    private float[] speeds;
    private float playerRotationSpeed;
    private float playerRotation;
    private bool isPaused;
    private int jumpInt;
    private bool isRunning;
    private string filePath;
    private List<string> scoresList;

    private Vector3 playerMove;
    private Rigidbody playerRB;


    public Slider hpBar;
    public Slider staminaBar;
    public Text timeText;
    public Text pickupText;
    public Text notifyText;
    public Text finishText;
    public GameObject PausePanel;
    public GameObject UIPanel;
    public GameObject leaderBoardPanel;
    public GameObject gameOverPanel;
    public InputField username;
 
    private CharacterController charController;
    private Animator animController;

    private void Awake()
    {
        playerKeys = new bool[3];
        playerKeys[0] = false;
        playerKeys[1] = false;
        playerKeys[2] = false;

        speeds = new float[3];
        speeds[0] = 2.5f;
        speeds[1] = 7.5f;
        speeds[2] = 15;

        playerTime = 0;
        playerStamina = 50;
        playerHP = 50;
        pickupText.text = "Keys you currently have: ";
        playerSpeed = 5;
        playerRotationSpeed = 150;
        playerRotation = 0.0f;
        playerMove = Vector3.zero;

        charController = GetComponent<CharacterController>();
        animController = GetComponent<Animator>();
        playerRB = GetComponent<Rigidbody>();

        isPaused = false;
        isRunning = false;

        filePath = "Assets/Resources/Scores.txt";
        scoresList = new List<string>();
        readScores();
    }

    private void FixedUpdate()
    {
        float horizontalMovement = Input.GetAxis("Horizontal");
        float verticalMovement = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(horizontalMovement, 0.0f, verticalMovement);

        playerRB.AddForce(move * playerSpeed);
    }

    private void Update()
    {
        playerTimeUpdate();

        if (Input.GetKey(KeyCode.Space))
        {
            playerRun();
        }

        else
        {
            playerNormalSpeed();
        }

        if (Input.GetKey(KeyCode.J))
        {
            jumpInt = 0;
            playerJump();
        }

        if (jumpInt <= 10)
        {
            playerJump();
        }

        playerMovement();
    }

    //Handle jumping via the j key (simple jump with no simultaneous movement)
    private void playerJump()
    {
        transform.position = transform.position + new Vector3(0, 0.2f, 0);
        jumpInt += 1;
    }

    //Handle movement of the player and swapping of animations between idle, walk and run
    private void playerMovement()
    {
        if (charController.isGrounded)
        {
            if (Input.GetKey(KeyCode.W))
            {
                if (isRunning == false)
                {
                    animController.SetInteger("idleToWalk", 1);
                    Debug.Log("Changed...");
                }

                else
                {
                    animController.SetInteger("idleToWalk", 2);
                }
                playerMove = new Vector3(0, 0, 1);
                playerMove = playerMove * playerSpeed;
                playerMove = transform.TransformDirection(playerMove);
            }

            if (Input.GetKeyUp(KeyCode.W))
            {

                animController.SetInteger("idleToWalk", 0);
                Debug.Log("Changed 2...");
                playerMove = new Vector3(0, 0, 0);
            }

            if (Input.GetKey(KeyCode.S))
            {
                playerSlow();         
                animController.SetInteger("idleToWalk", 1);
                Debug.Log("Changed 3...");
                playerMove = new Vector3(0, 0, -1);
                playerMove = playerMove * playerSpeed;
                playerMove = transform.TransformDirection(playerMove);
            }

            if (Input.GetKeyUp(KeyCode.S))
            {
                playerNormalSpeed();
                animController.SetInteger("idleToWalk", 0);
                Debug.Log("Changed 4...");
                playerMove = new Vector3(0, 0, 0);
            }

            playerRotation += Input.GetAxis("Horizontal") * playerRotationSpeed * Time.deltaTime;
            transform.eulerAngles = new Vector3(0, playerRotation, 0);
            charController.Move(playerMove * Time.deltaTime);

            playerMove = new Vector3(0, 0, 0);
        }
    }

    //Calls the compare scores function to sort the scores (including player's current time) and then writes the new list back to scores.txt file, then return to main menu
    public void writeScores()
    {
        compareScores();
        StreamWriter scoreWriter = new StreamWriter(filePath);
        for(int i = 0; i < scoresList.Count; i++)
        {
            scoreWriter.WriteLine(scoresList[i]);
        }
        scoreWriter.Close();
        //AssetDatabase.ImportAsset(filePath);
        Resources.Load("Scores.txt");
        Time.timeScale = 1;
        SceneManager.LoadScene(1);
    }

    //Reads the leader board scores from the scores.txt file
    public void readScores()
    {
        StreamReader scoreReader = new StreamReader(filePath);
        string readLines;
        while((readLines = scoreReader.ReadLine()) != null)
        {
            scoresList.Add(readLines);
            Debug.Log(scoresList[0]);
        }
    }

    //Sorts the scores including the newly entered value to determine where it fits into the leader board
    public void compareScores()
    {
        if (scoresList.Count == 0)
        {
            scoresList.Add(username.text + " " + playerTime);
        }
        else
        {
            for (int i = 0; i < scoresList.Count; i++)
            {
                Debug.Log("ScoresList Count: " + scoresList.Count);
                string score = scoresList[i];
                Debug.Log("worked1");
                Debug.Log(scoresList[i]);
                string[] scoreAndName = new string[2];
                scoreAndName = score.Split(' ');
                Debug.Log("Worked...");
                float theScore = float.Parse(scoreAndName[1]);
                Debug.Log("Score: " + theScore);
                if (playerTime < theScore)
                {
                    scoresList.Insert(i, username.text + " " + playerTime);
                    if (scoresList.Count == 11)
                    {
                        scoresList.RemoveAt(scoresList.Count - 1);
                    }
                    break;
                }
            }
        }
    }

    //Reduce player speed to 2.5 (for moving backwards)
    private void playerSlow()
    {
        playerSpeed = speeds[0];
        isRunning = false;
    }

    //Return player to normal speed (7.5)
    private void playerNormalSpeed()
    {
        playerSpeed = speeds[1];
        playerStaminaUpdate(0.1f, "h");
        isRunning = false;
    }

    //Increase player speed to 15 (for running) and call stamina update to reduce stamina
    private void playerRun()
    {
        if(playerStamina > 0)
        {   
            if(isRunning == false)
            {
                isRunning = true;
            }

            playerSpeed = speeds[2];
            playerStaminaUpdate(0.3f, "l");
           
        }

        else
        {
            playerNormalSpeed();
        }
    }

    //Pause the game and open the paused UI panel, hide the existing UI panel
    public void paused()
    {
        if (isPaused == false)
        {   
            Time.timeScale = 0;
            isPaused = true;
            PausePanel.SetActive(true);
            UIPanel.SetActive(false);
        }

        else
        {
            Time.timeScale = 1;
            isPaused = false;
            PausePanel.SetActive(false);
            UIPanel.SetActive(true);
        }
    }

    //Resume time scale if paused and return to main menu
    public void exitGame()
    {
        if(isPaused == true)
        {
            Time.timeScale = 1;
            isPaused = false;
            PausePanel.SetActive(false);
            UIPanel.SetActive(true);
            SceneManager.LoadScene(1);
        }

        else
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(1);
        }
    }

    //Handle collisions with the various objects within the maze
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("HP+"))
        {
            playerHPUpdate(50, "h");
            other.gameObject.SetActive(false);
        }

        else if (other.CompareTag("Lava"))
        {
            gameOverPanel.SetActive(true);
            UIPanel.SetActive(false);
            Time.timeScale = 0;
        }

        else if (other.CompareTag("Boss"))
        {
            playerHPUpdate(20, "l");
        }

        else if (other.CompareTag("Guard"))
        {
            playerHPUpdate(10, "l");
        }

        else if (other.CompareTag("Finish"))
        {
            leaderBoardPanel.SetActive(true);
            UIPanel.SetActive(false);
            Time.timeScale = 0;
            finishText.text = "Congratulations! Your time was: " + playerTime + " seconds. Please enter a name to save your score: ";
        }

        else if (other.gameObject.CompareTag("GatePad"))
        {
            if (GameObject.Find("Gate1") != null)
            {
                GameObject.Find("Gate1").SetActive(false);
            }
            
        }

        else if (other.gameObject.CompareTag("OrangeDoor"))
        {
            if (playerKeys[0] == true)
            {
                GameObject.Find("OrangeDoor").SetActive(false);
            }

            else
            {
                notifyText.text = "You don't have this key yet!";
            }
        }

        else if (other.gameObject.CompareTag("BlueDoor"))
        {
            if (playerKeys[1] == true)
            {
                GameObject.Find("BlueDoor").SetActive(false);
            }

            else
            {
                notifyText.text = "You don't have this key yet!";
            }
        }

        else if (other.gameObject.CompareTag("RedDoor"))
        {
            if (playerKeys[2] == true)
            {
                GameObject.Find("RedDoor").SetActive(false);
            }

            else
            {
                notifyText.text = "You don't have this key yet!";
            }
        }

        else if(other.CompareTag("falseGate"))
        {
            other.gameObject.SetActive(false);
        }

        else if (other.gameObject.CompareTag("Treasure"))
        {
            if (playerKeys[0] == false)
            {
                playerKeys[0] = true;
                pickupUpdate("Orange");
                other.gameObject.SetActive(false);
            }
        }

        else if (other.gameObject.CompareTag("Treasure2"))
        {
            if (playerKeys[1] == false)
            {
                playerKeys[1] = true;
                pickupUpdate("Blue");
                other.gameObject.SetActive(false);
            }
        }

        else if (other.gameObject.CompareTag("Treasure3"))
        {
            if (playerKeys[2] == false)
            {
                playerKeys[2] = true;
                pickupUpdate("Red");
                other.gameObject.SetActive(false);
            }
        }
    }

    //Update the player's HP and reflect this change in the HP bar UI element
    private void playerHPUpdate(int hp, string direction)
    {
        if (direction == "h")
        {
            playerHP += hp;
        }

        else if (direction == "l")
        {
            playerHP -= hp;
        }

        if (playerHP >= 50)
        {
            playerHP = 50;
        }

        hpBar.value = playerHP;


        if (playerHP <= 0)
        {
            gameOverPanel.SetActive(true);
            UIPanel.SetActive(false);
            Time.timeScale = 0;
        }
    }

    //Update the player's stamina and reflect this change in the stamina bar UI element
    private void playerStaminaUpdate(float stamina, string direction)
    {   
        if(direction == "h")
        {
            playerStamina += stamina;
        }

        else if (direction == "l")
        {
            playerStamina -= stamina;
            
        }


        if (playerStamina >= 50)
        {
            playerStamina = 50;
        }

        staminaBar.value = playerStamina;
    }

    //Update the player's game time and reflect this change in the time text 
    private void playerTimeUpdate()
    {
        playerTime += Time.deltaTime;
        string text = playerTime.ToString();
        timeText.text = text;
    }

    //Update the list of held keys in the UI panel
    private void pickupUpdate(string key)
    {
        pickupText.text += key + " ";
    }
}
