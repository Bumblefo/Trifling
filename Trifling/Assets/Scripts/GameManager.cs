using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;
    public BoardManager boardScript;
    public EnemyManager enemyScript;
    public GameObject player;

    private int score;
    public Text scoreText;

    private int initialSquare = 3; //Change this later

    void Awake()
    {
        Random.seed = (int)System.DateTime.Now.Ticks;
        if (instance == null)
        {
            instance = this;
            //Makes gamemanager a singleton
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        boardScript = GetComponent<BoardManager>();
        enemyScript = GetComponent<EnemyManager>();
        scoreText = GameObject.Find("ScoreText").GetComponent<Text>();
        InitGame();
    }
    
    // Use this for initialization
	void Start () {
	    
	}

    void InitGame()
    {
        score = 0;
        scoreText.text = "0";
        enemyScript.ClearEnemies();
        boardScript.SetupScene(initialSquare, initialSquare); //numInnerColumns, numOuterColumns
        enemyScript.StartSpawning();

        StartCoroutine("IncreaseBoardSize");
    }
	
	// Update is called once per frame
	void Update () {

	}

    public void GameOver()
    {
        Debug.Log("GameOver");
    }

    private Vector2 GetCameraExtents()
    {
        float vertExtent = Camera.main.orthographicSize;
        float horizExtent = vertExtent * Screen.width / Screen.height;
        return new Vector2(horizExtent, vertExtent);
    }

    public Vector2 GetCameraTopRight()
    {
        Vector2 extents = GetCameraExtents();
        return new Vector3(Camera.main.transform.position.x + extents.x, Camera.main.transform.position.y + extents.y);
    }

    public Vector2 GetCameraBottomLeft()
    {
        Vector2 extents = GetCameraExtents();
        return new Vector3(Camera.main.transform.position.x - extents.x, Camera.main.transform.position.y - extents.y);
    }

    public void AddPoints(int points)
    {
        score += points;
        
        scoreText.text = score.ToString();
    }

    public void ExtendBoardRandomDirection()
    {
        int dir = Random.Range(0, 4);
        boardScript.ExtendBoard(dir);
    }

    private IEnumerator IncreaseBoardSize()
    {
        while (true)
        {
            yield return new WaitForSeconds(3);
            ExtendBoardRandomDirection();
        }
    }
}
