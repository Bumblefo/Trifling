using UnityEngine;
using System.Collections;

public abstract class TileMovingObject : MonoBehaviour {

    public float moveTime; // time it takes to move in sec
    public LayerMask blockingLayer;
    protected const float blockSize = 0.32f;

    protected BoxCollider2D boxColl;
    protected Rigidbody2D body;
    protected float moveSpeed;

	// Use this for initialization
	protected virtual void Start () {
        boxColl = GetComponent<BoxCollider2D>();
        body = GetComponent<Rigidbody2D>();
        moveSpeed = 1f;
	}

    protected bool Move(int xDir, int yDir, out RaycastHit2D hit)
    {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir * blockSize, yDir * blockSize); // 

        boxColl.enabled = false; //Turn off boxCollider2D so lineCast doesn't hit player's own boxCollider2D
        hit = Physics2D.Linecast(start, end, blockingLayer);
        boxColl.enabled = true;

        if (hit.transform == null) //If it didn't hit any blockingLayer then start movement
        {
            transform.position = end;
            //StartCoroutine(SmoothMovement(end));
            return true;
        }

        return false;
    }

    protected IEnumerator SmoothMovement(Vector3 end)
    {
        while (transform.position != end)
        {
            transform.position = Vector3.MoveTowards(transform.position, end, moveSpeed * Time.deltaTime);
            yield return null;
        }

        /*
        //From Unity 2d Roguelike example
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        while (sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(body.position, end, inverseMoveTime * Time.deltaTime);
            body.MovePosition(newPosition);
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            yield return null;
        }
        */
    }

    protected virtual void AttemptMove<T>(int xDir, int yDir)
        where T : Component
    {
        RaycastHit2D hit;
        bool canMove = Move(xDir, yDir, out hit);

        if (hit.transform == null) //If it didn't hit a blocking layer then move
        {
            return;
        }

        //If it did hit a blocking layer then get T component of the blockingLayer. If it can't move and the object with the blockingLayer
        //has T component then call OnCantMove
        T hitComponent = hit.transform.GetComponent<T>();

        if (!canMove && hitComponent != null)
        {
            OnCantMove(hitComponent);
        }
    } 

    protected abstract void OnCantMove<T>(T component)
        where T : Component;
}
