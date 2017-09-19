using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Rogue // esto es una prueba para ver si funciona como debe funcionar el juego sin meter los objetos del namespace Completed
{

    public class GameManager : MonoBehaviour
    {

        public static GameManager Instance;
        public float turnDelay = 0.1f;
        public BoardManager boardScript;
        //indica el tiempo en total de segundos que se vera la pantalla del canvas que indica el nivel de juego
        public float levelStartDelay = 2f;
        //hicimos que GameManager no se destruyera para poder guardar el valor deesta variable
        public int playerFoodPoints = 100;
        public bool doingSetup;
        //esta variable aunque sea publica, la hacemos invisible en el inspector
        [HideInInspector]
        public bool playerTurn = true;

        private List<Enemy> enemies = new List<Enemy>();
        //variable para saber si los enemigos se estan moviendo 
        private bool enemiesMoving;
        private int level = 0;
        /*  esdtos 2 objetos van a estar creandose y destruyendose en cada escena, por eso no los asignamos en el inspector y los asignamos por codigo*/

        private GameObject levelImage;
        private Text levelText;


        private void Awake()
        {
            if (GameManager.Instance == null)
            {
                GameManager.Instance = this;
            }
            else if (GameManager.Instance != this)
            {
                Destroy(gameObject);
            }

            //que se marque para que nunca se destruya
            DontDestroyOnLoad(gameObject);

            boardScript = GetComponent<BoardManager>();


        }
        /*
        private void Start()
        {
            InitGame();
        }*/
        private void Update()
        {
            if (playerTurn || enemiesMoving || doingSetup) return;
            StartCoroutine(MoveEnemies());
        }
        private void InitGame()
        {
            //Este codigo prepara en base al nivel que vayamos a jugar.
            doingSetup = true;
            levelImage = GameObject.Find("LevelImage");
            levelText = GameObject.Find("LevelText").GetComponent<Text>();
            levelText.text = "Day " + level;
            levelImage.SetActive(true);
            //este codigo prepara cada nivel
            //limpiar la lista de enemigos
            enemies.Clear();
            boardScript.SetupScene(level);

            Invoke("HideLevelImage", levelStartDelay);
        }

        private void HideLevelImage()
        {
            levelImage.SetActive(false);
            doingSetup = false;
        }

        public void GameOver()
        {
            //despues de tantos dias te moristes de hambre, o te has muerto de hambre
            levelText.text = "After " + level + "days, you starved.";
            levelImage.SetActive(true);
            //desactivamos el componente GameManager 
            enabled = false;
        }

        //corutinas para mover los enemigos
        IEnumerator MoveEnemies()
        {
            //vamos a mover los enemigos y lo ponemos a true
            enemiesMoving = true;
            yield return new WaitForSeconds(turnDelay);
            if (enemies.Count == 0)
            {
                yield return new WaitForSeconds(turnDelay);
            }
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].MoveEnemy();
                //aqui vamos a esperar el valoe moveTime del objeto MovingObject
                yield return new WaitForSeconds(enemies[i].moveTime);
            }
            playerTurn = true;
            enemiesMoving = false;
        }
        public void AddEnemyToList(Enemy enemy)
        {
            enemies.Add(enemy);
        }
        //Estos metodos me interesan que funcionen 
        private void OnEnable()
        {
            //aqui estamos usando un evento llamado sceneLoaded y llamamos al metodo OnLevelFinishLoading usando un delegado. 
            SceneManager.sceneLoaded += OnLevelFinishLoading;
        }
        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnLevelFinishLoading;
        }
        //usaremos este metodo como delegado cuando se detecte que se ha recargado el nivel 
        private void OnLevelFinishLoading(Scene scene, LoadSceneMode mode)
        {
            level++;
            InitGame();
        }
    }
}
