using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy_Laser : Enemy_MoveStraight
{
    public GameObject laserPrefab;

    public List<GameObject> laserSight;
    public Sprite laserSprite;
    private Vector2 moveDir;
    public LayerMask playerLayer;

    public BoxCollider2D boxColl;

    public static float moveTime = 1f;
    public static float delayUntilFiring = 1.5f;
    public static float fireDuration = 0.75f;
    private bool isMoving;

    protected override void Awake()
    {
        base.Awake();
        instance = this;
        moveSpeed = 1.5f;
        points = 3;
        boxColl = GetComponent<BoxCollider2D>();
    }

    public override void StartMove(Vector2 moveDir)
    {
        if (Mathf.Abs(moveDir.x) > Mathf.Abs(moveDir.y)) // move horizontal
        {
            moveDir.y = 0;
        }
        else //move vertical
        {
            moveDir.x = 0;
            transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        }
        
        this.moveDir = moveDir;
        isMoving = true;
        MoveStraight(this.moveDir);
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (isMoving && coll.gameObject.layer == 8)
        {
            body.velocity = new Vector2(0, 0);
            isMoving = false;
            StartCoroutine("Fire", moveDir);
        }
        //Debug.Log("Touched: " + coll.gameObject.name);
    }

    private IEnumerator Fire()
    {
        yield return new WaitForSeconds(moveTime);
        body.velocity = body.velocity * 0;

        CreateLaserSight();
        CreateLaserCollider();

        yield return new WaitForSeconds(delayUntilFiring);
        ChangeLaserSightToLaser();

        yield return new WaitForSeconds(fireDuration);
        DestroyEnemy();
    }

    protected void CreateLaserSight()
    {
        Vector2 pos = GameManager.Vector3ToVector2(transform.position);

        Vector2 camBotLeft = GameManager.instance.GetCameraBottomLeft();
        Vector2 camTopRight = GameManager.instance.GetCameraTopRight();

        Quaternion laserRot;

        if (moveDir.x == 0)
        {
            laserRot = Quaternion.Euler(0f, 0f, 90f);
        }
        else
        {
            laserRot = Quaternion.identity;
        }

        while (true)
        {
            pos += moveDir * GameManager.instance.GetTileSize();
            
            GameObject laserPart = Instantiate(laserPrefab, pos, laserRot) as GameObject;
            laserSight.Add(laserPart);

            if (pos.x < camBotLeft.x || pos.x > camTopRight.x || pos.y < camBotLeft.y || pos.y > camTopRight.y)
            {
                break;
            }
        }
    }

    protected void CreateLaserCollider()
    {
        boxColl.enabled = false;

        if (moveDir.x == 0) //Vertical
        {
            float vertExtent = GameManager.instance.GetCameraVerticalExtent();
            if (moveDir.y < 0)
            {
                boxColl.offset = new Vector2(-vertExtent, 0);
            }
            else
            {
                boxColl.offset = new Vector2(vertExtent, 0);
            }

            boxColl.size = new Vector2(vertExtent * 2, GameManager.instance.GetTileSize());
        }
        else //Horizontal
        {
            float horizExtent = GameManager.instance.GetCameraHorizontalExtent();

            if (moveDir.x < 0)
            {
                boxColl.offset = new Vector2(-horizExtent, 0);
            }
            else
            {
                boxColl.offset = new Vector2(horizExtent, 0);
            }
            boxColl.size = new Vector2(horizExtent * 2, GameManager.instance.GetTileSize());
        }
    }

    /*protected void RayCastLaser()
    {
        float distance = GameManager.instance.GetCameraHorizontalExtent() * 2;
        Debug.Log("moveDir + " + moveDir);
        RaycastHit2D hit = Physics2D.Raycast(GameManager.Vector3ToVector2(transform.position), moveDir, distance, playerLayer);

        if (hit.collider != null)
        {
            GameManager.instance.KillPlayer();
            //For some reason null reference if you try to GetComponent<Player>().GameOver()
        }
    }*/

    protected void ChangeLaserSightToLaser()
    {
        for (int i = 0; i < laserSight.Count; i++)
        {
            laserSight[i].GetComponent<SpriteRenderer>().sprite = laserSprite;
        }
        boxColl.enabled = true;

        if (boxColl.bounds.Contains(GameManager.instance.player.transform.position))
        {
            GameManager.instance.KillPlayer();
        }

        //RayCastLaser();
    }

    private void DeleteLaser()
    {
        for (int i = 0; i < laserSight.Count; i++)
        {
            Destroy(laserSight[i].gameObject);
        }
    }

    protected override void DestroyEnemy()
    {
        base.DestroyEnemy();
        DeleteLaser();
        Destroy(gameObject);
    }

    protected override void OnBecameInvisible()
    {
    }

    public override void DeleteEnemy()
    {
        DeleteLaser();
        base.DeleteEnemy();
    }

}