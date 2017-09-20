using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rogue
{
    public class Enemy : MovingObject
    {
        public AudioClip enemyAttack1, enemyAttack2;
        public int playerDamage;

        private Animator animator;
        //se van a mover dependiendo de donde este el jugador
        private Transform target;
        //Se muevan un turno si y otro no se movera
        private bool skipMove;

        protected override void Awake()
        {
            //obtener el componente Animator
            animator = GetComponent<Animator>();
            base.Awake();
        }

        // Use this for initialization
        protected override void Start()
        {
            GameManager.Instance.AddEnemyToList(this);
            // cuando se vaya moviendo el objeto Player se ira cambiando su transform y habra que decirselo al objeto Enemy
            target = GameObject.FindWithTag("Player").transform;
            base.Start();
        }

        // Update is called once per frame
        void Update()
        {

        }
        // metodo que prueba a mover al enemigo hacia una direccion 
        protected override bool AttemptMove(int xDir, int yDir)
        {
            //solo estamos devolviendo el valor porque el metodo es bool pero no porque lo necesitemos el metodo base. 
            if (skipMove)
            {
                skipMove = false;
                return false;
            }
            bool canMove = base.AttemptMove(xDir, yDir);
            skipMove = true;
            return canMove;
        }

        public void MoveEnemy()
        {
            int xDir = 0, yDir = 0;
            //comprobaremos si esta en la misma columna, a mi se me ocurre otra forma de hacerlo viendo en que columna y fila esta cada uno
            // si la posicion absoluta entre el target.position.x es muy peque�a con el transform.position.x significa que estan en la misma coloumna
            if (Math.Abs(target.position.x - transform.position.x) < float.Epsilon)
            {
                //ternario que significa que si la target.transform.position.y es mayor que transform.position.y sera igual a 1 y si no -1 
                yDir = target.transform.position.y > transform.position.y ? 1 : -1;

            }
            else
            {
                xDir = target.position.x > transform.position.x ? 1 : -1;
            }
            AttemptMove(xDir, yDir);
        }
        protected override void OnCantMove(GameObject go)
        {
            Player hitPlayer = go.GetComponent<Player>();

            if (hitPlayer != null)
            {
                hitPlayer.LoseFood(playerDamage);
                animator.SetTrigger("enemyAttack");
                SoundManager.instance.RandomizeSfx(enemyAttack1, enemyAttack2);
            }
            /* aqui voy a poner en otra version del videojuego, que si es un wall con lo que me topo, no le hara da�o, pero se movera a otro sitio mas interesante 
             * en contra del jugador.
             */

        }
    }

}