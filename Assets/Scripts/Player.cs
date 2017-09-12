using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MovingObject {

    public int wallDamage = 1;
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;
    //El tiempo en recargar el siguietne nivel de 1 segundo que nosotros lo hemos definido de tipo float
    public float restartLevelDelay = 1f;

    private Animator animator;
    private int food;

    protected override void Awake()
    {
        //obtenemos el componente Animator de Player
        animator = GetComponent<Animator>();
        //aqui llamamos al metodo Awake() de la clase derivada.
        base.Awake();
    }
    protected override void Start()
    {
        food = GameManager.Instance.playerFoodPoints;
        base.Start();
    }
    
    //Metodo que se ejecutara justo antes del OnDestroy, que se ejecutara cuando el objeto Player, sea destruido para recargar la escena. 
    private void OnDisable()
    {
        GameManager.Instance.playerFoodPoints = food;
          
    }
    //metodo para comprobar si hemos llegado a GameOver 
    private void checkIfGameOver()
    {
        if (food <= 0)
            GameManager.Instance.GameOver();
    }

    protected override void AttemptMove(int xDir, int yDir)
    {
        //la comida se decrementa su valor en 1, cada movimiento decrementa su valor
        food--;
        base.AttemptMove(xDir, yDir);
        //cada vez que se decrementa el valoe food tenemos que comprobar que no esta en 0 su valor, si esta a 0 se activara gameOver()
        checkIfGameOver();
        GameManager.Instance.playerTurn = false;

    }
    
    private void Update()
    {
        //Si no es el turno del jugador salimos y no nos dejara mover
        if (!GameManager.Instance.playerTurn) return;
        //si es el turno del jugador 
        int horizontal;
        int vertical;
        //esto solo funciona con teclado ahora mismo
        /* Le quiero dar soporte a GamePad fisico y tambien a un gamepad como hicimos en space shooter virtual para Smartphone. */ 
        horizontal = (int)Input.GetAxisRaw("Horizontal");
        vertical = (int)Input.GetAxisRaw("Vertical");
        //evitar que se mueva en diagonal 
        if (horizontal != 0)
            vertical = 0;
        if (horizontal != 0 || vertical != 0)
            AttemptMove(horizontal, vertical);
    }

    protected override void OnCantMove(GameObject go)
    {
        //la unica accion especial que tenemos es cuando intentamos movernos a un muro interior del 6x6
        Wall hitWall = go.GetComponent<Wall>();
        if (hitWall != null)
        {
            hitWall.DamageWall(wallDamage);
            animator.SetTrigger("playerChop");
        }
    }

    private void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoseFood(int loss)
    {
        food -= loss;
        animator.SetTrigger("playerHit");
        checkIfGameOver();
    }

}
