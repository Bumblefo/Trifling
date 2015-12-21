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

        Vector2 middle = new Vector2(Mathf.Floor((float)columns / 2), Mathf.Floor((float)rows / 2));
        Camera.main.transform.position = new Vector3(middle.x * 0.32f, middle.y * 0.32f, Camera.main.transform.position.z);
        
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
    }

    public void SetupScene(int numInnerCol, int numInnerRow)
    {
        columns = numInnerCol + 2;
        rows = numInnerRow + 2;
        BoardSetup();
        InitialiseList();
    }

    public void ExtendBoard(int dir)
    {
        //dir: int 0-3 indicating direction to extend
        if (dir == 0) //Top
        {
            //Make old CornerTiles sideWalls
            outerWallsLeft.Add(cornerWalls.topLeft);
            outerWallsRight.Add(cornerWalls.topRight);

            //Make new CornerTiles
            Vector3 topLeftPos = cornerWalls.topLeft.transform.position;
            Vector3 topRightPos = cornerWalls.topRight.transform.position;
            Vector3 diff = new Vector3(0, 0.32f, 0);
            GameObject topLeftCorner = Instantiate(outerWallTile, topLeftPos + diff, Quaternion.identity) as GameObject;
            GameObject topRightCorner = Instantiate(outerWallTile, topRightPos + diff, Quaternion.identity) as GameObject;
            cornerWalls.topLeft = topLeftCorner;
            cornerWalls.topRight = topRightCorner;

            //Add new and replace the non-corner Top Outer Wall
            for (int i = 1; i < columns - 1; i++)
            {
                Vector3 oldPos = outerWallsTop[i].transform.position;
                gridPositions.Add(oldPos); //Will now be seen as a movable and targetable tile
                Destroy(outerWallsTop[i].gameObject);
                GameObject moveableTile = Instantiate(baseTile, oldPos, Quaternion.identity) as GameObject;
                walkableTiles.Add(moveableTile);

                GameObject instance = Instantiate(outerWallTile, oldPos + diff, Quaternion.identity) as GameObject;
                outerWallsTop[i] = instance;
            }

            
        }
        else if (dir == 1) //Bot
        {

        }
        else if (dir == 2) //Left
        {

        }
        else //Right
        {

        }
    }

    public Vector3 GetRandomGridPosition()
    {
        return gridPositions[Mathf.RoundToInt(Random.Range(0, gridPositions.Count - 1))];
    }
}
