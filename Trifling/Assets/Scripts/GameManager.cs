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
    public Player playerScript;

    public int difficulty;
    public const int maxDifficulty = 16;
    public bool gameOver;
    private int totalScore;
    private int pointsGainedSinceLastExtension;
    private int pointsUntilExtension;
    public Text scoreText;
    
    private int initialSquare = 3; //Change this later
    private float cameraMoveSpeed = 2f;
    
    private bool isCameraMoving;
    private GameObject mainCamera;
    private Rigidbody2D cameraBody;

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
        mainCamera = Camera.main.gameObject;
        scoreText = GameObject.Find("ScoreText").GetComponent<Text>();
        isCameraMoving = false;
        cameraBody = Camera.main.GetComponent<Rigidbody2D>();
        pointsGainedSinceLastExtension = 0;
        pointsUntilExtension = 20;
        InitGame();
    }

    // Use this for initialization
    void Start() {

    }

    void InitGame()
    {
        totalScore = 0;
        scoreText.text = "0";
        gameOver = false;
        enemyScript.ClearEnemies();
        boardScript.SetupScene(initialSquare, initialSquare); //numInnerColumns, numInnerRows
        playerScript = player.GetComponent<Player>();
        enemyScript.StartSpawning();
    }

    public float GetTileSize()
    {
        return boardScript.tileSize;
    }

    public Vector2 GetBoardMiddle()
    {
        return boardScript.middle;
    }

    public void KillPlayer()
    {
        if (!playerScript.isDead)
        {
            playerScript.GameOver();
        }        
    }

    public void GameOver()
    {
        Debug.Log("GameOver");
        if (!gameOver)
        {
            gameOver = true;
            enemyScript.StopSpawning();
            enemyScript.ClearEnemies();
        }        
    }

    public float GetCameraVerticalExtent()
    {
        return Camera.main.orthographicSize;
    }

    public float GetCameraHorizontalExtent()
    {
        return GetCameraVerticalExtent() * Screen.width / Screen.height;
    }

    public Vector2 GetCameraExtents()
    {
        return new Vector2(GetCameraHorizontalExtent(), GetCameraVerticalExtent());
    }

    public Vector2 GetCameraTopRight()
    {
        Vector2 extents = GetCameraExtents();
        return new Vector3(mainCamera.transform.position.x + extents.x, mainCamera.transform.position.y + extents.y);
    }
    
    public Vector2 GetCameraBottomLeft()
    {
        Vector2 extents = GetCameraExtents();
        return new Vector3(mainCamera.transform.position.x - extents.x, mainCamera.transform.position.y - extents.y);
    }

    public void AddPoints(int points)
    {
        totalScore += points;
        pointsGainedSinceLastExtension += points;
        if (scoreText != null)
        {
            scoreText.text = totalScore.ToString();
        }

        if (pointsGainedSinceLastExtension >= pointsUntilExtension)
        {
            //When it becomes a 5x5 square. Don't increase size but only increase Difficulty every 50 points
            if (difficulty <= maxDifficulty)
            {
                IncreaseDifficulty();
            }
            pointsGainedSinceLastExtension -= pointsUntilExtension;

            if (boardScript.columns < 7 || boardScript.rows < 7) //If not a 5x5 square yet
            {
                //dir 0:Top, 1:Bot, 2:Left, 3:Right
                int dir = Random.Range(0, 2);
                if (boardScript.lastAddedIsColumn) //Alternate between directions added
                {
                    dir += 2;
                    boardScript.lastAddedIsColumn = false;
                }
                else
                {
                    boardScript.lastAddedIsColumn = true;
                }
                boardScript.ExtendBoard(dir);

                Debug.Log("Extended at: " + totalScore + " with ptsToExt: " + pointsUntilExtension);

                pointsUntilExtension += 20;
                if (pointsUntilExtension == 100)
                {
                    pointsUntilExtension = 50;
                }
                //20-20
                //40-60
                //60-120
                //80-200
            }
            else
            {
                Debug.Log("Difficulty: " + difficulty + " with ptsToExt: " + pointsUntilExtension);
                pointsUntilExtension += 10;
            }
        }
    }

    private void IncreaseDifficulty()
    {
        difficulty++;
        enemyScript.timeBetweenSpawn -= .05f;
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

    public void SetCameraPosition(Vector2 pos)
    {
        mainCamera.transform.position = new Vector3(pos.x, pos.y, mainCamera.transform.position.z);
    }

    public static Vector2 Vector3ToVector2(Vector3 vect)
    {
        return new Vector2(vect.x, vect.y);
    }

    public static Vector3 Vector2ToVector3(Vector2 vect)
    {
        return new Vector3(vect.x, vect.y, 0f);
    }

    public void CenterCameraOnBoard()
    {
        //SetCameraPosition(boardScript.middle);
        if (isCameraMoving)
        {
            StopCoroutine("SmoothMoveCamera");
        }
        StartCoroutine("SmoothMoveCamera", Vector2ToVector3(boardScript.middle));
    }

    public void CenterCameraOnPlayer()
    {
        //Looks more like you're moving the board around the player
        //SetCameraPosition(player.transform.position);
        if (isCameraMoving)
        {
            StopCoroutine("SmoothMoveCamera");
        }
        StartCoroutine("SmoothMoveCamera", player.transform.position);
    }

    private IEnumerator SmoothMoveCamera(Vector3 target)
    {
        isCameraMoving = true;

        float sqrRemainingDistance = (new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, 0f) - target).sqrMagnitude;

        while (sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(mainCamera.transform.position,
                new Vector3(target.x, target.y, mainCamera.transform.position.z), cameraMoveSpeed * Time.deltaTime);
            cameraBody.MovePosition(newPosition);

            sqrRemainingDistance = (new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, 0f) - target).sqrMagnitude;

            yield return null;
        }
        isCameraMoving = true;
    }
}
