using UnityEngine;
using System.Collections;
using System;

public abstract class Enemy : MonoBehaviour {

    //Game Manager should set the position of the Enemy during instantiation

    public Enemy instance;
    protected float moveSpeed;
    protected Rigidbody2D body;
    protected int points;

    // Use this for initialization
    protected virtual void Awake () {
        body = GetComponent<Rigidbody2D>();
    }
	
	// Update is called once per frame
	protected virtual void Update () {
	    
	}

    //Called by GameManager after instantiation is done
    public abstract void StartMove(Vector2 moveDir); // should be in the direction of the board maybe middle of the board - pos of enemy start

    protected virtual void OnBecameInvisible()
    {
        this.Destroy();
        Destroy(gameObject);
    }

    protected virtual void Destroy()
    {
        GameManager.instance.AddPoints(points);
    }
}
