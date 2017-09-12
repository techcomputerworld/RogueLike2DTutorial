using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour {


    public float moveTime = 0.1f;
    public LayerMask blockingLayer;
    // movementSpeed un float que nos indicara la velocidad de movimiento de los objetos. 
    private float movementSpeed;
    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;

    protected virtual void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
    }
    // Use this for initialization
    protected virtual void Start () {
        //la inversa de moveTime
        movementSpeed = 1f / moveTime;	
	}
	
    protected IEnumerator SmoothMovement (Vector2 end)
    {
        //nos da como resultado la distancia que hay entre el punto inicial y el final.
        float remainingDistance = Vector2.Distance(rb2D.position, end);
        while(remainingDistance > float.Epsilon)
        {
            //rb2D.posicion es la posicion inicial, end es la posicion final, movementSpeed * Time.deltaTime la velocidad de avance hacia el punto final.
            Vector2 newPosition = Vector2.MoveTowards(rb2D.position, end, movementSpeed * Time.deltaTime);
            rb2D.MovePosition(newPosition);
            remainingDistance = Vector2.Distance(rb2D.position, end);
            yield return null;
        }
    }
        
    protected bool Move(int xDir, int yDir, out RaycastHit2D hit)
    {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir, yDir);
        //
        boxCollider.enabled = false;
        /* El RayCastHit2D lo hacemos aqui si nos encontramos con un boxCollider no podremos hacer el movimiento 
         * pero si no hay un boxCollider se terminara de hacer el movimiento
         */
        hit = Physics2D.Linecast(start, end, blockingLayer);
        boxCollider.enabled = true;
        //si nos da null quiere decir que bnoi habia ningun objeto con blockingLayer 
        if (hit.transform == null)
        {
            //Hacer el movimiento
            StartCoroutine(SmoothMovement(end));
            return true;
        }
        else
        {
            // no nos hemos podido mover
            return false;
        }

    }
    //este metodo es abstracto lo implementaremos en las clases que hereden de MovingObject
    protected abstract void OnCantMove(GameObject go);
    // vamos a realizar este metodo sin usar un metodo generico
    protected virtual void AttemptMove(int xDir, int yDir)
    {
        RaycastHit2D hit;
        bool canMove = Move(xDir, yDir, out hit);
        if (canMove) return;
        OnCantMove(hit.transform.gameObject);

    }
}
