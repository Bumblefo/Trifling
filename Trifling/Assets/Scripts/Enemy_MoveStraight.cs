using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class Enemy_MoveStraight : Enemy {

    // Use this for initialization
    protected override void Awake() {
        base.Awake();
        instance = this;
        moveSpeed = 1.25f;
        points = 1;
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
        
        MoveStraight(moveDir);
    }

    protected void MoveStraight(Vector2 moveDir)
    {
        body.velocity = moveDir * moveSpeed;
    }

}
