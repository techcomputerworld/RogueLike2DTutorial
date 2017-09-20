using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Rogue
{
    public class Player : MovingObject
    {
        public AudioClip moveSound1, moveSound2, eatSound1, eatSound2, drinkSound1, drinkSound2, gameOverSound;

        public int wallDamage = 1;
        public int pointsPerFood = 10;
        public int pointsPerSoda = 20;
        //El tiempo en recargar el siguietne nivel de 1 segundo que nosotros lo hemos definido de tipo float
        public float restartLevelDelay = 1f;
        public Text foodText;
        private Animator animator;
        private int food = 0;

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
            //foodText.text = "Food: " + food;
            FoodPoints(food);
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
            {
                SoundManager.instance.PlaySingle(gameOverSound);
                GameManager.Instance.GameOver();
            }
                
        }

        //crearemos parametros para los otros metodos 
        
        private void FoodPoints(int food)
        {
            
            foodText.text = "Food: " + food;
        }
        
        protected override bool AttemptMove(int xDir, int yDir)
        {
            //la comida se decrementa su valor en 1, cada movimiento decrementa su valor
            food--;
            FoodPoints(food);
            //foodText.text = "Food: " + food;
            bool canMove = base.AttemptMove(xDir, yDir);
            if (canMove)
                SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
            //cada vez que se decrementa el valoe food tenemos que comprobar que no esta en 0 su valor, si esta a 0 se activara gameOver()
            checkIfGameOver();
            GameManager.Instance.playerTurn = false;
            return canMove;
        }

        private void Update()
        {
            //Si no es el turno del jugador salimos y no nos dejara mover
            if (!GameManager.Instance.playerTurn || GameManager.Instance.doingSetup) return;
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
            {
                bool canMove = AttemptMove(horizontal, vertical);
                
            }
               
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

        private void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void LoseFood(int loss)
        {
            food -= loss;
            foodText.text = "- " + loss + " Food: " + food;
            animator.SetTrigger("playerHit");
            checkIfGameOver();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Exit"))
            {
                Invoke("Restart", restartLevelDelay);
                enabled = false;
            }
            else if (other.CompareTag("Food"))
            {

                food += pointsPerFood;
                SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);
                foodText.text = "+ " + pointsPerFood + " Food: " + food;
                // no lo destruye y no hace trabajar al recolector de basura simplemente desactiva el objeto 
                other.gameObject.SetActive(false);

            }
            else if (other.CompareTag("Soda"))
            {
                food += pointsPerSoda;
                SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);
                foodText.text = "+ " + pointsPerSoda + " Food: " + food;
                other.gameObject.SetActive(false);
            }

        }

    }
}
