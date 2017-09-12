using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;
    public BoardManager boardScript;
    //hicimos que GameManager no se destruyera para poder guardar el valor deesta variable
    public int playerFoodPoints = 100;
    //esta variable aunque sea publica, la hacemos invisible en el inspector
    [HideInInspector]public bool playerTurn = true;

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

    private void Start()
    {
        InitGame();
    }
    private void InitGame()
    {
        boardScript.SetupScene(3);
    }
    public void GameOver()
    {
        //desactivamos el componente GameManager 
        enabled = false;
    }
}
