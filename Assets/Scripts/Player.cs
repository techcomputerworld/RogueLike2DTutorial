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
        // touchOrigin no tiene ningun valor que nos sirva 
        private Vector2 touchOrigin = -Vector2.one;

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
            int horizontal = 0;
            int vertical = 0;
            //esto solo funciona con teclado ahora mismo
            /* Le quiero dar soporte a GamePad fisico y tambien a un gamepad como hicimos en space shooter virtual para Smartphone. */
            /* UNITY_STANDALONE plataforma Windows, Linux y Mac OS X.
             * UNITY_WEBGL web player jugar desde el navegador.
             */
#if UNITY_STANDALONE || UNITY_WEBGL || UNITY_EDITOR
            horizontal = (int)Input.GetAxisRaw("Horizontal");
            vertical = (int)Input.GetAxisRaw("Vertical");
            //evitar que se mueva en diagonal 
            if (horizontal != 0)
                vertical = 0;
#else
            //gestos tactiles se hace el gesto tacil en la pantalla, con el dedo si el movimiento va en el eje X de izquierda a derecha o derecha y izquierda se traza una linea
            // en la pantalla y se detecta de que punto hasta que punto va el dedo
            // si es de arriba a abajo y al reves se detecta y estas mvoviendote en el eje Y y describimos un vector de la misma manera que en el eje X detectando inicio y final.
            if (Input.touchCount > 0)
            {
                //ahi estamos obteniendo la informacion del primer dedo que toca la pantalla 
                Touch myTouch = Input.touches[0];
                if (myTouch.phase == TouchPhase.Began)
                {
                    touchOrigin = myTouch.position;
                }
                //miramos que el Ended es la final phase cuando levantamos el dedo y que ademas el touchOrigin sea distinto vamos que hayamos movido el dedo
                else if (myTouch.phase == TouchPhase.Ended && touchOrigin!=-Vector2.one)
                {
                    Vector2 touchEnd = myTouch.position;
                    //calculamos un valor que nos dice cuanto se ha desplazado en el eje X el dedo y en el eje Y, estos valores pueden estar en negativo. 
                    float x = touchEnd.x - touchEnd.x;
                    float y = touchEnd.y - touchEnd.y;
                    if (Mathf.Abs(x) > Mathf.Abs(y))
                    {
                        horizontal = x > 0 ? 1 : -1;
                        //ver como se haria con esto de Mathf.Sign
                        //Mathf.Sign(horizontal);
                    }
                    else
                    {
                        vertical = y > 0 ? 1 : -1;
                    }

                }
            }
            
        #endif
            if (horizontal != 0 || vertical != 0)
            {
                AttemptMove(horizontal, vertical);
                
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
