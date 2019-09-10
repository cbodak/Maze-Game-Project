using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour {

    private List<string> scoresList;
    private string filePath;
    public Text scoresText;

    public void Awake()
    {
        filePath = "Assets/Resources/Scores.txt";
        scoresList = new List<string>();
        getScores();
        writeScores();
    }
    public void startGame()
    {
        SceneManager.LoadScene(0);
    }

    public void exitGame()
    {
        SceneManager.LoadScene(1);
    }

    public void getScores()
    {
        StreamReader scoreReader = new StreamReader(filePath);
        string readLines;
        while ((readLines = scoreReader.ReadLine()) != null)
        {
            scoresList.Add(readLines);
            //Debug.Log(scoresList[0]);
        }
    }

    public void writeScores()
    {
        for(int i = 0; i < scoresList.Count; i++)
        {
            string[] scoreAndName = scoresList[i].Split(' ');
            scoresText.text += i + 1 + ". " + scoreAndName[0] + " time: " + scoreAndName[1] + "\n" + "\n";
        }
    }
}
