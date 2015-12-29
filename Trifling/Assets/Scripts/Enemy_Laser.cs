using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy_Laser : Enemy_MoveStraight
{

    public GameObject laserPrefab;

    public List<GameObject> laserSight;
    public Sprite laserSprite;
    private Vector2 moveDir;

    public BoxCollider2D boxColl;

    public static float moveTime = 1f;
    public static float delayUntilFiring = 1.5f;
    public static float fireDuration = 0.75f;

    protected override void Awake()
    {
        base.Awake();
        instance = this;
        moveSpeed = 1f;
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
        }

        moveDir.Normalize();
        this.moveDir = moveDir;
        MoveStraight(this.moveDir);
    }

    private void OnTriggerEnter2D()
    {
        body.velocity = new Vector2(0, 0);
        StartCoroutine("Fire", moveDir);
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
        Destroy();
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

            if (pos.x < camBotLeft.x || pos.x > camTopRight.x || pos.y < camBotLeft.y || pos.y > camTopRight.y)
            {
                break;
            }

            GameObject laserPart = Instantiate(laserPrefab, pos, laserRot) as GameObject;
            laserSight.Add(laserPart);
        }

        CreateLaserCollider();
    }

    protected void CreateLaserCollider()
    {
        boxColl.enabled = false;
        Vector2 boardMiddle = GameManager.instance.GetBoardMiddle();

        if (moveDir.x == 0) //Vertical
        {
            float vertExtent = GameManager.instance.GetCameraVerticalExtent();
            if (moveDir.y < 0)
            {
                boxColl.offset = new Vector2(0, -vertExtent);
            }
            else
            {
                boxColl.offset = new Vector2(0, vertExtent);
            }

            boxColl.size = new Vector2(GameManager.instance.GetTileSize(), vertExtent * 2);
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

    protected void ChangeLaserSightToLaser()
    {
        for (int i = 0; i < laserSight.Count; i++)
        {
            laserSight[i].GetComponent<SpriteRenderer>().sprite = laserSprite;
        }
        boxColl.enabled = true;
    }

    protected override void Destroy()
    {
        base.Destroy();
        for (int i = 0; i < laserSight.Count; i++)
        {
            Destroy(laserSight[i].gameObject);
        }
        Destroy(gameObject);
    }

    
}
