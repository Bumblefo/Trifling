using UnityEngine;
using System.Collections;
using System;

public class Player : TileMovingObject {

    private Animator animator;
    private Vector2 lastMove;
    //private bool justMoved;

    // Use this for initialization
    protected override void Start() {
        lastMove = new Vector2(0, 0);
        animator = GetComponent<Animator>();
        moveTime = 1f;
        base.Start();
    }

    // Update is called once per frame
    void Update() {
        int horizontal = 0;
        int vertical = 0;
        
        horizontal = (int)Input.GetAxisRaw("Horizontal"); //Always either -1,0,1
        vertical = (int)Input.GetAxisRaw("Vertical");

        /*if (horizontal == 0 && vertical == 0) //Prevent holding down of keys
        {
            justMoved = false;
        }*/

        //prevent moving horizontal and vertical at the same time. Favor horizontal movement over vertical movement
        if (horizontal != 0) 
        {
            vertical = 0;
        }

        if (lastMove.x != horizontal || lastMove.y != vertical) //If the current move isn't the same as the last move
        {
            if (horizontal != 0 || vertical != 0) //try to move if a move key was pressed
            {
                AttemptMove<Wall>(horizontal, vertical);
            }
            lastMove = new Vector2(horizontal, vertical);
        }
        /*
        if (!justMoved)
        {
            if (horizontal != 0 || vertical != 0) //try to move if a move key was pressed
            {
                AttemptMove<Wall>(horizontal, vertical);
            }
        }
        */


    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        //justMoved = true;
        base.AttemptMove<T>(xDir, yDir);
    }

    protected override void OnCantMove<T>(T component)
    {
        //justMoved = false; //Attempted to move but didn't actually move
        Wall hitwall = component as Wall;
        //This is just here for breaking walls
        //DON'T NEED YET UNTIL YOU IMPLEMENT BREAKABLE INNERWALLS
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        GameManager.instance.GameOver();
    }
}
