using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour {

    public List<Enemy> enemyPrefabs;
    public BoardManager boardScript;
    public List<GameObject> currentEnemies;
    
    public float timeBetweenSpawn;

    void Start()
    {
        boardScript = GetComponent<BoardManager>();
    }

    public void StartSpawning()
    {
        timeBetweenSpawn = 1;
        StartCoroutine("SpawnEnemies");
    }
    public void StopSpawning()
    {
        StopCoroutine("SpawnEnemies");
    }

    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenSpawn);

            int enemyType = Random.Range(0, enemyPrefabs.Count);
            Vector3 boardTarget = boardScript.GetRandomGridPosition();
            Vector3 spawnPos = GenerateEnemySpawnPosition(boardTarget);
            Vector2 dirOfBoard = boardTarget - spawnPos;

            createEnemy(enemyType, spawnPos, dirOfBoard);
        }
    }



    public void createEnemy(int enemyType, Vector3 position, Vector2 moveDir)
    {
        //Need the .gameObject for some reason. Null Ref w/o it
        GameObject enemy = Instantiate(enemyPrefabs[enemyType].gameObject, position, Quaternion.identity) as GameObject;
        enemy.GetComponent<Enemy>().StartMove(moveDir);
    }

    public Vector3 GenerateEnemySpawnPosition(Vector3 boardTarget)
    {
        int dir = Random.Range(0, 4);
        //SpawnSide 0:Top 1:Right 2:Bottom 3:Left
        Vector3 spawnPos;

        //Later maybe spawn outside of sight

        if (dir == 0)
        {
            spawnPos = GameManager.instance.GetCameraTopRight();
            spawnPos = new Vector3(boardTarget.x, spawnPos.y, 0);
        }
        else if (dir == 1)
        {
            spawnPos = GameManager.instance.GetCameraTopRight();
            spawnPos = new Vector3(spawnPos.x, boardTarget.y, 0);
        }
        else if (dir == 2)
        {
            spawnPos = GameManager.instance.GetCameraBottomLeft();
            spawnPos = new Vector3(boardTarget.x, spawnPos.y, 0);
        }
        else
        {
            spawnPos = GameManager.instance.GetCameraBottomLeft();
            spawnPos = new Vector3(spawnPos.x, boardTarget.y, 0);
        }

        return spawnPos;
    }

    public void ClearEnemies()
    {
        //
    }
}
