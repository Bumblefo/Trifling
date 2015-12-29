using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour {

    /*[Serializable]
    public class Count
    {
        public int minimum;
        public int maximum;

        public Count(int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }*/

    private struct CornerWall
    {
        public GameObject topLeft;
        public GameObject topRight;
        public GameObject botLeft;
        public GameObject botRight;
    }

    public int columns;
    public int rows;
    public float tileSize = 0.32f;
    public GameObject playerPrefab;
    public GameObject baseTile;
    //If you want an arraylist? do public GameObject[] floorTiles;
    public GameObject outerWallTile;

    public Vector2 middle;
    private Transform boardHolder;
    private List<Vector3> gridPositions = new List<Vector3>(); // List of moveable grid positions
    private CornerWall cornerWalls;

    private List<GameObject> outerWallsTop = new List<GameObject>();
    private List<GameObject> outerWallsBot = new List<GameObject>();
    private List<GameObject> outerWallsLeft = new List<GameObject>();
    private List<GameObject> outerWallsRight = new List<GameObject>();

    private List<GameObject> walkableTiles = new List<GameObject>();

    void InitialiseList()
    {
        //Movable Tiles
        gridPositions.Clear();
        for (int x = 1; x < columns - 1; x++)
        {
            for (int y = 1; y < rows - 1; y++)
            {
                gridPositions.Add(new Vector3(x * tileSize, y * tileSize, 0f));
            }
        }

    }

    void BoardSetup()
    {
        //Sets up outer wall and floor
        boardHolder = new GameObject("Board").transform;
        
        middle = new Vector2(Mathf.Floor((float)columns / 2), Mathf.Floor((float)rows / 2)); //Sets initial middle of board
                        
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                GameObject tile;
                GameObject instance;
                if (x == 0) //Left
                {
                    tile = outerWallTile;
                    instance = Instantiate(tile, new Vector3(x * tileSize, y * tileSize, 0f), Quaternion.identity) as GameObject;
                    if (y == 0)
                    {
                        cornerWalls.botLeft = instance;
                    }
                    else if (y == columns - 1)
                    {
                        cornerWalls.topLeft = instance;
                    }
                    else
                    {
                        outerWallsLeft.Add(instance);
                    }
                }
                else if (x == columns - 1) //Right
                {
                    tile = outerWallTile;
                    instance = Instantiate(tile, new Vector3(x * tileSize, y * tileSize, 0f), Quaternion.identity) as GameObject;
                    if (y == 0)
                    {
                        cornerWalls.botRight = instance;
                    }
                    else if (y == columns - 1)
                    {
                        cornerWalls.topRight = instance;
                    }
                    else
                    {
                        outerWallsRight.Add(instance);
                    }            
                }
                else if (y == 0) //Bot
                {
                    tile = outerWallTile;
                    instance = Instantiate(tile, new Vector3(x * tileSize, y * tileSize, 0f), Quaternion.identity) as GameObject;
                    outerWallsBot.Add(instance);
                }
                else if (y == columns - 1) //Top
                {
                    tile = outerWallTile;
                    instance = Instantiate(tile, new Vector3(x * tileSize, y * tileSize, 0f), Quaternion.identity) as GameObject;
                    outerWallsTop.Add(instance);
                }
                else
                {
                    tile = baseTile;
                    instance = Instantiate(tile, new Vector3(x * tileSize, y * tileSize, 0f), Quaternion.identity) as GameObject;
                    walkableTiles.Add(instance);
                    if (x == middle.x && y == middle.y)
                    {
                        GameObject Player = Instantiate(playerPrefab, instance.transform.position, Quaternion.identity) as GameObject;
                        GameManager.instance.player = Player;
                    }
                }

                instance.transform.SetParent(boardHolder);
            }
        }

        UpdateMiddleOfBoard();
        GameManager.instance.SetCameraPosition(middle);
    }

    public void UpdateMiddleOfBoard()
    {
        Vector3 midpoint = (cornerWalls.topRight.transform.position + cornerWalls.botLeft.transform.position) / 2;
        middle = new Vector2(midpoint.x, midpoint.y);
    }

    public void SetupScene(int numInnerCol, int numInnerRow)
    {
        columns = numInnerCol + 2;
        rows = numInnerRow + 2;
        BoardSetup();
        InitialiseList();
    }

    private void DebugPrintCorners()
    {
        Debug.Log("TopLeft: " + cornerWalls.topLeft.transform.position.ToString());
        Debug.Log("TopRight: " + cornerWalls.topRight.transform.position.ToString());
        Debug.Log("botLeft: " + cornerWalls.botLeft.transform.position.ToString());
        Debug.Log("botRight: " + cornerWalls.botRight.transform.position.ToString());
    }

    public void ExtendBoard(int dir)
    {
        //dir: int 0-3 indicating direction to extend
        if (dir == 0) //Top
        {
            ExtendWall(ref outerWallsTop, ref outerWallsLeft, ref outerWallsRight, ref cornerWalls.topLeft,
                ref cornerWalls.topRight, Vector3.up * tileSize);
            rows++;      
        }
        else if (dir == 1) //Bot
        {
            ExtendWall(ref outerWallsBot, ref outerWallsRight, ref outerWallsLeft, ref cornerWalls.botRight,
                ref cornerWalls.botLeft, Vector3.down * tileSize);
            rows++;
        }
        else if (dir == 2) //Left
        {
            ExtendWall(ref outerWallsLeft, ref outerWallsBot, ref outerWallsTop, ref cornerWalls.botLeft,
                ref cornerWalls.topLeft, Vector3.left * tileSize);
            columns++;
        }
        else //Right
        {
            ExtendWall(ref outerWallsRight, ref outerWallsTop, ref outerWallsBot, ref cornerWalls.topRight,
                ref cornerWalls.botRight, Vector3.right * tileSize);
            columns++;
        }
        
        UpdateMiddleOfBoard();
        GameManager.instance.CenterCameraOnBoard();

        //DebugPrintCorners();
    }

    public void ExtendWall(ref List<GameObject> wallToExtend, ref List<GameObject> leftSideWall, ref List<GameObject> rightSidewall
        , ref GameObject leftCorner, ref GameObject rightCorner, Vector3 diff)
    {
        ExtendCorners(ref leftSideWall, ref rightSidewall, ref leftCorner, ref rightCorner, ref diff);
        ExtendCenterSideWall(ref wallToExtend, ref diff);
    }

    public void ExtendCorners(ref List<GameObject> leftSideWall, ref List<GameObject> rightSidewall
        , ref GameObject leftCorner, ref GameObject rightCorner, ref Vector3 diff)
    {
        //Make old CornerTiles into sideWalls
        leftSideWall.Add(leftCorner);
        rightSidewall.Add(rightCorner);

        //Make new CornerTiles
        Vector3 leftPos = leftCorner.transform.position;
        Vector3 rightPos = rightCorner.transform.position;

        GameObject newLeftCorner = Instantiate(outerWallTile, leftPos + diff, Quaternion.identity) as GameObject;
        GameObject newRightCorner = Instantiate(outerWallTile, rightPos + diff, Quaternion.identity) as GameObject;

        newLeftCorner.transform.SetParent(boardHolder);
        newRightCorner.transform.SetParent(boardHolder);

        leftCorner = newLeftCorner;
        rightCorner = newRightCorner;

    }

    public void ExtendCenterSideWall(ref List<GameObject> wallToExtend, ref Vector3 diff)
    {
        //Add new and replace the non-corner Outer Wall
        for (int i = 0; i < wallToExtend.Count; i++)
        {
            Vector3 oldPos = wallToExtend[i].transform.position;
            gridPositions.Add(oldPos); //Will now be seen as a movable and targetable tile
            Destroy(wallToExtend[i].gameObject);
            GameObject moveableTile = Instantiate(baseTile, oldPos, Quaternion.identity) as GameObject;
            walkableTiles.Add(moveableTile);

            moveableTile.transform.SetParent(boardHolder);

            GameObject instance = Instantiate(outerWallTile, oldPos + diff, Quaternion.identity) as GameObject;
            wallToExtend[i] = instance;

            instance.transform.SetParent(boardHolder);
        }
    }

    public Vector3 GetRandomGridPosition()
    {
        return gridPositions[Mathf.RoundToInt(Random.Range(0, gridPositions.Count - 1))];
    }
}
