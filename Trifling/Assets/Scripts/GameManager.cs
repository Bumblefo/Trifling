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
    private float cameraMoveSpeed = 2f;

    private bool isCameraMoving;
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
        scoreText = GameObject.Find("ScoreText").GetComponent<Text>();
        isCameraMoving = false;
        cameraBody = Camera.main.GetComponent<Rigidbody2D>();
        InitGame();
    }

    // Use this for initialization
    void Start() {

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
    void Update() {

    }

    public float GetTileSize()
    {
        return boardScript.tileSize;
    }

    public Vector2 GetBoardMiddle()
    {
        return boardScript.middle;
    }

    public void GameOver()
    {
        Debug.Log("GameOver");
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

    public void SetCameraPosition(Vector2 pos)
    {
        Camera.main.transform.position = new Vector3(pos.x, pos.y, Camera.main.transform.position.z);
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
        
        float sqrRemainingDistance = (new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0f) - target).sqrMagnitude;

        while (sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(Camera.main.transform.position,
                new Vector3(target.x, target.y, Camera.main.transform.position.z), cameraMoveSpeed * Time.deltaTime);
            cameraBody.MovePosition(newPosition);

            sqrRemainingDistance = (new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0f) - target).sqrMagnitude;

            yield return null;
        }
        isCameraMoving = true;
    }
}
