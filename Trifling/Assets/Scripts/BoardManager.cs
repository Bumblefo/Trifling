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

    public int columns;
    public int rows;
    public float tileSize = 0.32f;
    public GameObject playerPrefab;
    public GameObject baseTile;
    //If you want an arraylist? do public GameObject[] floorTiles;
    public GameObject outerWallTile;

    private Transform boardHolder;
    private List<Vector3> gridPositions = new List<Vector3>(); // List of moveable grid positions

    private List<GameObject> outerWalls;
    private List<GameObject> walkableTiles;

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
        outerWalls = new List<GameObject>();
        walkableTiles = new List<GameObject>();

        Vector2 middle = new Vector2(Mathf.Floor((float)columns / 2), Mathf.Floor((float)rows / 2));
        Camera.main.transform.position = new Vector3(middle.x * 0.32f, middle.y * 0.32f, Camera.main.transform.position.z);
        
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                GameObject tile;
                GameObject instance;
                if (x == 0 || y == 0 || x == columns - 1 || y == columns - 1)
                {
                    tile = outerWallTile;
                    instance = Instantiate(tile, new Vector3(x * tileSize, y * tileSize, 0f), Quaternion.identity) as GameObject;
                    outerWalls.Add(instance);
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

    public Vector3 GetRandomGridPosition()
    {
        return gridPositions[Mathf.RoundToInt(Random.Range(0, gridPositions.Count - 1))];
    }
}
